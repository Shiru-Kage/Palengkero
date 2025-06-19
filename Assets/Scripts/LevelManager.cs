using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelManager : MonoBehaviour
{
    [Header("All Level Data")]
    [SerializeField] private LevelData[] levels;

    [Header("UI References")]
    [SerializeField] private Image levelSprite;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI levelDescriptionText;
    [SerializeField] private TextMeshProUGUI objectivesPerLevel;

    void Start()
    {
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
        if (levels == null || levels.Length == 0)
        {
            Debug.LogError("No level data assigned.");
            return;
        }

        if (levelIndex < 0 || levelIndex >= levels.Length)
        {
            Debug.LogError("Invalid level index: " + levelIndex);
            return;
        }
        if (LevelStateManager.Instance != null)
        {
            LevelStateManager.Instance.SetLevelIndex(levelIndex);
        }
        UpdateUI(levelIndex);
    }

    private void UpdateUI(int levelIndex)
    {
        LevelData currentLevel = levels[levelIndex];

        if (CharacterSelectionManager.Instance == null || CharacterSelectionManager.Instance.SelectedCharacterData == null)
        {
            Debug.LogError("No selected character found.");
            return;
        }

        CharacterData selectedCharacter = CharacterSelectionManager.Instance.SelectedCharacterData;
        CharacterObjective objective = currentLevel.GetObjectiveFor(selectedCharacter);

        levelText.text = "Level " + currentLevel.levelNumber;
        levelDescriptionText.text = currentLevel.levelDescription;

        if (levelSprite != null && currentLevel.levelIcon != null)
        {
            levelSprite.sprite = currentLevel.levelIcon;
        }

        if (objective != null)
        {
            objectivesPerLevel.text = $"Objectives:\n" +
                                      $"- Nutrition: TBD\n" +
                                      $"- Satisfaction: TBD\n" +
                                      $"- Savings Goal: {objective.savingsGoal}";
        }
        else
        {
            objectivesPerLevel.text = $"No objective set for {selectedCharacter.characterName}.";
        }
    }
}
