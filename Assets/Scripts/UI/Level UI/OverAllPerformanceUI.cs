using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class OverAllPerformanceUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI performanceSummaryText;  
    [SerializeField] private TextMeshProUGUI noticeableHabitText;  
    [SerializeField] private TextMeshProUGUI endingText;
    [SerializeField] private Endings endingsScript;  
    [SerializeField] private TextMeshProUGUI greatJobText;
    
    [SerializeField] private Image nutritionBar;  
    [SerializeField] private Image satisfactionBar; 
    [SerializeField] private Image savingsBar; 

    private string selectedCharacterID;
    private int totalLevels = 5; 
    private int maxStarsPerCategory = 5;

    void Start()
    {
        selectedCharacterID = CharacterSelectionManager.Instance.SelectedCharacterID;
        UpdatePerformanceUI();
        DisplayEnding();
        UpdateBarChart();
    }

    void UpdatePerformanceUI()
    {
        int totalStars = StarSystem.Instance.GetTotalStarsForCharacter(selectedCharacterID);

        int totalLevels = LevelStateManager.Instance.AllLevels.Length;
        bool[] nutritionStars = new bool[totalLevels];
        bool[] satisfactionStars = new bool[totalLevels];
        bool[] savingsStars = new bool[totalLevels];

        float totalSavings = 0f;

        for (int i = 0; i < totalLevels; i++)
        {
            var levelStars = StarSystem.Instance.GetStarsForLevel(i, selectedCharacterID);

            nutritionStars[i] = levelStars.nutritionStars > 0;
            satisfactionStars[i] = levelStars.satisfactionStars > 0;
            savingsStars[i] = levelStars.savingsStars > 0;

            var levelData = LevelStateManager.Instance.AllLevels[i];
            var characterObjective = levelData.GetObjectiveFor(CharacterSelectionManager.Instance.SelectedCharacterData);
            totalSavings += characterObjective.levelSavings;
        }

        performanceSummaryText.text = $"Total Stars: {totalStars}/15";

        int characterTotalBudget = CharacterSelectionManager.Instance.SelectedCharacterData.characterTotalBudget;
        float targetSavings = characterTotalBudget * 0.5f;

        if (totalSavings >= targetSavings)
        {
            greatJobText.text = $"Great Job! You saved more than 50% of your total budget! ₱{totalSavings:F2}/₱{characterTotalBudget}";
            greatJobText.color = Color.green;
        }
        else
        {
            greatJobText.text = $"You did not seem to have saved enough, you can try better next time! ₱{totalSavings:F2}/₱{characterTotalBudget} "; 
        }


        string noticeableHabit = GetMostNoticedHabit(nutritionStars, satisfactionStars, savingsStars);
        noticeableHabitText.text = $"Most Noticeable Habit: {noticeableHabit}";
    }

    void DisplayEnding()
    {
        endingText.text = "Ending: " + endingsScript.GetEndingName();
    }

    string GetMostNoticedHabit(bool[] nutritionStars, bool[] satisfactionStars, bool[] savingsStars)
    {
        int nutritionCount = nutritionStars.Count(star => star);
        int satisfactionCount = satisfactionStars.Count(star => star);
        int savingsCount = savingsStars.Count(star => star);

        if (nutritionCount == nutritionStars.Length)
        {
            return "Excellent Nutrition Habit";
        }
        else if (satisfactionCount == satisfactionStars.Length)
        {
            return "High Satisfaction";
        }
        else if (savingsCount == savingsStars.Length)
        {
            return "Strong Savings Habit";
        }
        else
        {
            return "Balanced Performance";
        }
    }

    void UpdateBarChart()
    {
        float totalNutritionStars = 0f;
        float totalSatisfactionStars = 0f;
        float totalSavingsStars = 0f;

        for (int i = 0; i < totalLevels; i++)
        {
            var levelStars = StarSystem.Instance.GetStarsForLevel(i, selectedCharacterID);

            totalNutritionStars += levelStars.nutritionStars;
            totalSatisfactionStars += levelStars.satisfactionStars;
            totalSavingsStars += levelStars.savingsStars;
        }

        float nutritionPercentage = CalculatePercentage(totalNutritionStars);
        float satisfactionPercentage = CalculatePercentage(totalSatisfactionStars);
        float savingsPercentage = CalculatePercentage(totalSavingsStars);

        SetBar(nutritionBar, nutritionPercentage);
        SetBar(satisfactionBar, satisfactionPercentage);
        SetBar(savingsBar, savingsPercentage);
    }

    float CalculatePercentage(float totalStars)
    {
        float percentage = (totalStars / maxStarsPerCategory) * 100f; 
        return Mathf.Clamp(percentage, 0f, 100f); 
    }

    void SetBar(Image bar, float percentage)
    {
        bar.fillAmount = Mathf.Clamp(percentage / 100f, 0f, 1f);
    }
}