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
                case 0: discountPercentage = 0.30f; break;  // 30% discount for first attempt
                case 1: discountPercentage = 0.20f; break;  // 20% for second attempt
                case 2: discountPercentage = 0.10f; break;  // 10% for third attempt
            }

            // Apply discount to the item price
            float originalPrice = originalPrices.ContainsKey(item.id) ? originalPrices[item.id] : item.price;
            item.price = Mathf.RoundToInt(originalPrice * (1 - discountPercentage));

            // Update UI after haggling
            var stallUI = GetComponent<StallUI>();
            if (stallUI != null)
            {
                stallUI.UpdateSelectedItemPrice(originalPrice, item.price);  // Update with original and discounted prices
            }
        }
    }



    public float GetDiscountPercentage() => discountPercentage;

    public void ResetDiscount()
{
    discountedItemId = null;
    discountPercentage = 0f;

    // Update the UI to reset the item price to the original price
    var stallUI = GetComponent<StallUI>();
    if (stallUI != null)
    {
        // Get the item details
        var item = assignedItems[selectedItemIndex];

        // Retrieve the original price from the originalPrices dictionary
        float originalPrice = originalPrices.ContainsKey(item.id) ? originalPrices[item.id] : item.price;

        // Pass both original and discounted price (the same if no discount)
        stallUI.UpdateSelectedItemPrice(originalPrice, originalPrice);  // No discount, so pass the same value for both
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
        
            // Apply the discount based on the attempt count (from HaggleSystem)
            float finalPrice = item.price * (1 - discountPercentage);  // Dynamic discount logic
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
                    stallUI.UpdateDisplayItemsForPlayer(assignedItems, stockAmounts, this);
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
    float originalPrice = originalPrices.ContainsKey(item.id) ? originalPrices[item.id] : item.price;  // Get the original price
    float discountedPrice = item.price;  // Default to the current price

    // Apply discount (if any)
    float discountPercentage = this.GetDiscountPercentage();
    if (discountPercentage > 0)
    {
        discountedPrice = Mathf.RoundToInt(originalPrice * (1 - discountPercentage));  // Apply the discount and round
    }

    Debug.Log($"Original Price: ₱{originalPrice}, Discounted Price: ₱{discountedPrice}");

    // Pass both original and discounted price to StallUI
    var ui = GetComponent<StallUI>();
    if (ui != null)
    {
        ui.UpdateSelectedItemPrice(originalPrice, discountedPrice);  // Pass both prices
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
