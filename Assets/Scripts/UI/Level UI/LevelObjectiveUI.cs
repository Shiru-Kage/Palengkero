using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class LevelObjectiveUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI nutritionGoalText;
    [SerializeField] private TextMeshProUGUI satisfactionGoalText;
    [SerializeField] private TextMeshProUGUI savingsGoalText;
    [SerializeField] private TextMeshProUGUI levelModifiers;

    [Header("UI Image References")]
    [SerializeField] private Image nutritionImage;
    [SerializeField] private Image satisfactionImage;
    [SerializeField] private Image savingsImage;
    [SerializeField] private Sprite completionImage;

    [Header("Exit Image (Pulse Effect)")]
    [SerializeField] private Image exitImage;
    [SerializeField] private float pulseDuration = 1f;

    private CharacterObjective currentObjective;
    public CharacterObjective CurrentObjective => currentObjective;

    private int currentNutrition = 0;
    private int currentSatisfaction = 0;

    private Color defaultColor;

    private Sprite originalNutritionSprite;
    private Sprite originalSatisfactionSprite;
    private Sprite originalSavingsSprite;

    private void Awake()
    {
        defaultColor = nutritionGoalText.color;
        originalNutritionSprite = nutritionImage?.sprite ?? default;
        originalSatisfactionSprite = satisfactionImage?.sprite ?? default;
        originalSavingsSprite = savingsImage?.sprite ?? default;

        SetExitImageOpacity(0f);
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

            levelModifiers.text = currentLevel?.levelDescription ?? "Tutorial!";
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
        if (nutritionImage != null)
        {
            if (currentNutrition >= currentObjective.nutritionGoal)
            {
                nutritionGoalText.color = Color.green;
                nutritionGoalText.text = $"Nutrition Goal: {currentObjective.nutritionGoal} Cleared";
                nutritionImage.sprite = completionImage;
            }
            else
            {
                nutritionGoalText.color = defaultColor;
                nutritionGoalText.text = $"Nutrition Goal: {currentObjective.nutritionGoal}";
                nutritionImage.sprite = originalNutritionSprite;
            }
        }

        // Satisfaction
        if (satisfactionImage != null)
        {
            if (currentSatisfaction >= currentObjective.satisfactionGoal)
            {
                satisfactionGoalText.color = Color.green;
                satisfactionGoalText.text = $"Satisfaction Goal: {currentObjective.satisfactionGoal} Cleared";
                satisfactionImage.sprite = completionImage;
            }
            else
            {
                satisfactionGoalText.color = defaultColor;
                satisfactionGoalText.text = $"Satisfaction Goal: {currentObjective.satisfactionGoal}";
                satisfactionImage.sprite = originalSatisfactionSprite;
            }
        }

        // Savings
        var runtimeCharacter = CharacterSelectionManager.Instance?.SelectedRuntimeCharacter;
        if (runtimeCharacter != null && savingsImage != null)
        {
            if (runtimeCharacter.currentWeeklyBudget >= currentObjective.savingsGoal)
            {
                savingsGoalText.color = Color.green;
                savingsGoalText.text = $"Savings Goal: ₱{currentObjective.savingsGoal} Cleared";
                savingsImage.sprite = completionImage;
            }
            else
            {
                savingsGoalText.color = defaultColor;
                savingsGoalText.text = $"Savings Goal: ₱{currentObjective.savingsGoal}";
                savingsImage.sprite = originalSavingsSprite;
            }
        }

        if (currentNutrition >= currentObjective.nutritionGoal && currentSatisfaction >= currentObjective.satisfactionGoal && runtimeCharacter != null && runtimeCharacter.currentWeeklyBudget >= currentObjective.savingsGoal)
        {
            if (exitImage.color.a == 0f)
            {
                StartCoroutine(PulseExitImage());
            }
        }
        else
        {
            StopCoroutine(PulseExitImage());
            SetExitImageOpacity(0f);
        }
    }

    private IEnumerator PulseExitImage()
    {
        float time = 0;
        Color initialColor = exitImage.color;
        while (true)
        {
            time += Time.deltaTime;
            float alpha = Mathf.PingPong(time / pulseDuration, 1f);
            SetExitImageOpacity(alpha);
            yield return null;
        }
    }

    private void SetExitImageOpacity(float alpha)
    {
        if (exitImage != null) // Null check before assignment
        {
            Color currentColor = exitImage.color;
            exitImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
        }
    }
}
