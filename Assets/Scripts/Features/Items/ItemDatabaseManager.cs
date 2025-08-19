using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ItemDatabaseManager : MonoBehaviour
{
    public static ItemDatabaseManager Instance { get; private set; }
    public ItemDatabase itemDatabase;  // This stores all the item data

    [Header("Price Adjustments")]
    [SerializeField] private float priceIncreasePercentage = 10f; // Default to 10% increase per level

    private Dictionary<int, bool> levelPriceAdjusted = new Dictionary<int, bool>(); // Track if prices were adjusted for a level
    private Dictionary<string, int> originalPrices = new Dictionary<string, int>();  // Store original prices for items

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

                // Store the original price for each item
                originalPrices[item.id] = item.price;
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
            // Apply the price increase based on the original base price, not the adjusted price
            item.price = Mathf.RoundToInt(originalPrices[item.id] * priceMultiplier);
        }
    }

    // Method to adjust prices based on the level (uses base prices from the JSON file)
    public void AdjustPricesBasedOnLevel(int levelIndex)
    {
        // Check if the prices have already been adjusted for this level
        if (levelPriceAdjusted.ContainsKey(levelIndex) && levelPriceAdjusted[levelIndex])
        {
            Debug.Log("Skipping Price Increase");
            return; // Skip if prices have already been adjusted
        }

        // Cumulative increase based on the level index (e.g., Level 2 = 10%, Level 3 = 20%, etc.)
        if (levelIndex >= 1)
        {
            float priceMultiplier = 1 + (priceIncreasePercentage * levelIndex) / 100f;

            // Apply the price multiplier to each item's base price from the JSON data
            IncreaseItemPrices(priceMultiplier);
        }

        // Mark this level as having had its prices adjusted
        levelPriceAdjusted[levelIndex] = true;
    }

    // Method to get the item by its ID
    public ItemData GetItem(string id)
    {
        // Directly access the item using the items list in itemDatabase
        foreach (var item in itemDatabase.items)
        {
            if (item.id == id)
            {
                return item;
            }
        }

        Debug.LogWarning($"Item with ID '{id}' not found.");
        return null;
    }
}
