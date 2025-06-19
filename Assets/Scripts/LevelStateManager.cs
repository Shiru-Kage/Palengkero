using UnityEngine;

public class LevelStateManager : MonoBehaviour
{
    public static LevelStateManager Instance { get; private set; }

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
        Debug.Log("SetLevelIndex called! New index = " + index);
    }
}
