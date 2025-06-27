using UnityEngine;
using System.Collections;

public class HaggleSystem : MonoBehaviour
{
    [Header("Scenes")]
    [SerializeField] private StoryScene haggleIntroScene;
    [SerializeField] private StoryScene successScene;
    [SerializeField] private StoryScene failScene;

    [Header("Cooldown")]
    [SerializeField] private StallCooldown stallCooldown;

    [Header("UI")]
    [SerializeField] private GameObject stallInnerUIContainer;

    private DialogueManager dialogueManager;
    private int attemptCount = 0;
    private int[] successRates = new int[] { 50, 40, 30 };
    private bool waitingForResult = false;

    private string discountedItemId = null;
    public string DiscountedItemId => discountedItemId;

    public void ApplyHaggleDiscount(string itemId)
    {
        discountedItemId = itemId;
    }

    public void ResetDiscount()
    {
        discountedItemId = null;
    }

    public void StartHaggle(DialogueManager manager)
    {
        if (haggleIntroScene == null || manager == null)
        {
            Debug.LogWarning("Missing DialogueManager or haggleIntroScene.");
            return;
        }

        dialogueManager = manager;
        waitingForResult = true;

        dialogueManager.events.OnSceneEnd.AddListener(HandleResultAfterIntro);
        manager.PlayScene(haggleIntroScene);
    }

    private void HandleResultAfterIntro()
    {
        if (!waitingForResult) return;

        waitingForResult = false;
        dialogueManager.events.OnSceneEnd.RemoveListener(HandleResultAfterIntro);

        bool success = DetermineSuccess();

        if (success)
        {
            HandleSuccess();
        }
        else
        {
            HandleFailure();
        }
    }

    private bool DetermineSuccess()
    {
        int rate = successRates[Mathf.Clamp(attemptCount, 0, successRates.Length - 1)];
        return Random.Range(0, 100) < rate;
    }

    private void HandleSuccess()
    {
        attemptCount = 0;

        var stall = Object.FindAnyObjectByType<Stall>();
        if (stall != null)
        {
            var selectedItem = stall.GetSelectedItem();
            if (selectedItem != null)
            {
                ApplyHaggleDiscount(selectedItem.id);
                stall.UpdateSelectedItemUIAfterHaggle();
            }
        }

        Debug.Log("Haggle success: 50% off applied to specific item.");

        if (successScene != null)
        {
            dialogueManager.events.OnSceneEnd.AddListener(OnDialogueSceneEnd_ShowStallDetails);
            dialogueManager.PlayScene(successScene);
        }
    }

    private void HandleFailure()
    {
        attemptCount++;
        ResetDiscount();
        Debug.Log($"Haggle failed at attempt {attemptCount}. Discount reset.");

        if (attemptCount >= 3)
        {
            Debug.Log("3 failed attempts. Triggering cooldown.");
            attemptCount = 0;

            if (stallCooldown != null)
                stallCooldown.TriggerCooldown();

            if (stallInnerUIContainer != null)
                stallInnerUIContainer.SetActive(false);

            StartCoroutine(ResetAfterCooldown());
        }

        if (failScene != null)
        {
            dialogueManager.events.OnSceneEnd.AddListener(OnDialogueSceneEnd_ShowStallDetails);
            dialogueManager.PlayScene(failScene);
        }
    }


    private void ResetAttempts()
    {
        attemptCount = 0;
        Debug.Log("Attempt count reset after cooldown.");
    }

    private IEnumerator ResetAfterCooldown()
    {
        while (stallCooldown != null && stallCooldown.isCoolingDown)
        {
            yield return null;
        }

        ResetAttempts();
    }

    public void SetStallCooldown(StallCooldown cooldown)
    {
        stallCooldown = cooldown;
    }
    
    private void OnDialogueSceneEnd_ShowStallDetails()
    {
        dialogueManager.events.OnSceneEnd.RemoveListener(OnDialogueSceneEnd_ShowStallDetails);

        var stallUI = Object.FindAnyObjectByType<StallUI>();
        if (stallUI != null)
        {
            stallUI.DisplayDetailsAfterHaggle();
        }
        else
        {
            Debug.LogWarning("StallUI not found when trying to display details after haggle.");
        }
    }
}
