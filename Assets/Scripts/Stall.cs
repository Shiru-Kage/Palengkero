using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Stall : Interactable
{
    [Header("Stall UI")]
    [SerializeField] private GameObject stallUI;

    [Header("Manual Setup (optional)")]
    public bool useRandomItem = true;
    public string specificItemId; // If useRandomItem is false
    public int customStock = -1;  // -1 = random stock

    [Header("UI References")]
    public Image itemIcon;

    private ItemData assignedItem;
    private int stock;

    private void Start()
    {
        AssignItem();
        DisplayItem();
    }

    private void AssignItem()
    {
        if (ItemDatabaseManager.Instance == null || ItemDatabaseManager.Instance.itemDatabase == null)
        {
            Debug.LogError("ItemDatabaseManager not ready!");
            return;
        }

        if (useRandomItem)
        {
            var items = ItemDatabaseManager.Instance.itemDatabase.items;
            assignedItem = items[Random.Range(0, items.Count)];
        }
        else
        {
            assignedItem = ItemDatabaseManager.Instance.GetItem(specificItemId);
        }

        if (assignedItem == null)
        {
            Debug.LogWarning($"Item not found for {gameObject.name}");
            return;
        }

        stock = (customStock >= 0) ? customStock : Random.Range(1, assignedItem.stockLimit + 1);
    }

    private void DisplayItem()
    {
        if (itemIcon != null && assignedItem != null)
        {
            // Assume itemIcon is stored in ItemData, add sprite reference to it
            itemIcon.sprite = assignedItem.icon; // Add 'public Sprite icon;' to ItemData
        }
    }

    // Optional getter
    public (ItemData, int) GetItemAndStock() => (assignedItem, stock);

    public override void Interact()
    {
        base.Interact();
        if (stallUI != null)
            stallUI.SetActive(true);
    }
}
