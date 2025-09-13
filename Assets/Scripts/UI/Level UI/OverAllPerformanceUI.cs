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
    public Image nutritionBar;
    public Image satisfactionBar;
    public Image savingsBar;
    public Image nutritionPieSection; 
    public Image satisfactionPieSection;  
    public Image savingsPieSection;  

    private string selectedCharacterID;

    void Start()
    {
        selectedCharacterID = CharacterSelectionManager.Instance.SelectedCharacterID;
        UpdatePerformanceUI();
        DisplayEnding();
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

        performanceSummaryText.text = $"Total Stars: {totalStars}/15";

        SetPerformanceBar(nutritionBar, nutritionStars.Count(star => star), totalLevels);
        SetPerformanceBar(satisfactionBar, satisfactionStars.Count(star => star), totalLevels);
        SetPerformanceBar(savingsBar, savingsStars.Count(star => star), totalLevels);

        UpdatePieChart(nutritionStars, satisfactionStars, savingsStars);

        string noticeableHabit = GetMostNoticedHabit(nutritionStars, satisfactionStars, savingsStars);
        noticeableHabitText.text = $"Most Noticeable Habit: {noticeableHabit}";
    }

    void SetPerformanceBar(Image bar, int starsMet, int totalLevels)
    {
        float fillAmount = (float)starsMet / totalLevels;
        bar.fillAmount = fillAmount;
    }

    void UpdatePieChart(bool[] nutritionStars, bool[] satisfactionStars, bool[] savingsStars)
    {
        float nutritionAchieved = nutritionStars.Count(star => star);
        float satisfactionAchieved = satisfactionStars.Count(star => star);
        float savingsAchieved = savingsStars.Count(star => star);

        int totalLevels = LevelStateManager.Instance.AllLevels.Length;

        float nutritionPercentage = nutritionAchieved / totalLevels;
        float satisfactionPercentage = satisfactionAchieved / totalLevels;
        float savingsPercentage = savingsAchieved / totalLevels;

        float totalPercentage = nutritionPercentage + satisfactionPercentage + savingsPercentage;

        if (totalPercentage > 1f)
        {
            float scaleFactor = 1f / totalPercentage;
            nutritionPercentage *= scaleFactor;
            satisfactionPercentage *= scaleFactor;
            savingsPercentage *= scaleFactor;
        }

        float currentAngle = 0f;

        nutritionPieSection.fillAmount = nutritionPercentage;
        nutritionPieSection.transform.rotation = Quaternion.Euler(0, 0, -currentAngle * 360f); 
        currentAngle += nutritionPercentage;

        satisfactionPieSection.fillAmount = satisfactionPercentage;
        satisfactionPieSection.transform.rotation = Quaternion.Euler(0, 0, -currentAngle * 360f); 
        currentAngle += satisfactionPercentage;

        savingsPieSection.fillAmount = savingsPercentage;
        savingsPieSection.transform.rotation = Quaternion.Euler(0, 0, -currentAngle * 360f); 
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
}
