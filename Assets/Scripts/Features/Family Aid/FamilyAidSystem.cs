using UnityEngine;
using UnityEngine.UI; 

public class FamilyAidSystem : MonoBehaviour
{
    [Header("Timer Settings")]
    [SerializeField] private Timer timer;  

    [Header("Family Aid Deductions")]
    private const int youngerSiblingDeduction = 500;
    private const int fatherDeduction = 300;
    private const int motherDeduction = 100;

    [Header("Family Aid Chances")]
    [Range(0, 100)] public int youngerSiblingChance = 50;
    [Range(0, 100)] public int fatherChance = 30;
    [Range(0, 100)] public int motherChance = 20;

    [Header("Cooldown Indicator")]
    [SerializeField] private Image fillImage; 
    [SerializeField] private Color fillColor = Color.green;  
    [SerializeField] private Color emptyColor = Color.red; 

    [Header("Adjustable Timer Interval")]
    [SerializeField] private float aidInterval = 300f;  

    private float maxFillTime;
    private float currentFillTime;

    void Start()
    {
        maxFillTime = aidInterval;
        currentFillTime = 0f;
        fillImage.fillAmount = 0f;
        fillImage.color = emptyColor;
        if (timer.IsRunning)
        {
            timer.ResetTimer(aidInterval);  
        }
    }

    void Update()
    {
        if (timer.IsRunning)
        {
            if (currentFillTime < maxFillTime)
            {
                currentFillTime += Time.deltaTime;
                fillImage.fillAmount = currentFillTime / maxFillTime;

                fillImage.color = Color.Lerp(emptyColor, fillColor, fillImage.fillAmount);
            }

            if (currentFillTime >= maxFillTime)
            {
                DeductFamilyAid();
                ResetCooldown(); 
            }
        }
    }

    private void DeductFamilyAid()
    {
        if (CharacterSelectionManager.Instance.SelectedRuntimeCharacter != null)
        {
            var character = CharacterSelectionManager.Instance.SelectedRuntimeCharacter;

            int randomValue = Random.Range(0, 100);

            if (randomValue < youngerSiblingChance)
            {
                character.currentWeeklyBudget -= youngerSiblingDeduction;
                Debug.Log($"[Family Aid] Younger Sibling selected! Deduction: -500. Current Budget: {character.currentWeeklyBudget}");
            }
            else if (randomValue < (youngerSiblingChance + fatherChance))
            {
                character.currentWeeklyBudget -= fatherDeduction;
                Debug.Log($"[Family Aid] Father selected! Deduction: -300. Current Budget: {character.currentWeeklyBudget}");
            }
            else
            {
                character.currentWeeklyBudget -= motherDeduction;
                Debug.Log($"[Family Aid] Mother selected! Deduction: -100. Current Budget: {character.currentWeeklyBudget}");
            }
        }
        else
        {
            Debug.LogWarning("[Family Aid] No character selected to deduct from.");
        }

        LevelManager levelManager = Object.FindAnyObjectByType<LevelManager>();
        if (levelManager != null)
            levelManager.UpdateBudgetDisplay();
    }

    private void ResetCooldown()
    {
        currentFillTime = 0f;
        fillImage.fillAmount = 0f;  
        timer.ResetTimer(aidInterval);  
    }

    public void SetAidInterval(float newInterval)
    {
        aidInterval = newInterval;
        maxFillTime = newInterval; 
        currentFillTime = 0f; 
        fillImage.fillAmount = 0f; 
        timer.ResetTimer(newInterval); 
    }

    public void SetFamilyAidChances(int sibling, int father, int mother)
    {
        if (sibling + father + mother > 100)
        {
            return;
        }

        youngerSiblingChance = sibling;
        fatherChance = father;
        motherChance = mother;
    }
}
