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
            string characterID = CharacterSelectionManager.Instance.SelectedCharacterID;
            starUI.UpdateStarsForSelectedLevel(levelIndex, characterID);
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
