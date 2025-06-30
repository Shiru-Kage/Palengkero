using UnityEngine;
using System.Collections.Generic;

public class LevelStateManager : MonoBehaviour
{
    public static LevelStateManager Instance { get; private set; }

    [Header("All Level Data")]
    [SerializeField] private LevelData[] allLevels;
    public LevelData[] AllLevels => allLevels;

    // Dictionary to hold the level locks for each character by name
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

    // Call this method whenever the character changes
    public void SetSelectedCharacter(string characterName)
    {
        currentCharacterName = characterName;

        // If this character's level locks don't exist, initialize them
        if (!characterLevelLocks.ContainsKey(characterName))
        {
            bool[] levelLocks = new bool[allLevels.Length];
            levelLocks[0] = true; // First level is always unlocked
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
        // Return false if the character hasn't been selected or the level index is invalid
        if (!characterLevelLocks.ContainsKey(currentCharacterName) || levelIndex < 0 || levelIndex >= allLevels.Length)
        {
            return false;
        }

        return characterLevelLocks[currentCharacterName][levelIndex];
    }

    public void UnlockNextLevel()
    {
        // Unlock the next level for the selected character
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
}
