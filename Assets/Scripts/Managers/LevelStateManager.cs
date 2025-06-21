using UnityEngine;

public class LevelStateManager : MonoBehaviour
{
    public static LevelStateManager Instance { get; private set; }

    [Header("All Level Data")]
    [SerializeField] private LevelData[] allLevels;
    public LevelData[] AllLevels => allLevels;

    public int CurrentLevelIndex { get; private set; } = 0;

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

    public void SetLevelIndex(int index)
    {
        CurrentLevelIndex = index;
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
