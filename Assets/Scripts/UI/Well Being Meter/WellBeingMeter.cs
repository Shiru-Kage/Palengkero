using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WellBeingMeter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image nutritionBar1; 
    [SerializeField] private Image nutritionBar2;
    [SerializeField] private Image satisfactionBar1; 
    [SerializeField] private Image satisfactionBar2;
    [Header("Text References")]
    [SerializeField] private TextMeshProUGUI nutritionText;
    [SerializeField] private TextMeshProUGUI satisfactionText; 

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
        currentNutrition = Mathf.Clamp(currentNutrition + nutritionDelta, 0, maxNutrition);
        currentSatisfaction = Mathf.Clamp(currentSatisfaction + satisfactionDelta, 0, maxSatisfaction);

        UpdateBars();
    }

    private void UpdateBars()
    {
        if (nutritionBar1 != null && nutritionBar2 != null)
        {
            float firstBarFill = Mathf.Clamp(currentNutrition, 0, 100) / 100f;
            float secondBarFill = Mathf.Clamp(currentNutrition - 100, 0, 100) / 100f;

            nutritionBar1.fillAmount = firstBarFill;
            nutritionBar2.fillAmount = secondBarFill;
        }

        if (satisfactionBar1 != null && satisfactionBar2 != null)
        {
            float firstBarFill = Mathf.Clamp(currentSatisfaction, 0, 100) / 100f;
            float secondBarFill = Mathf.Clamp(currentSatisfaction - 100, 0, 100) / 100f;

            satisfactionBar1.fillAmount = firstBarFill;
            satisfactionBar2.fillAmount = secondBarFill;
        }
        if (nutritionText != null)
        {
            nutritionText.text = $"{currentNutrition}/{maxNutrition}";
        }

        if (satisfactionText != null)
        {
            satisfactionText.text = $"{currentSatisfaction}/{maxSatisfaction}";
        }
    }

    public void ResetBars()
    {
        currentNutrition = 0;
        currentSatisfaction = 0;
        UpdateBars();
    }
}
