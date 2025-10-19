using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro; 

public class FamilyAidSystem : MonoBehaviour
{
    [Header("Timer Settings")]
    [SerializeField] private Timer timer;

    [Header("Family Aid Data")]
    [SerializeField] private FamilyAidData familyAidData; 

    [Header("Cooldown Indicator")]
    [SerializeField] private Image fillImage;
    [SerializeField] private Color fillColor = Color.green;
    [SerializeField] private Color emptyColor = Color.red;

    [Header("Adjustable Timer Interval")]
    [SerializeField] private float aidInterval = 300f;

    [Header("UI Elements for Selected Family Member")]
    [SerializeField] private TextMeshProUGUI callerDescriptionText; 
    [SerializeField] private TextMeshProUGUI callerNameText;
    [SerializeField] private GameObject aidReachedObject;
    [SerializeField] private LogBookUI logBookUI;

    private float maxFillTime;
    private float currentFillTime;

    void Start()
    {
        maxFillTime = aidInterval;
        currentFillTime = 0f;
        fillImage.fillAmount = 0f;
        fillImage.color = emptyColor;
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
                ActivateAidReachedObject();
            }
        }
    }

    private void DeductFamilyAid()
    {
        if (CharacterSelectionManager.Instance.SelectedRuntimeCharacter != null)
        {
            var character = CharacterSelectionManager.Instance.SelectedRuntimeCharacter;

            FamilyMemberData selectedMember = RandomlySelectFamilyMember();

            if (selectedMember != null)
            {
                character.currentWeeklyBudget -= selectedMember.familyDeduction;
                logBookUI.AddLog($"Aid deduction: {selectedMember.callerName} -<color=red>{selectedMember.familyDeduction} PHP</color>");

                UpdateFamilyMemberUI(selectedMember);
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

    private FamilyMemberData RandomlySelectFamilyMember()
    {
        int randomIndex = Random.Range(0, familyAidData.familyMembers.Length);
        return familyAidData.GetFamilyMember(randomIndex);
    }

    private void ResetCooldown()
    {
        currentFillTime = 0f;
        fillImage.fillAmount = 0f;
    }

    private void ActivateAidReachedObject()
    {
        if (aidReachedObject != null)
        {
            aidReachedObject.SetActive(true);
            Debug.Log("[Family Aid] Aid interval reached. Aid activated.");
        }
    }

    private void UpdateFamilyMemberUI(FamilyMemberData selectedMember)
    {
        if (callerDescriptionText != null)
        {
            callerDescriptionText.text = selectedMember.callerDescription;
        }

        if (callerNameText != null)
        {
            callerNameText.text = selectedMember.callerName;
        }
    }

    public void SetAidInterval(float newInterval)
    {
        aidInterval = newInterval;
        maxFillTime = newInterval;
        currentFillTime = 0f;
        fillImage.fillAmount = 0f;
        timer.ResetTimer(newInterval);
    }
}
