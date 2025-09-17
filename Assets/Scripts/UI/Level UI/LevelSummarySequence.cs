using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelSummarySequence : MonoBehaviour
{
    [Header("Intro Message")]
    [SerializeField] private TextMeshProUGUI introMessageText;
    [SerializeField] [TextArea] private string introMessage;

    [Header("Time Spent")]
    [SerializeField] private TextMeshProUGUI timeSpentText;

    [Header("Continue Button")]
    [SerializeField] private Button continueButton;
    [SerializeField] private Button levelSelect;

    [Header("Advice UI")]
    [SerializeField] private TextMeshProUGUI adviceText;

    [Header("Advice Profiles")]
    [SerializeField] private List<AdviceProfile> adviceProfiles;

    [Header("Settings")]
    [SerializeField] private float fadeDuration = 1f;

    [Header("Goal Texts")]
    [SerializeField] private TextMeshProUGUI nutritionGoalText;
    [SerializeField] private TextMeshProUGUI satisfactionGoalText;
    [SerializeField] private TextMeshProUGUI savingsGoalText;

    [SerializeField] private Timer levelTimer;
    [SerializeField] private WellBeingMeter wellBeingMeter;

    [SerializeField] private InventoryUI inventory;

    private CharacterObjective currentObjective;

    private void Awake()
    {
        introMessageText.alpha = 0f;
        timeSpentText.alpha = 0f;
        adviceText.alpha = 0f;
        nutritionGoalText.alpha = 0f;
        satisfactionGoalText.alpha = 0f;
        savingsGoalText.alpha = 0f;
        continueButton.gameObject.SetActive(false);
        levelSelect.gameObject.SetActive(false);
    }

    void Start()
    {
        inventory = FindAnyObjectByType<InventoryUI>();
    }
    public void BeginSummarySequence()
    {
        var objectiveUI = Object.FindAnyObjectByType<LevelObjectiveUI>();
        if (objectiveUI != null)
            currentObjective = objectiveUI.CurrentObjective;

        if (introMessageText != null)
        {
            introMessageText.text = introMessage;
            StartCoroutine(ShowIntro());
        }
    }

    private IEnumerator ShowIntro()
    {
        yield return FadeInText(introMessageText);

        continueButton.gameObject.SetActive(true);
        continueButton.onClick.RemoveAllListeners();
        continueButton.onClick.AddListener(() =>
        {
            continueButton.gameObject.SetActive(false);
            StartCoroutine(ShowObjectivesAndTime());
        });
    }

    private IEnumerator ShowObjectivesAndTime()
    {
        if (currentObjective != null)
        {
            yield return ShowObjectiveWithAnimation(currentObjective.nutritionGoal, nutritionGoalText, "Nutrition Goal: ", wellBeingMeter.CurrentNutrition, false);
            yield return ShowObjectiveWithAnimation(currentObjective.satisfactionGoal, satisfactionGoalText, "Satisfaction Goal: ", wellBeingMeter.CurrentSatisfaction, false);
            yield return ShowObjectiveWithAnimation(currentObjective.savingsGoal, savingsGoalText, "Savings Goal: ", currentObjective.savingsGoal, true);
        }

        float levelTimeSpent = levelTimer.CurrentTime;
        timeSpentText.text = $"Time Spent: {levelTimeSpent:F1} seconds";
        yield return FadeInText(timeSpentText);

        continueButton.gameObject.SetActive(true);
        continueButton.onClick.RemoveAllListeners();
        continueButton.onClick.AddListener(() =>
        {
            continueButton.gameObject.SetActive(false);
            introMessageText.alpha = 0f;
            StartCoroutine(ShowAdviceSequence());
        });
    }

    private IEnumerator ShowObjectiveWithAnimation(int goalValue, TextMeshProUGUI goalText, string goalMessage, int actualValue, bool isMoney)
    {
        if (isMoney)
        {
            actualValue = CharacterSelectionManager.Instance?.SelectedRuntimeCharacter.currentWeeklyBudget ?? 0;
        }

        string displayGoalValue = isMoney ? $"₱{goalValue}" : $"{goalValue}";
        string displayActualValue = isMoney ? $"₱{actualValue}" : $"{actualValue}";

        string goalObjective = $"{goalMessage}{displayGoalValue}";
        string resultText = $"You got: {displayActualValue}";
        goalText.text = $"{goalObjective}\n{resultText}";
        goalText.alpha = 0f;

        yield return FadeInText(goalText);

        bool goalMet = actualValue >= goalValue;
        goalText.color = goalMet ? Color.green : Color.red;
        goalText.text = goalMet ?
            $"{goalObjective}\nYou got: {displayActualValue} Cleared" :
            $"{goalObjective}\nYou got: {displayActualValue} Failed";
    }

    private IEnumerator ShowAdviceSequence()
{
    var runtimeCharacter = CharacterSelectionManager.Instance?.SelectedRuntimeCharacter;
    var wellBeing = Object.FindAnyObjectByType<WellBeingMeter>();

    bool metNutrition = false;
    bool metSatisfaction = false;
    bool metSavings = false;

    if (currentObjective != null && wellBeing != null)
    {
        metNutrition = wellBeing.CurrentNutrition >= currentObjective.nutritionGoal;
        metSatisfaction = wellBeing.CurrentSatisfaction >= currentObjective.satisfactionGoal;
    }

    if (runtimeCharacter != null && currentObjective != null)
    {
        metSavings = runtimeCharacter.currentWeeklyBudget >= currentObjective.savingsGoal;
    }

    StarSystem.Instance.AssignStarsForLevel(LevelStateManager.Instance.CurrentLevelIndex, CharacterSelectionManager.Instance.SelectedCharacterID, metNutrition, metSatisfaction, metSavings);
    float levelTimeSpent = levelTimer.CurrentTime;
    timeSpentText.text = $"Time Spent: {levelTimeSpent:F1} seconds";
    LevelStateManager.Instance.SaveLevelTime(levelTimeSpent);

    bool adviceShown = false;

    foreach (var profile in adviceProfiles)
    {
        bool show = false;

        if (profile.requiresSavingsMet && metSavings && !metNutrition && !metSatisfaction) show = true;
        if (profile.requiresPerfectGoals && metNutrition && metSatisfaction && metSavings) show = true;
        if (profile.requiresAllFailedGoals && !metNutrition && !metSatisfaction && !metSavings) show = true;
        if (profile.requiresLowNutrition && !metNutrition && metSatisfaction && metSavings) show = true;
        if (profile.requiresLowSatisfaction && !metSatisfaction && metNutrition && metSavings) show = true;
        if (profile.requiresLowSavings && !metSavings) show = true;

        if (show)
        {
            adviceShown = true;
            adviceText.text = profile.adviceText;
            adviceText.alpha = 0f;
            yield return FadeInText(adviceText);
        }
    }

    if (!adviceShown)
    {
        adviceText.text = "No specific advice this time. You're doing okay!";
        adviceText.alpha = 0f;
        yield return FadeInText(adviceText);
    }

    if (LevelStateManager.Instance.CurrentLevelIndex == LevelStateManager.Instance.AllLevels.Length - 1 &&
        (StarSystem.Instance.GetStarsForLevel(LevelStateManager.Instance.CurrentLevelIndex, CharacterSelectionManager.Instance.SelectedCharacterID).nutritionStars > 0 ||
        StarSystem.Instance.GetStarsForLevel(LevelStateManager.Instance.CurrentLevelIndex, CharacterSelectionManager.Instance.SelectedCharacterID).satisfactionStars > 0 ||
        StarSystem.Instance.GetStarsForLevel(LevelStateManager.Instance.CurrentLevelIndex, CharacterSelectionManager.Instance.SelectedCharacterID).savingsStars > 0))
    {
        // Transition to the Endings scene if the last level is completed
        levelSelect.gameObject.SetActive(true);
        levelSelect.onClick.RemoveAllListeners();
        levelSelect.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("Ending"); // Load the Endings scene
        });
    }
    else
    {
        // Otherwise, allow to move to next level
        levelSelect.gameObject.SetActive(true);
        levelSelect.onClick.RemoveAllListeners();
        levelSelect.onClick.AddListener(() =>
        {
            inventory.ClearInventory();
            if (metNutrition || metSatisfaction || metSavings)
            {
                LevelStateManager.Instance.LockCurrentLevel();
                LevelStateManager.Instance.UnlockNextLevel();
            }
            ReturnToLevelSelect();
        });
    }
}


    private IEnumerator FadeInText(TextMeshProUGUI text)
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            text.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        text.alpha = 1f;
    }

    private void ReturnToLevelSelect()
    {
        SceneManager.LoadScene("LevelSelect");
    }
}
