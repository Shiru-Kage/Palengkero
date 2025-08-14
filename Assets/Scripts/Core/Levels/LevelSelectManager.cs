using UnityEngine;

public class LevelSelectManager : MonoBehaviour
{
    private StarUI starUI;
    private void Start()
    {
        starUI = Object.FindAnyObjectByType<StarUI>();
        if (LevelStateManager.Instance != null)
        {
            SetLevel(LevelStateManager.Instance.CurrentLevelIndex);
        }
        else
        {
            Debug.LogWarning("LevelStateManager not found. Defaulting to index 0.");
            SetLevel(0);
        }
    }

    public void SetLevel(int levelIndex)
    {
        LevelData[] levels = LevelStateManager.Instance?.AllLevels;

        if (levels == null || levels.Length == 0)
        {
            Debug.LogError("No level data available from LevelStateManager.");
            return;
        }

        if (levelIndex < 0 || levelIndex >= levels.Length)
        {
            Debug.LogError("Invalid level index: " + levelIndex);
            return;
        }

        LevelStateManager.Instance.SetLevelIndex(levelIndex);
        UpdateUI(levels[levelIndex]);

        if (starUI != null)
        {
            starUI.UpdateStarsForSelectedLevel(levelIndex);
        }
        else
        {
            Debug.LogWarning("StarUI component not found!");
        }
    }

    private void UpdateUI(LevelData currentLevel)
    {
        LevelSelectUI.Instance.UpdateLevelUI(currentLevel);
    }
}
