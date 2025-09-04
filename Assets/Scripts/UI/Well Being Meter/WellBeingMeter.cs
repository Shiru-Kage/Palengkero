using UnityEngine;
using UnityEngine.UI;

public class WellBeingMeter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image nutritionBar;
    [SerializeField] private Image satisfactionBar;

    [Header("Settings")]
    [SerializeField] private int maxNutrition = 200;
    [SerializeField] private int maxSatisfaction = 200;

    private int currentNutrition = 0;
    private int currentSatisfaction = 0;
    public int CurrentNutrition => currentNutrition;
    public int CurrentSatisfaction => currentSatisfaction;

    private void Start()
    {
        UpdateBars();
    }

    private void OnEnable()
    {
        WellBeingEvents.OnWellBeingChanged += HandleWellBeingChanged;
    }

    private void OnDisable()
    {
        WellBeingEvents.OnWellBeingChanged -= HandleWellBeingChanged;
    }

    private void HandleWellBeingChanged(int nutritionDelta, int satisfactionDelta)
    {
        // Update nutrition and satisfaction values
        currentNutrition = Mathf.Clamp(currentNutrition + nutritionDelta, 0, maxNutrition);
        currentSatisfaction = Mathf.Clamp(currentSatisfaction + satisfactionDelta, 0, maxSatisfaction);

        // Update the fill amount of the bars
        UpdateBars();
    }

    private void UpdateBars()
    {
        if (nutritionBar != null)
        {
            nutritionBar.fillAmount = (float)currentNutrition / maxNutrition;
        }

        if (satisfactionBar != null)
        {
            satisfactionBar.fillAmount = (float)currentSatisfaction / maxSatisfaction;
        }
    }

    public void ResetBars()
    {
        currentNutrition = 0;
        currentSatisfaction = 0;
        UpdateBars();
    }
}
