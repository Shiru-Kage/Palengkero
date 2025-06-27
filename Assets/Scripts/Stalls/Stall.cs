using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Stall : Interactable
{
    [SerializeField] private StallCooldown stallCooldown;

    private HaggleSystem haggleSystem;
    private GameObject stallUI;
    private Button[] itemButtons;

    [Header("Manual Setup (optional)")]
    public bool useRandomItems = true;
    public string[] specificItemIds;
    public int customStock = -1;

    private ItemData[] assignedItems;
    private int[] stockAmounts;
    private int selectedItemIndex = -1;

    private bool isInitialized = false;

    public void Initialize(HaggleSystem haggleSystemRef, GameObject uiRef, Button[] buttonArray)
    {
        haggleSystem = haggleSystemRef;
        stallUI = uiRef;
        itemButtons = buttonArray;

        AssignItems();
        isInitialized = true;
    }

    private void AssignItems()
    {
        if (ItemDatabaseManager.Instance == null || ItemDatabaseManager.Instance.itemDatabase == null)
        {
            Debug.LogError("ItemDatabaseManager not ready!");
            return;
        }

        if (itemButtons == null || itemButtons.Length == 0)
        {
            Debug.LogError("ItemButtons not set. Call Initialize() first.");
            return;
        }

        var databaseItems = new List<ItemData>(ItemDatabaseManager.Instance.itemDatabase.items);
        int itemCount = Mathf.Min(itemButtons.Length, databaseItems.Count);

        assignedItems = new ItemData[itemCount];
        stockAmounts = new int[itemCount];

        if (useRandomItems)
        {
            for (int i = 0; i < databaseItems.Count; i++)
            {
                int j = Random.Range(i, databaseItems.Count);
                (databaseItems[i], databaseItems[j]) = (databaseItems[j], databaseItems[i]);
            }

            for (int i = 0; i < itemCount; i++)
            {
                assignedItems[i] = databaseItems[i];
                stockAmounts[i] = (customStock >= 0)
                    ? customStock
                    : Random.Range(1, assignedItems[i].stockLimit + 1);
            }
        }
        else
        {
            for (int i = 0; i < itemCount; i++)
            {
                if (i < specificItemIds.Length)
                {
                    assignedItems[i] = ItemDatabaseManager.Instance.GetItem(specificItemIds[i]);
                    if (assignedItems[i] == null)
                    {
                        Debug.LogWarning($"Missing item for ID: {specificItemIds[i]}");
                        continue;
                    }

                    stockAmounts[i] = (customStock >= 0)
                        ? customStock
                        : Random.Range(1, assignedItems[i].stockLimit + 1);
                }
            }
        }
    }

    public override void Interact()
    {
        if (!isInitialized)
        {
            Debug.LogWarning("Stall not initialized. Interaction blocked.");
            return;
        }

        if (stallCooldown != null && stallCooldown.isCoolingDown)
        {
            Debug.Log("Stall is cooling down. Interaction blocked.");
            return;
        }

        base.Interact();

        if (stallUI != null)
        {
            stallUI.SetActive(true);

            var ui = Object.FindAnyObjectByType<StallUI>();
            if (ui != null)
            {
                ui.SetStallReference(this);
                ui.DisplayItems(assignedItems, stockAmounts);
            }
            else
            {
                Debug.LogWarning("StallUI component not found.");
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
            haggleSystem.StartHaggle(dialogueManager);
        }
        else
        {
            Debug.LogWarning("HaggleSystem not assigned.");
        }
        StallUI stallUI = GetComponent<StallUI>();
        if (stallUI != null)
        {
            stallUI.HideDetailsAfterHaggle();
        }
    }

    public (ItemData, int) GetItemAndStock(int index) =>
        (index < 0 || index >= assignedItems.Length) ? (null, 0) : (assignedItems[index], stockAmounts[index]);

    public void SetSelectedItem(int index)
    {
        if (index >= 0 && index < assignedItems.Length)
            selectedItemIndex = index;
    }

    public int SelectedItemIndex => selectedItemIndex;

    public ItemData GetSelectedItem() =>
        (selectedItemIndex >= 0 && selectedItemIndex < assignedItems.Length) ? assignedItems[selectedItemIndex] : null;

    public void PurchaseSelectedItem()
    {
        if (selectedItemIndex < 0 || selectedItemIndex >= assignedItems.Length)
        {
            Debug.LogWarning("No item selected for purchase.");
            return;
        }

        PurchaseItem(selectedItemIndex);
    }

    public bool PurchaseItem(int index)
    {
        if (index < 0 || index >= assignedItems.Length)
            return false;

        ItemData item = assignedItems[index];
        int stock = stockAmounts[index];

        if (item == null || stock <= 0)
        {
            Debug.LogWarning("Item is null or out of stock.");
            return false;
        }

        var runtimeCharacter = CharacterSelectionManager.Instance?.SelectedRuntimeCharacter;
        if (runtimeCharacter == null)
        {
            Debug.LogWarning("No runtime character selected.");
            return false;
        }

        float finalPrice = item.price;

        if (haggleSystem != null && item.id == haggleSystem.DiscountedItemId)
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

        if (haggleSystem != null && item.id == haggleSystem.DiscountedItemId)
            haggleSystem.ResetDiscount();

        Debug.Log($"Purchased {item.itemName} for ₱{finalPrice}. Remaining budget: ₱{runtimeCharacter.currentWeeklyBudget}");

        LevelManager levelManager = Object.FindAnyObjectByType<LevelManager>();
        if (levelManager != null)
            levelManager.UpdateBudgetDisplay();

        return true;
    }

    public void OnPurchaseButtonPressed()
    {
        PurchaseSelectedItem();

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

    public void UpdateSelectedItemUIAfterHaggle()
    {
        if (selectedItemIndex < 0 || selectedItemIndex >= assignedItems.Length)
            return;

        var item = assignedItems[selectedItemIndex];
        float finalPrice = item.price;

        if (haggleSystem != null && item.id == haggleSystem.DiscountedItemId)
        {
            finalPrice *= 0.5f;
            finalPrice = Mathf.Round(finalPrice);
        }

        var ui = Object.FindAnyObjectByType<StallUI>();
        if (ui != null)
            ui.UpdateSelectedItemPrice(finalPrice);
    }
}
