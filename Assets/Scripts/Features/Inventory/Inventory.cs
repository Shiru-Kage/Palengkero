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
        DontDestroyOnLoad(gameObject);
    }

    public void ClearInventory()
    {
        items.Clear();
        OnInventoryUpdated?.Invoke();
    }

    public void AddItem(ItemData itemData, int quantity)
    {
        InventoryItem existingItem = items.Find(i => i.itemData.id == itemData.id);
        if (existingItem != null)
        {
            existingItem.AddQuantity(quantity);
        }
        else
        {
            items.Add(new InventoryItem(itemData, quantity));
        }

        OnInventoryUpdated?.Invoke();
    }

    public bool RemoveItem(ItemData itemData, int quantity)
    {
        InventoryItem existingItem = items.Find(i => i.itemData.id == itemData.id);
        if (existingItem != null)
        {
            bool result = existingItem.RemoveQuantity(quantity);
            if (result)
            {
                OnInventoryUpdated?.Invoke();
            }
            return result;
        }
        return false;
    }

    public bool HasItem(ItemData itemData)
    {
        return items.Exists(i => i.itemData.id == itemData.id);
    }

    public int GetItemQuantity(ItemData itemData)
    {
        InventoryItem item = items.Find(i => i.itemData.id == itemData.id);
        return item != null ? item.quantity : 0;
    }

    public List<InventoryItem> GetAllItems()
    {
        return new List<InventoryItem>(items);
    }
}
