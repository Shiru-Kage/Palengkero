using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WellBeingMeter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image nutritionBar;
    [SerializeField] private Image satisfactionBar;

    [Header("Settings")]
    [SerializeField] private int maxNutrition = 200;
    [SerializeField] private int maxSatisfaction = 200;

    [SerializeField] private Color normalColor = Color.green;
    [SerializeField] private Color decreaseColor = Color.red;
    [SerializeField] private float colorChangeDuration = 2f;

    private int currentNutrition = 0;
    private int currentSatisfaction = 0;
    public int CurrentNutrition => currentNutrition;
    public int CurrentSatisfaction => currentSatisfaction;

    private Coroutine nutritionColorRoutine;
    private Coroutine satisfactionColorRoutine;

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
        if (nutritionDelta < 0)
        {
            if (nutritionColorRoutine != null) StopCoroutine(nutritionColorRoutine);
            nutritionColorRoutine = StartCoroutine(FlashBarColor(nutritionBar, decreaseColor));
        }

        if (satisfactionDelta < 0)
        {
            if (satisfactionColorRoutine != null) StopCoroutine(satisfactionColorRoutine);
            satisfactionColorRoutine = StartCoroutine(FlashBarColor(satisfactionBar, decreaseColor));
        }

        currentNutrition = Mathf.Clamp(currentNutrition + nutritionDelta, 0, maxNutrition);
        currentSatisfaction = Mathf.Clamp(currentSatisfaction + satisfactionDelta, 0, maxSatisfaction);

        UpdateBars();
    }

    private IEnumerator FlashBarColor(Image bar, Color flashColor)
    {
        if (bar == null) yield break;

        Color originalColor = normalColor;
        bar.color = flashColor;

        yield return new WaitForSeconds(colorChangeDuration);

        bar.color = originalColor;
    }

    private void UpdateBars()
    {
        if (nutritionBar != null)
        {
            nutritionBar.fillAmount = (float)currentNutrition / maxNutrition;
            nutritionBar.color = normalColor; 
        }

        if (satisfactionBar != null)
        {
            satisfactionBar.fillAmount = (float)currentSatisfaction / maxSatisfaction;
            satisfactionBar.color = normalColor; 
        }
    }

    public void ResetBars()
    {
        currentNutrition = 0;
        currentSatisfaction = 0;
        UpdateBars();
    }
}
