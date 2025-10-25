using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public enum BuyerType
{
    Player,
    NPC
}

public class Stall : Interactable
{
    [SerializeField] private StallCooldown stallCooldown;
    public StallCooldown GetStallCooldown() => stallCooldown;

    private Stall_Inner_UI stallInnerUI;
    private LogBookUI logBookUI;
    private StallUI stallUIComponent;
    private HaggleSystem haggleSystem;
    private GameObject stallUI;
    private Button[] itemButtons;
    private ItemData[] assignedItems;
    private NPC_Shopper_Behavior reservedBy;

    public bool IsReserved => reservedBy != null;
    private int[] stockAmounts;
    private int selectedItemIndex = -1;
    private bool isInitialized = false;

    private string discountedItemId = null;
    private float discountPercentage = 0f; 

    [SerializeField]
    public bool useManualItems = false;

    [SerializeField]
    public List<string> manuallyAssignedItemIDs = new List<string>();
    private Dictionary<string, float> originalPrices = new Dictionary<string, float>();


    public void Initialize(HaggleSystem haggleSystemRef, GameObject uiRef, Button[] buttonArray)
    {
        haggleSystem = haggleSystemRef;
        stallUI = uiRef;
        itemButtons = buttonArray;

        AssignItems();
        isInitialized = true;

        stallInnerUI = UnityEngine.Object.FindAnyObjectByType<Stall_Inner_UI>();
        logBookUI = UnityEngine.Object.FindAnyObjectByType<LogBookUI>();
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

        int itemCount = UnityEngine.Random.Range(minItemsToStock, maxItemsToStock + 1);
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
                item = databaseItems[UnityEngine.Random.Range(0, databaseItems.Count)];
            }

            if (item != null)
            {
                assignedItems[i] = item;
                stockAmounts[i] = UnityEngine.Random.Range(1, item.stockLimit + 1);
                originalPrices[item.id] = item.price;
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
        DialogueManager dialogueManager = UnityEngine.Object.FindAnyObjectByType<DialogueManager>();
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

        if (assignedItems == null || assignedItems.Length == 0) return;

        ItemData item = Array.Find(assignedItems, i => i.id == itemId);
        if (item != null)
        {
            float discountPercentage = 0f;
            int haggleAttemptCount = haggleSystem.GetAttemptCount(); 
            switch (haggleAttemptCount)
            {
                case 0: discountPercentage = 0.30f; break;
                case 1: discountPercentage = 0.20f; break; 
                case 2: discountPercentage = 0.10f; break; 
            }
            float originalPrice = originalPrices.ContainsKey(item.id) ? originalPrices[item.id] : item.price;
            item.price = Mathf.RoundToInt(originalPrice * (1 - discountPercentage));

            var stallUI = GetComponent<StallUI>();
            if (stallUI != null)
            {
                stallUI.UpdateSelectedItemPrice(originalPrice, item.price);  
            }
        }
    }



    public float GetDiscountPercentage() => discountPercentage;

    public void ResetDiscount()
    {
        discountedItemId = null;
        discountPercentage = 0f;

        var stallUI = GetComponent<StallUI>();
        if (stallUI != null)
        {
            var item = assignedItems[selectedItemIndex];

            float originalPrice = originalPrices.ContainsKey(item.id) ? originalPrices[item.id] : item.price;

            stallUI.UpdateSelectedItemPrice(originalPrice, originalPrice);
        }
    }



    public bool PurchaseItem(int index, BuyerType buyerType)
    {
        if (index < 0 || index >= assignedItems.Length) return false;

        ItemData item = assignedItems[index];
        int stock = stockAmounts[index];

        if (item == null || stock <= 0) return false;

        stockAmounts[index]--;

        if (buyerType == BuyerType.Player)
        {
            var runtimeCharacter = CharacterSelectionManager.Instance?.SelectedRuntimeCharacter;
            if (runtimeCharacter == null) return false;

            float discountPercentage = this.GetDiscountPercentage();
        
            float finalPrice = item.price * (1 - discountPercentage);  
            finalPrice = Mathf.Round(finalPrice);

            if (runtimeCharacter.currentWeeklyBudget < finalPrice)
            {
                stockAmounts[index]++;
                var stallUI = GetComponent<StallUI>();
                if (stallUI != null)
                {
                    stallUI.PlayPurchaseSound();
                    stallUI.HideDetailsAfterHaggle();
                    stallUI.ShowUnableToPurchasePanel();
                }
                return false;
            }

            runtimeCharacter.currentWeeklyBudget -= (int)finalPrice;

            Inventory.Instance.AddItem(item, 1);
            if (item.id == discountedItemId)
                ResetDiscount();

            WellBeingEvents.OnWellBeingChanged?.Invoke(item.nutrition, item.satisfaction);

            LevelManager levelManager = UnityEngine.Object.FindAnyObjectByType<LevelManager>();
            if (levelManager != null)
                levelManager.UpdateBudgetDisplay();

            ArchiveManager.Instance.OnItemPurchased(new InventoryItem(item, 1));
            logBookUI.AddLog($"- Purchased {item.itemName} for <color=red>{item.price} PHP</color>");
            item.price = Mathf.RoundToInt(originalPrices[item.id]);
            haggleSystem.ResetAttempts();
        }
        else if (buyerType == BuyerType.NPC)
        {
            var player = UnityEngine.Object.FindFirstObjectByType<PlayerController>();
            if (player != null && player.CurrentInteractable == this)
            {
                StallUI stallUI = GetComponent<StallUI>();
                if (stallUI != null)
                {
                    stallUI.RefreshItemDetails(item, stockAmounts[index]);
                }
            }
        }

        StallManager.Instance?.ReduceGlobalItemCount(1);
        return true;
    }

    public void OnPurchaseButtonPressed(BuyerType buyerType)
    {
        bool success = PurchaseItem(selectedItemIndex, buyerType);

        StallUI stallUI = GetComponent<StallUI>();
        if (stallUI != null)
        {
            if (!success && buyerType == BuyerType.Player)
            {
                return;
            }
            stallUI.HideDetailsAfterPurchase();
        }

        if (stallCooldown != null)
        {
            stallCooldown.TriggerCooldown();
        }
    }

    public void UpdateSelectedItemUIAfterHaggle()
{
    if (selectedItemIndex < 0 || selectedItemIndex >= assignedItems.Length) return;

    var item = assignedItems[selectedItemIndex];
    float originalPrice = originalPrices.ContainsKey(item.id) ? originalPrices[item.id] : item.price;  
    float discountedPrice = item.price; 

    float discountPercentage = this.GetDiscountPercentage();
    if (discountPercentage > 0)
    {
        discountedPrice = Mathf.RoundToInt(originalPrice * (1 - discountPercentage)); 
    }

    Debug.Log($"Original Price: ₱{originalPrice}, Discounted Price: ₱{discountedPrice}");

    var ui = GetComponent<StallUI>();
    if (ui != null)
    {
        ui.UpdateSelectedItemPrice(originalPrice, discountedPrice); 
    }
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

    public bool ReserveFor(NPC_Shopper_Behavior npc)
    {
        if (reservedBy == null) 
        {
            reservedBy = npc;
            return true;
        }
        return reservedBy == npc; 
    }
    
    public bool CanReserve(NPC_Shopper_Behavior npc)
    {
        return reservedBy == null || reservedBy == npc;
    }

    public void ReleaseReservation(NPC_Shopper_Behavior npc)
    {
        if (reservedBy == npc)
            reservedBy = null;
    }
}
