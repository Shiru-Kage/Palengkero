using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ItemDatabaseManager : MonoBehaviour {
    public static ItemDatabaseManager Instance { get; private set; }

    private Dictionary<string, ItemData> itemLookup;

    [HideInInspector]
    public ItemDatabase itemDatabase;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            LoadData();
        } else {
            Destroy(gameObject);
        }
    }

    private void LoadData() {
        string path = Path.Combine(Application.streamingAssetsPath, "item_data.json");
        if (File.Exists(path)) {
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
            foreach (var item in itemDatabase.items) {
                itemLookup[item.id] = item;
            }
        } else {
            Debug.LogError("Item data JSON not found!");
        }
    }

    public ItemData GetItem(string id) {
        if (itemLookup.TryGetValue(id, out var item)) {
            return item;
        }
        Debug.LogWarning($"Item with ID '{id}' not found.");
        return null;
    }
}
