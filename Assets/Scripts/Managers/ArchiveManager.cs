using UnityEngine;
using System.Collections.Generic;

public class ArchiveManager : MonoBehaviour
{
    public static ArchiveManager Instance { get; private set; }
    private Dictionary<string, HashSet<string>> unlockedCutscenes = new Dictionary<string, HashSet<string>>();
    private Dictionary<string, InventoryItem> purchasedItems  = new Dictionary<string, InventoryItem>();
    private HashSet<string> unlockedItems = new HashSet<string>();
    private HashSet<string> viewedCutscenes = new HashSet<string>();

    private ArchiveItems archiveItems;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadFromFile();
    }

    public void UnlockCutscene(string characterName, string cutsceneName)
    {
        if (!unlockedCutscenes.ContainsKey(characterName))
        {
            unlockedCutscenes[characterName] = new HashSet<string>();
        }

        if (unlockedCutscenes[characterName].Add(cutsceneName))
        {
            Debug.Log($"[ArchiveManager] Unlocked {cutsceneName} for {characterName}.");
            SaveToFile();
        }
    }
    public void UnlockItem(string itemId)
    {
        if (!unlockedItems.Contains(itemId))
        {
            unlockedItems.Add(itemId);
            SaveToFile();
        }
    }
    

    public bool IsCutsceneUnlocked(string characterName, string cutsceneName)
    {
        return unlockedCutscenes.ContainsKey(characterName) && unlockedCutscenes[characterName].Contains(cutsceneName);
    }

    private void SaveToFile()
    {
        var saveData = new ArchiveSaveData
        {
            unlockedItems = new List<string>(unlockedItems) 
        };

        foreach (var kvp in unlockedCutscenes)
        {
            saveData.unlockedArchives.Add(new ArchiveEntry
            {
                characterName = kvp.Key,
                cutsceneNames = new List<string>(kvp.Value)
            });
        }

        SaveSystem.SaveArchives(saveData);
    }

    private void LoadFromFile()
    {
        var saveData = SaveSystem.LoadArchives();
        unlockedCutscenes.Clear();

        foreach (var entry in saveData.unlockedArchives)
        {
            unlockedCutscenes[entry.characterName] = new HashSet<string>(entry.cutsceneNames);
        }
        unlockedItems = new HashSet<string>(saveData.unlockedItems);

        foreach (var itemId in unlockedItems)
        {
            var item = ItemDatabaseManager.Instance.GetItem(itemId);
            if (item != null)
            {
                purchasedItems[itemId] = new InventoryItem(item, 1);
            }
        }

        Debug.Log($"[ArchiveManager] Loaded {saveData.unlockedArchives.Count} archive entries.");
    }


    public bool HasViewedCutscene(string cutsceneName)
    {
        return viewedCutscenes.Contains(cutsceneName);
    }

    public void MarkCutsceneAsViewed(string cutsceneName)
    {
        if (!viewedCutscenes.Contains(cutsceneName))
        {
            viewedCutscenes.Add(cutsceneName);
            Debug.Log($"[ArchiveManager] Marked {cutsceneName} as viewed.");
        }
    }

    public void OnItemPurchased(InventoryItem purchasedItem)
    {
        if (!purchasedItems.ContainsKey(purchasedItem.itemData.id))
        {
            purchasedItems.Add(purchasedItem.itemData.id, purchasedItem);
            UnlockItem(purchasedItem.itemData.id);

            if (archiveItems != null)
            {
                archiveItems.UnlockItemInArchive(purchasedItem);
            }

            Debug.Log($"{purchasedItem.itemData.itemName} has been unlocked in the archive.");
        }
    }

    public void SetArchiveItemsReference(ArchiveItems archiveUI)
    {
        archiveItems = archiveUI;
    }

    public bool IsItemUnlocked(string itemId)
    {
        return purchasedItems.ContainsKey(itemId);
    }
}
