using UnityEngine;
using System.Collections.Generic;

public class ArchiveManager : MonoBehaviour
{
    public static ArchiveManager Instance { get; private set; }
    private Dictionary<string, HashSet<string>> unlockedCutscenes = new Dictionary<string, HashSet<string>>();

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

    public bool IsCutsceneUnlocked(string characterName, string cutsceneName)
    {
        return unlockedCutscenes.ContainsKey(characterName) && unlockedCutscenes[characterName].Contains(cutsceneName);
    }

    private void SaveToFile()
    {
        var saveData = new ArchiveSaveData();

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

        Debug.Log($"[ArchiveManager] Loaded {saveData.unlockedArchives.Count} archive entries.");
    }
}
