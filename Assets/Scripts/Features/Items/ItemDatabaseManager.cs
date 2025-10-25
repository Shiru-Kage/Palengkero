using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ItemDatabaseManager : MonoBehaviour
{
    public static ItemDatabaseManager Instance { get; private set; }
    public ItemDatabase itemDatabase;

    [Header("Price Adjustments")]
    [SerializeField] private float priceIncreasePercentage = 10f; 

    private Dictionary<int, bool> levelPriceAdjusted = new Dictionary<int, bool>(); 
    private Dictionary<string, int> originalPrices = new Dictionary<string, int>();  

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
                item.icon = Resources.Load<Sprite>($"ResourceSprites/Items/{item.iconName}");

                if (item.icon == null)
                {
                    Debug.LogWarning($"Missing icon for item: {item.id} (iconName: {item.iconName})");
                }

                originalPrices[item.id] = item.price;
            }
        }
        else
        {
            Debug.LogError("Item data JSON not found!");
        }
    }

    public void IncreaseItemPrices(float priceMultiplier)
    {
        foreach (var item in itemDatabase.items)
        {
            item.price = Mathf.RoundToInt(originalPrices[item.id] * priceMultiplier);
        }
    }

    public void AdjustPricesBasedOnLevel(int levelIndex)
    {
        if (levelPriceAdjusted.ContainsKey(levelIndex) && levelPriceAdjusted[levelIndex])
        {
            Debug.Log("Skipping Price Increase");
            return;
        }

        if (levelIndex >= 2)
        {
            float priceMultiplier = 1 + (priceIncreasePercentage / 100f)* (levelIndex - 1); 
            IncreaseItemPrices(priceMultiplier);
        }

        levelPriceAdjusted[levelIndex] = true;
    }

    public ItemData GetItem(string id)
    {
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
