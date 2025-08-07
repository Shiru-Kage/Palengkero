using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class LevelStateManager : MonoBehaviour
{
    public static LevelStateManager Instance { get; private set; }

    [Header("All Level Data")]
    [SerializeField] private LevelData[] allLevels;
    public LevelData[] AllLevels => allLevels;
    private Dictionary<string, bool[]> characterLevelLocks = new Dictionary<string, bool[]>();

    public int CurrentLevelIndex { get; private set; } = 0;

    private string currentCharacterName;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetSelectedCharacter(string characterName)
    {
        currentCharacterName = characterName;

        if (!characterLevelLocks.ContainsKey(characterName))
        {
            bool[] levelLocks = new bool[allLevels.Length];
            levelLocks[0] = true;
            characterLevelLocks[characterName] = levelLocks;
        }
    }

    public void SetLevelIndex(int index)
    {
        if (index >= 0 && index < allLevels.Length)
        {
            CurrentLevelIndex = index;
        }
    }

    public bool IsLevelUnlocked(int levelIndex)
    {
        if (!characterLevelLocks.ContainsKey(currentCharacterName) || levelIndex < 0 || levelIndex >= allLevels.Length)
        {
            return false;
        }

        return characterLevelLocks[currentCharacterName][levelIndex];
    }

    public void UnlockNextLevel()
    {
        if (CurrentLevelIndex + 1 < characterLevelLocks[currentCharacterName].Length)
        {
            characterLevelLocks[currentCharacterName][CurrentLevelIndex + 1] = true;
        }
    }

    public LevelData GetCurrentLevelData()
    {
        if (allLevels == null || CurrentLevelIndex < 0 || CurrentLevelIndex >= allLevels.Length)
        {
            Debug.LogError("Invalid current level index or missing level data.");
            return null;
        }

        return allLevels[CurrentLevelIndex];
    }

    public bool[] GetUnlockedLevelsForCurrentCharacter()
    {
        if (characterLevelLocks.TryGetValue(currentCharacterName, out var levels))
        {
            return (bool[])levels.Clone();
        }

        return new bool[allLevels.Length];
    }

    public void SetUnlockedLevelsForCurrentCharacter(bool[] levels)
    {
        if (string.IsNullOrEmpty(currentCharacterName)) return;

        if (characterLevelLocks.ContainsKey(currentCharacterName))
        {
            characterLevelLocks[currentCharacterName] = levels;
        }
        else
        {
            characterLevelLocks.Add(currentCharacterName, levels);
        }
    }

    // ✅ Reset level data for all characters
    public void ResetAllCharacterLevelData()
    {
        foreach (var characterName in new List<string>(characterLevelLocks.Keys))  // Use a copy of keys to avoid modification issues
        {
            bool[] resetLevels = new bool[allLevels.Length];
            resetLevels[0] = true;  // Keep the first level unlocked
            SetUnlockedLevelsForCurrentCharacterForCharacter(characterName, resetLevels);
            SetLevelIndex(0);  // Reset level index to 0 for all characters
        }
    }

    private void SetUnlockedLevelsForCurrentCharacterForCharacter(string characterName, bool[] levels)
    {
        if (string.IsNullOrEmpty(characterName)) return;

        if (characterLevelLocks.ContainsKey(characterName))
        {
            characterLevelLocks[characterName] = levels;
        }
        else
        {
            characterLevelLocks.Add(characterName, levels);
        }
    }
}
