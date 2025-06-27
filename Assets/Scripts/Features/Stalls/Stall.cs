using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Stall : Interactable
{
    [SerializeField] private StallCooldown stallCooldown;
    public StallCooldown GetStallCooldown() => stallCooldown;
    private HaggleSystem haggleSystem;
    private GameObject stallUI;
    private Button[] itemButtons;
    private ItemData[] assignedItems;
    private int[] stockAmounts;
    private int selectedItemIndex = -1;
    private bool isInitialized = false;

    private string discountedItemId = null;

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
        var db = ItemDatabaseManager.Instance;
        if (db == null || db.itemDatabase == null) return;

        var databaseItems = new List<ItemData>(db.itemDatabase.items);
        int itemCount = Mathf.Min(itemButtons.Length, databaseItems.Count);

        assignedItems = new ItemData[itemCount];
        stockAmounts = new int[itemCount];

        for (int i = 0; i < databaseItems.Count; i++)
        {
            int j = Random.Range(i, databaseItems.Count);
            (databaseItems[i], databaseItems[j]) = (databaseItems[j], databaseItems[i]);
        }

        for (int i = 0; i < itemCount; i++)
        {
            assignedItems[i] = databaseItems[i];
            stockAmounts[i] = Random.Range(1, assignedItems[i].stockLimit + 1);
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
            var ui = Object.FindAnyObjectByType<StallUI>();
            if (ui != null)
            {
                ui.SetStallReference(this);
                ui.DisplayItems(assignedItems, stockAmounts);
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

        StallUI stallUI = GetComponent<StallUI>();
        if (stallUI != null)
        {
            stallUI.HideDetailsAfterHaggle();
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
    }

    public void ResetDiscount()
    {
        discountedItemId = null;
    }

    public bool PurchaseItem(int index)
    {
        if (index < 0 || index >= assignedItems.Length)
            return false;

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

        if (item.id == discountedItemId)
            ResetDiscount();

        LevelManager levelManager = Object.FindAnyObjectByType<LevelManager>();
        if (levelManager != null)
            levelManager.UpdateBudgetDisplay();

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

    public void UpdateSelectedItemUIAfterHaggle()
    {
        if (selectedItemIndex < 0 || selectedItemIndex >= assignedItems.Length) return;

        var item = assignedItems[selectedItemIndex];
        float finalPrice = item.price;

        if (item.id == discountedItemId)
        {
            finalPrice *= 0.5f;
            finalPrice = Mathf.Round(finalPrice);
        }

        var ui = Object.FindAnyObjectByType<StallUI>();
        if (ui != null)
            ui.UpdateSelectedItemPrice(finalPrice);
    }

    public (ItemData, int) GetItemAndStock(int index) =>
        (index < 0 || index >= assignedItems.Length) ? (null, 0) : (assignedItems[index], stockAmounts[index]);
        
    public Button[] GetItemButtons() => itemButtons;
}
