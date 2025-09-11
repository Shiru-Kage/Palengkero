using UnityEngine;
using TMPro;

public class LevelObjectiveUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI nutritionGoalText;
    [SerializeField] private TextMeshProUGUI satisfactionGoalText;
    [SerializeField] private TextMeshProUGUI savingsGoalText;
    [SerializeField] private TextMeshProUGUI levelModifiers;

    private CharacterObjective currentObjective;
    public CharacterObjective CurrentObjective => currentObjective;

    private int currentNutrition = 0;
    private int currentSatisfaction = 0;

    private Color defaultColor;

    private void Awake()
    {
        defaultColor = nutritionGoalText.color;
    }

    private void OnEnable()
    {
        WellBeingEvents.OnWellBeingChanged += HandleWellBeingChanged;
    }

    private void OnDisable()
    {
        WellBeingEvents.OnWellBeingChanged -= HandleWellBeingChanged;
    }

    public void UpdateObjectiveUI(CharacterObjective objective, LevelData currentLevel)
    {
        currentObjective = objective;
    

        if (objective != null)
        {
            nutritionGoalText.text = $"Nutrition Goal: {objective.nutritionGoal}";
            satisfactionGoalText.text = $"Satisfaction Goal: {objective.satisfactionGoal}";
            savingsGoalText.text = $"Savings Goal: ₱{objective.savingsGoal}";
            levelModifiers.text = $"Level description: \n{currentLevel.levelDescription}";
        }
        else
        {
            nutritionGoalText.text = "Nutrition Goal: N/A";
            satisfactionGoalText.text = "Satisfaction Goal: N/A";
            savingsGoalText.text = "Savings Goal: N/A";
        }
    }

    private void HandleWellBeingChanged(int nutritionDelta, int satisfactionDelta)
    {
        currentNutrition += nutritionDelta;
        currentSatisfaction += satisfactionDelta;

        if (currentObjective == null) return;

        // Nutrition
        if (currentNutrition >= currentObjective.nutritionGoal)
        {
            nutritionGoalText.color = Color.green;
            nutritionGoalText.text = $"Nutrition Goal: {currentObjective.nutritionGoal} Cleared";
        }
        else
        {
            nutritionGoalText.color = defaultColor;
            nutritionGoalText.text = $"Nutrition Goal: {currentObjective.nutritionGoal}";
        }

        // Satisfaction
        if (currentSatisfaction >= currentObjective.satisfactionGoal)
        {
            satisfactionGoalText.color = Color.green;
            satisfactionGoalText.text = $"Satisfaction Goal: {currentObjective.satisfactionGoal} Cleared";
        }
        else
        {
            satisfactionGoalText.color = defaultColor;
            satisfactionGoalText.text = $"Satisfaction Goal: {currentObjective.satisfactionGoal}";
        }

        // Savings
        var runtimeCharacter = CharacterSelectionManager.Instance?.SelectedRuntimeCharacter;
        if (runtimeCharacter != null)
        {
            if (runtimeCharacter.currentWeeklyBudget >= currentObjective.savingsGoal)
            {
                savingsGoalText.color = Color.green;
                savingsGoalText.text = $"Savings Goal: ₱{currentObjective.savingsGoal} Cleared";
            }
            else
            {
                savingsGoalText.color = defaultColor;
                savingsGoalText.text = $"Savings Goal: ₱{currentObjective.savingsGoal}";
            }
        }
    }
}
