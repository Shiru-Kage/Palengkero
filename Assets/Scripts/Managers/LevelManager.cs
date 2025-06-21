using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI weeklyBudgetText;
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
        CharacterSelectionManager manager = CharacterSelectionManager.Instance;

        if (manager == null || manager.SelectedCharacterData == null || manager.SelectedRuntimeCharacter == null)
        {
            Debug.LogError("No selected character found.");
            return;
        }

        CharacterData selectedCharacter = manager.SelectedCharacterData;
        RuntimeCharacter runtimeCharacter = manager.SelectedRuntimeCharacter;

        CharacterObjective objective = currentLevel.GetObjectiveFor(selectedCharacter);
        weeklyBudgetText.text = $"PHP {runtimeCharacter.currentWeeklyBudget}.00";

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


    public void UpdateBudgetDisplay()
    {
        RuntimeCharacter runtimeCharacter = CharacterSelectionManager.Instance?.SelectedRuntimeCharacter;
        if (runtimeCharacter == null)
            return;

        weeklyBudgetText.text = $"PHP {runtimeCharacter.currentWeeklyBudget}.00";
    }
}
