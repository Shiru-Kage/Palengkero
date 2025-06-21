using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Stall : Interactable
{
    [Header("Stall UI")]
    [SerializeField] private GameObject stallUI;

    [Header("Manual Setup (optional)")]
    public bool useRandomItems = true;
    public string[] specificItemIds; // Used if not random
    public int customStock = -1;  // -1 = random stock

    [Header("UI References")]
    public Button[] itemButtons;

    private ItemData[] assignedItems;
    private int[] stockAmounts;

    private int selectedItemIndex = -1;

    private void Start()
    {
        AssignItems();
    }

    private void AssignItems()
    {
        if (ItemDatabaseManager.Instance == null || ItemDatabaseManager.Instance.itemDatabase == null)
        {
            Debug.LogError("ItemDatabaseManager not ready!");
            return;
        }

        var databaseItems = new List<ItemData>(ItemDatabaseManager.Instance.itemDatabase.items);
        int itemCount = Mathf.Min(itemButtons.Length, databaseItems.Count);

        assignedItems = new ItemData[itemCount];
        stockAmounts = new int[itemCount];

        if (useRandomItems)
        {
            // Shuffle the list using Fisher-Yates shuffle
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
        base.Interact();
        if (stallUI != null)
        {
            stallUI.SetActive(true);

            StallUI ui = Object.FindAnyObjectByType<StallUI>();
            if (ui != null)
            {
                ui.SetStallReference(this);
                ui.DisplayItems(assignedItems, stockAmounts);
            }
            else
            {
                Debug.LogWarning("StallUI component missing on assigned stallUI object.");
            }
        }
    }

    // Optional getter per item
    public (ItemData, int) GetItemAndStock(int index)
    {
        if (index < 0 || index >= assignedItems.Length)
            return (null, 0);
        return (assignedItems[index], stockAmounts[index]);
    }

    public void SetSelectedItem(int index)
    {
        if (index >= 0 && index < assignedItems.Length)
        {
            selectedItemIndex = index;
        }
    }

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

        var characterManager = CharacterSelectionManager.Instance;
        var runtimeCharacter = characterManager?.SelectedRuntimeCharacter;

        if (runtimeCharacter == null)
        {
            Debug.LogWarning("No runtime character selected.");
            return false;
        }

        if (runtimeCharacter == null)
        {
            Debug.LogWarning("No runtime character selected.");
            return false;
        }

        if (runtimeCharacter.currentWeeklyBudget < item.price)
        {
            Debug.Log("Not enough budget.");
            return false;
        }

        stockAmounts[index]--;
        runtimeCharacter.currentWeeklyBudget -= item.price;

        Debug.Log($"Purchased {item.itemName} for ₱{item.price}. Remaining budget: ₱{runtimeCharacter.currentWeeklyBudget}");

        LevelManager levelManager = Object.FindAnyObjectByType<LevelManager>();
        if (levelManager != null)
        {
            levelManager.UpdateBudgetDisplay();
        }

        return true;
    }

}
