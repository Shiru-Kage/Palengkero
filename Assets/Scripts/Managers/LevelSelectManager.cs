using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelSelectManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image levelSprite;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI levelDescriptionText;

    [Header("Objective Texts")]
    [SerializeField] private TextMeshProUGUI nutritionGoalText;
    [SerializeField] private TextMeshProUGUI satisfactionGoalText;
    [SerializeField] private TextMeshProUGUI savingsGoalText;

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

        // Already updated by SetLevel, no need to call UpdateUI again here
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
    }

    private void UpdateUI(LevelData currentLevel)
    {
        if (CharacterSelectionManager.Instance == null || CharacterSelectionManager.Instance.SelectedCharacterData == null)
        {
            Debug.LogError("No selected character found.");
            return;
        }

        CharacterData selectedCharacter = CharacterSelectionManager.Instance.SelectedCharacterData;
        CharacterObjective objective = currentLevel.GetObjectiveFor(selectedCharacter);

        if (levelText != null)
            levelText.text = "Level " + currentLevel.levelNumber;

        if (levelDescriptionText != null)
            levelDescriptionText.text = currentLevel.levelDescription ?? "No description available.";

        if (levelSprite != null)
            levelSprite.sprite = currentLevel.levelIcon;

        if (objective != null)
        {
            nutritionGoalText.text = $"Nutrition Goal: {objective.nutritionGoal}";
            satisfactionGoalText.text = $"Satisfaction Goal: {objective.satisfactionGoal}";
            savingsGoalText.text = $"Savings Goal: ₱{objective.savingsGoal}";
        }
        else
        {
            nutritionGoalText.text = "Nutrition Goal: N/A";
            satisfactionGoalText.text = "Satisfaction Goal: N/A";
            savingsGoalText.text = "Savings Goal: N/A";
        }
    }
}
