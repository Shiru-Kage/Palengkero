using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class OverAllPerformanceUI : MonoBehaviour
{
    // UI Elements to display performance
    [Header("UI Elements")]
    public TextMeshProUGUI performanceSummaryText;
    public Image nutritionBar;
    public Image satisfactionBar;
    public Image savingsBar;
    public Image nutritionPieSection;  // Pie Chart section for Nutrition
    public Image satisfactionPieSection;  // Pie Chart section for Satisfaction
    public Image savingsPieSection;  // Pie Chart section for Savings

    private string selectedCharacterID;

    void Start()
    {
        selectedCharacterID = CharacterSelectionManager.Instance.SelectedCharacterID;

        // Call method to update performance UI
        UpdatePerformanceUI();
    }

    void UpdatePerformanceUI()
    {
        // Get the total stars and level-specific stars from StarSystem
        int totalStars = StarSystem.Instance.GetTotalStarsForCharacter(selectedCharacterID);

        // Use StarSystem to fetch the level-specific stars for the selected character
        int totalLevels = LevelStateManager.Instance.AllLevels.Length;
        bool[] nutritionStars = new bool[totalLevels];
        bool[] satisfactionStars = new bool[totalLevels];
        bool[] savingsStars = new bool[totalLevels];

        // Get level-specific stars from StarSystem
        for (int i = 0; i < totalLevels; i++)
        {
            int stars = StarSystem.Instance.GetStarsForLevel(i, selectedCharacterID);
            nutritionStars[i] = stars > 0;  // Nutrition star logic
            satisfactionStars[i] = stars > 1;  // Satisfaction star logic
            savingsStars[i] = stars == 3;  // Savings star logic
        }

        // Set performance summary text
        performanceSummaryText.text = $"Total Stars: {totalStars}/15";

        // Set performance bars (nutrition, satisfaction, savings)
        SetPerformanceBar(nutritionBar, nutritionStars.Count(star => star), totalLevels);
        SetPerformanceBar(satisfactionBar, satisfactionStars.Count(star => star), totalLevels);
        SetPerformanceBar(savingsBar, savingsStars.Count(star => star), totalLevels);

        // Update PieChart (or any other overall performance visualization)
        UpdatePieChart(nutritionStars, satisfactionStars, savingsStars);

        // Update the most noticeable habit based on performance
        string noticeableHabit = GetMostNoticedHabit(nutritionStars, satisfactionStars, savingsStars);
        Debug.Log($"Most Noticeable Habit: {noticeableHabit}");
    }

    // Set performance bars (for nutrition, satisfaction, savings)
    void SetPerformanceBar(Image bar, int starsMet, int totalLevels)
    {
        float fillAmount = (float)starsMet / totalLevels;
        bar.fillAmount = fillAmount;
    }

    // Update PieChart (overall performance)
    void UpdatePieChart(bool[] nutritionStars, bool[] satisfactionStars, bool[] savingsStars)
    {
        // Calculate percentages for each category
        float nutritionPercentage = (float)nutritionStars.Count(star => star) / nutritionStars.Length;
        float satisfactionPercentage = (float)satisfactionStars.Count(star => star) / satisfactionStars.Length;
        float savingsPercentage = (float)savingsStars.Count(star => star) / savingsStars.Length;

        // Assuming PieChart divides the circle into 3 sections (nutrition, satisfaction, savings)
        // Update each section of the pie chart with the respective percentage
        nutritionPieSection.fillAmount = nutritionPercentage;  // Set PieChart section for Nutrition
        satisfactionPieSection.fillAmount = satisfactionPercentage;  // Set PieChart section for Satisfaction
        savingsPieSection.fillAmount = savingsPercentage;  // Set PieChart section for Savings
    }

    // Get the most noticeable habit based on player's performance
    string GetMostNoticedHabit(bool[] nutritionStars, bool[] satisfactionStars, bool[] savingsStars)
    {
        if (nutritionStars.Count(star => star) == nutritionStars.Length)
        {
            return "Excellent Nutrition Habit";
        }
        else if (satisfactionStars.Count(star => star) == satisfactionStars.Length)
        {
            return "High Satisfaction";
        }
        else if (savingsStars.Count(star => star) == savingsStars.Length)
        {
            return "Strong Savings Habit";
        }
        else
        {
            return "Balanced Performance";
        }
    }
}
