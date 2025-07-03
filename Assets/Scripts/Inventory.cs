using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private List<InventoryItem> items;
    public static Inventory Instance { get; private set; }
    public delegate void InventoryUpdated();
    public event InventoryUpdated OnInventoryUpdated;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            items = new List<InventoryItem>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Add item to inventory
    public void AddItem(ItemData itemData, int quantity)
    {
        InventoryItem existingItem = items.Find(i => i.itemData.id == itemData.id);
        if (existingItem != null)
        {
            existingItem.AddQuantity(quantity); // Increase quantity if item already exists
        }
        else
        {
            items.Add(new InventoryItem(itemData, quantity)); // Add new item
        }

        OnInventoryUpdated?.Invoke();
    }

    // Remove item from inventory
    public bool RemoveItem(ItemData itemData, int quantity)
    {
        InventoryItem existingItem = items.Find(i => i.itemData.id == itemData.id);
        if (existingItem != null)
        {
            bool result = existingItem.RemoveQuantity(quantity);
            if (result)
            {
                // Notify UI to update
                OnInventoryUpdated?.Invoke();
            }
            return result;
        }
        return false;
    }

    // Check if an item is in the inventory
    public bool HasItem(ItemData itemData)
    {
        return items.Exists(i => i.itemData.id == itemData.id);
    }

    // Get the quantity of a specific item
    public int GetItemQuantity(ItemData itemData)
    {
        InventoryItem item = items.Find(i => i.itemData.id == itemData.id);
        return item != null ? item.quantity : 0;
    }

    // Get all items in the inventory (for UI purposes)
    public List<InventoryItem> GetAllItems()
    {
        return new List<InventoryItem>(items);
    }
}
