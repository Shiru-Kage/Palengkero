using UnityEngine;
using System.Collections.Generic;

public class ArchiveManager : MonoBehaviour
{
    public static ArchiveManager Instance { get; private set; }

    // Dictionary: characterName -> HashSet of unlocked cutscene names
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
    }

    /// <summary>
    /// Unlocks a cutscene for a character.
    /// </summary>
    public void UnlockCutscene(string characterName, string cutsceneName)
    {
        if (!unlockedCutscenes.ContainsKey(characterName))
        {
            unlockedCutscenes[characterName] = new HashSet<string>();
        }

        if (unlockedCutscenes[characterName].Add(cutsceneName))
        {
            Debug.Log($"[ArchiveManager] Unlocked {cutsceneName} for {characterName}.");
            // TODO: Save system hook here
        }
    }

    /// <summary>
    /// Checks if a cutscene is unlocked.
    /// </summary>
    public bool IsCutsceneUnlocked(string characterName, string cutsceneName)
    {
        return unlockedCutscenes.ContainsKey(characterName) && unlockedCutscenes[characterName].Contains(cutsceneName);
    }
}
