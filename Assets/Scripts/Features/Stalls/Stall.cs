using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Stall : Interactable
{
    [SerializeField] private StallCooldown stallCooldown;
    public StallCooldown GetStallCooldown() => stallCooldown;

    private Stall_Inner_UI stallInnerUI;
    private StallUI stallUIComponent;
    
    private HaggleSystem haggleSystem;
    private GameObject stallUI;
    private Button[] itemButtons;
    private ItemData[] assignedItems;
    private int[] stockAmounts;
    private int selectedItemIndex = -1;
    private bool isInitialized = false;

    private string discountedItemId = null;

    [SerializeField]
    public bool useManualItems = false;

    [SerializeField]
    public List<string> manuallyAssignedItemIDs = new List<string>();

    public void Initialize(HaggleSystem haggleSystemRef, GameObject uiRef, Button[] buttonArray)
    {
        haggleSystem = haggleSystemRef;
        stallUI = uiRef;
        itemButtons = buttonArray;

        AssignItems();
        isInitialized = true;

        stallInnerUI = Object.FindAnyObjectByType<Stall_Inner_UI>();
    }

    private void AssignItems()
    {
        var db = ItemDatabaseManager.Instance;
        if (db == null || db.itemDatabase == null) return;

        LevelData currentLevelData = LevelStateManager.Instance.GetCurrentLevelData();

        if (currentLevelData == null)
        {
            Debug.LogError("Current level data is missing!");
            return;
        }

        int minItemsToStock = currentLevelData.minStallItemStock;
        int maxItemsToStock = currentLevelData.maxStallItemStock;

        int itemCount = Random.Range(minItemsToStock, maxItemsToStock + 1);
        itemCount = Mathf.Min(itemCount, itemButtons.Length);  

        assignedItems = new ItemData[itemCount];
        stockAmounts = new int[itemCount];

        for (int i = 0; i < itemCount; i++)
        {
            ItemData item = null;

            if (useManualItems && manuallyAssignedItemIDs != null && manuallyAssignedItemIDs.Count > 0)
            {
                if (i < manuallyAssignedItemIDs.Count)
                {
                    item = db.GetItem(manuallyAssignedItemIDs[i]);
                }
            }
            else
            {
                List<ItemData> databaseItems = new List<ItemData>(db.itemDatabase.items);
                item = databaseItems[Random.Range(0, databaseItems.Count)];
            }

            if (item != null)
            {
                assignedItems[i] = item;
                stockAmounts[i] = Random.Range(1, item.stockLimit + 1);
            }
            else
            {
                Debug.LogWarning("No valid item found to assign to button.");
            }
        }
    }

    public override void Interact()
    {
        if (!isInitialized) return;
        if (stallCooldown != null && stallCooldown.isCoolingDown) return;

        base.Interact();

        if (stallUI != null)
        {
            stallUI.SetActive(true);
            var ui = GetComponent<StallUI>();
            if (ui != null)
            {
                ui.SetStallReference(this);
                ui.DisplayItems(assignedItems, stockAmounts);
                stallInnerUI.UpdateUI(ui);
            }
        }
    }

    public void TryStartHaggling()
    {
        DialogueManager dialogueManager = Object.FindAnyObjectByType<DialogueManager>();
        if (dialogueManager == null)
        {
            Debug.LogWarning("DialogueManager not found in scene.");
            return;
        }

        if (haggleSystem != null)
        {
            haggleSystem.StartHaggle(dialogueManager, this);
        }
        else
        {
            Debug.LogWarning("HaggleSystem not assigned.");
        }
    }

    public ItemData GetSelectedItem()
    {
        return (selectedItemIndex >= 0 && selectedItemIndex < assignedItems.Length) ? assignedItems[selectedItemIndex] : null;
    }

    public void SetSelectedItem(int index)
    {
        if (index >= 0 && index < assignedItems.Length)
            selectedItemIndex = index;
    }

    public int SelectedItemIndex => selectedItemIndex;

    public string GetDiscountedItemId() => discountedItemId;

    public void ApplyHaggleDiscount(string itemId)
    {
        discountedItemId = itemId;
        UpdateSelectedItemUIAfterHaggle();
    }

    public void ResetDiscount()
    {
        discountedItemId = null;
    }

    public bool PurchaseItem(int index)
    {
        if (index < 0 || index >= assignedItems.Length) return false;

        ItemData item = assignedItems[index];
        int stock = stockAmounts[index];

        if (item == null || stock <= 0) return false;

        var runtimeCharacter = CharacterSelectionManager.Instance?.SelectedRuntimeCharacter;
        if (runtimeCharacter == null) return false;

        float finalPrice = item.price;

        if (item.id == discountedItemId)
        {
            finalPrice *= 0.5f;
            finalPrice = Mathf.Round(finalPrice);
        }

        if (runtimeCharacter.currentWeeklyBudget < finalPrice)
        {
            Debug.Log("Not enough budget.");
            return false;
        }

        stockAmounts[index]--;
        runtimeCharacter.currentWeeklyBudget -= (int)finalPrice;

        Inventory.Instance.AddItem(item, 1);  
        if (item.id == discountedItemId)
            ResetDiscount();

        WellBeingEvents.OnWellBeingChanged?.Invoke(item.nutrition, item.satisfaction);

        LevelManager levelManager = Object.FindAnyObjectByType<LevelManager>();
        if (levelManager != null)
            levelManager.UpdateBudgetDisplay();

        StallManager.Instance?.ReduceGlobalItemCount(1);
        return true;
    }


    public bool PurchaseItemForNPC(int index)
    {
        if (index < 0 || index >= assignedItems.Length) return false;

        ItemData item = assignedItems[index];
        int stock = stockAmounts[index];

        if (item == null || stock <= 0) return false;

        stockAmounts[index]--;
        Debug.Log($"NPC purchased {item.itemName}!");
        StallManager.Instance?.ReduceGlobalItemCount(1);

        return true;
    }

    public void OnPurchaseButtonPressed()
    {
        PurchaseItem(selectedItemIndex);

        StallUI stallUI = GetComponent<StallUI>();
        if (stallUI != null)
        {
            stallUI.HideDetailsAfterPurchase();
        }

        if (stallCooldown != null)
        {
            stallCooldown.TriggerCooldown();
        }
    }

    private void UpdateSelectedItemUIAfterHaggle()
    {
        if (selectedItemIndex < 0 || selectedItemIndex >= assignedItems.Length) return;

        var item = assignedItems[selectedItemIndex];
        float finalPrice = item.price;

        if (item.id == discountedItemId)
        {
            finalPrice *= 0.5f;
            finalPrice = Mathf.Round(finalPrice);
        }

        var ui = GetComponent<StallUI>();
        if (ui != null)
            ui.UpdateSelectedItemPrice(finalPrice);
    }

    public (ItemData, int) GetItemAndStock(int index) =>
        (index < 0 || index >= assignedItems.Length) ? (null, 0) : (assignedItems[index], stockAmounts[index]);

    public Button[] GetItemButtons() => itemButtons;

    public ItemData[] GetAssignedItems() => assignedItems;

    public int GetItemIndex(ItemData item)
    {
        for (int i = 0; i < assignedItems.Length; i++)
        {
            if (assignedItems[i].id == item.id)
            {
                return i;
            }
        }
        return -1;
    }
    public int GetTotalAssignedItemCount()
    {
        int total = 0;
        if (stockAmounts != null)
        {
            foreach (int stock in stockAmounts)
            {
                total += stock;
            }
        }
        return total;
    }

    public Vector3 GetApproachPoint()
    {
        Vector2Int gridPos = PathfindingGrid.Instance.GetGridPosition(interactionTransform.position + (Vector3)boxOffset);
        return PathfindingGrid.Instance.GetWorldPosition(gridPos.x, gridPos.y);
    }
}
