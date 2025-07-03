using UnityEngine;
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
}
