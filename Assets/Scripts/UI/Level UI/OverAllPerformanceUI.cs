using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class OverAllPerformanceUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI performanceSummaryText;  // Total stars display
    [SerializeField] private TextMeshProUGUI noticeableHabitText;  // Display the most noticeable habit
    [SerializeField] private TextMeshProUGUI endingText;  // Display the ending
    [SerializeField] private Endings endingsScript;  // Reference to the Endings script
    
    [SerializeField] private Image nutritionBar;  // Nutrition Bar
    [SerializeField] private Image satisfactionBar;  // Satisfaction Bar
    [SerializeField] private Image savingsBar;  // Savings Bar

    private string selectedCharacterID;
    private int totalLevels = 5;  // Total number of levels in the game
    private int maxStarsPerCategory = 5;  // Maximum stars per category (5 levels × 1 star per level)

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

        for (int i = 0; i < totalLevels; i++)
        {
            var levelStars = StarSystem.Instance.GetStarsForLevel(i, selectedCharacterID);

            nutritionStars[i] = levelStars.nutritionStars > 0;
            satisfactionStars[i] = levelStars.satisfactionStars > 0;
            savingsStars[i] = levelStars.savingsStars > 0;
        }

        performanceSummaryText.text = $"Total Stars: {totalStars}/15";  // 15 is the total stars possible across all levels for each category

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

        // Calculate total stars for each category across all levels
        for (int i = 0; i < totalLevels; i++)
        {
            var levelStars = StarSystem.Instance.GetStarsForLevel(i, selectedCharacterID);

            totalNutritionStars += levelStars.nutritionStars;
            totalSatisfactionStars += levelStars.satisfactionStars;
            totalSavingsStars += levelStars.savingsStars;
        }

        // Calculate the overall percentage for each category (0% to 100%)
        float nutritionPercentage = CalculatePercentage(totalNutritionStars);
        float satisfactionPercentage = CalculatePercentage(totalSatisfactionStars);
        float savingsPercentage = CalculatePercentage(totalSavingsStars);

        // Update the bars based on the overall percentage
        SetBar(nutritionBar, nutritionPercentage);
        SetBar(satisfactionBar, satisfactionPercentage);
        SetBar(savingsBar, savingsPercentage);
    }

    // Method to calculate percentage for bar fill (based on 5 stars max per category)
    float CalculatePercentage(float totalStars)
    {
        float percentage = (totalStars / maxStarsPerCategory) * 100f;  // Calculate percentage out of 100
        return Mathf.Clamp(percentage, 0f, 100f);  // Ensure it's between 0 and 100
    }

    // Method to set the fill amount for the bar
    void SetBar(Image bar, float percentage)
    {
        // Map the percentage to the 0-1 range for fillAmount
        bar.fillAmount = Mathf.Clamp(percentage / 100f, 0f, 1f);  // Ensure fillAmount stays between 0 and 1
    }
}