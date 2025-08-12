using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ItemDatabaseManager : MonoBehaviour
{
    public static ItemDatabaseManager Instance { get; private set; }
    private Dictionary<string, ItemData> itemLookup;

    [HideInInspector]
    public ItemDatabase itemDatabase;

    [Header("Price Adjustments")]
    [SerializeField] private float priceIncreasePercentage = 10f; // Default to 10% increase per level

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            LoadData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadData()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "item_data.json");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            itemDatabase = JsonUtility.FromJson<ItemDatabase>(json);
            
            foreach (var item in itemDatabase.items)
            {
                item.icon = Resources.Load<Sprite>($"Sprites/Items/{item.iconName}");

                if (item.icon == null)
                {
                    Debug.LogWarning($"Missing icon for item: {item.id} (iconName: {item.iconName})");
                }
            }
            
            // Build lookup dictionary
            itemLookup = new Dictionary<string, ItemData>();
            foreach (var item in itemDatabase.items)
            {
                itemLookup[item.id] = item;
            }
        }
        else
        {
            Debug.LogError("Item data JSON not found!");
        }
    }

    // Method to increase all item prices by a given percentage multiplier
    public void IncreaseItemPrices(float priceMultiplier)
    {
        foreach (var item in itemDatabase.items)
        {
            // Apply the price increase based on the multiplier
            item.price = Mathf.RoundToInt(item.price * priceMultiplier);
        }
    }

    // Method to adjust prices based on the level (uses base prices from the JSON file)
    public void AdjustPricesBasedOnLevel(int levelIndex)
    {
        // Cumulative increase based on the level index (e.g., Level 2 = 10%, Level 3 = 20%, etc.)
        float priceMultiplier = 1 + (priceIncreasePercentage * levelIndex) / 100f;

        // Apply the price multiplier to each item's base price from the JSON data
        IncreaseItemPrices(priceMultiplier);
    }

    // Method to get the item by its ID
    public ItemData GetItem(string id)
    {
        if (itemLookup.TryGetValue(id, out var item))
        {
            return item;
        }
        Debug.LogWarning($"Item with ID '{id}' not found.");
        return null;
    }
}
