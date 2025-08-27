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
    private Dictionary<string, float[]> characterLevelTimes = new Dictionary<string, float[]>();
    private Dictionary<string, int> characterMaxEverUnlockedIndex = new Dictionary<string, int>();

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

        if (!characterLevelTimes.ContainsKey(characterName))
        {
            float[] levelTimes = new float[allLevels.Length];
            characterLevelTimes[characterName] = levelTimes;
        }

        if (!characterMaxEverUnlockedIndex.ContainsKey(characterName))
            characterMaxEverUnlockedIndex[characterName] = 0;
        else
            characterMaxEverUnlockedIndex[characterName] = Mathf.Max(characterMaxEverUnlockedIndex[characterName], 0);
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
            MarkEverUnlockedForCurrentCharacter(CurrentLevelIndex + 1);
        }
    }

    public void LockCurrentLevel()
    {
        if (string.IsNullOrEmpty(currentCharacterName)) return;

        if (CurrentLevelIndex >= 0 && CurrentLevelIndex < characterLevelLocks[currentCharacterName].Length)
        {
            characterLevelLocks[currentCharacterName][CurrentLevelIndex] = false;
            Debug.Log($"[LevelStateManager] Locked current level {CurrentLevelIndex} for {currentCharacterName}.");
        }
        else
        {
            Debug.LogWarning("[LevelStateManager] Invalid CurrentLevelIndex, cannot lock current level.");
        }
    }

    public void LockPreviousLevel()
    {
        if (string.IsNullOrEmpty(currentCharacterName)) return;

        if (CurrentLevelIndex - 1 >= 0 && CurrentLevelIndex - 1 < characterLevelLocks[currentCharacterName].Length)
        {
            characterLevelLocks[currentCharacterName][CurrentLevelIndex - 1] = false;
            Debug.Log($"[LevelStateManager] Locked level {CurrentLevelIndex - 1} for {currentCharacterName}.");
        }
        else
        {
            Debug.LogWarning("[LevelStateManager] No previous level to lock.");
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

    public void ResetAllCharacterLevelData()
    {
        foreach (var characterName in new List<string>(characterLevelLocks.Keys))
        {
            bool[] resetLevels = new bool[allLevels.Length];
            resetLevels[0] = true;
            SetUnlockedLevelsForCurrentCharacterForCharacter(characterName, resetLevels);
            SetLevelIndex(0);

            characterMaxEverUnlockedIndex[characterName] = 0;
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

    public void SaveLevelTime(float timeSpent)
    {
        if (string.IsNullOrEmpty(currentCharacterName)) return;

        if (!characterLevelTimes.ContainsKey(currentCharacterName))
        {
            characterLevelTimes[currentCharacterName] = new float[allLevels.Length];
        }

        characterLevelTimes[currentCharacterName][CurrentLevelIndex] = timeSpent;
    }

    public float GetLevelTime(int levelIndex)
    {
        if (string.IsNullOrEmpty(currentCharacterName)) return 0f;

        if (characterLevelTimes.TryGetValue(currentCharacterName, out var times) &&
            levelIndex >= 0 && levelIndex < times.Length)
        {
            return times[levelIndex];
        }

        return 0f;
    }

    public void MarkEverUnlockedForCurrentCharacter(int levelIndex)
    {
        if (string.IsNullOrEmpty(currentCharacterName)) return;
        if (!characterMaxEverUnlockedIndex.ContainsKey(currentCharacterName))
            characterMaxEverUnlockedIndex[currentCharacterName] = 0;

        if (levelIndex > characterMaxEverUnlockedIndex[currentCharacterName])
            characterMaxEverUnlockedIndex[currentCharacterName] = levelIndex;
    }

    public int GetMaxEverUnlockedLevelIndexForCurrentCharacter()
    {
        if (string.IsNullOrEmpty(currentCharacterName)) return 0;
        return characterMaxEverUnlockedIndex.TryGetValue(currentCharacterName, out var idx) ? idx : 0;
    }
    
    public bool[] GetEverUnlockedButtonsForCurrentCharacter()
    {
        int max = GetMaxEverUnlockedLevelIndexForCurrentCharacter();
        var arr = new bool[allLevels.Length];
        for (int i = 0; i < arr.Length; i++) arr[i] = (i <= max);
        return arr;
    }
}
