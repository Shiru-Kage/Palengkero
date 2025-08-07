using UnityEngine;
using System.Collections;

public class HaggleSystem : MonoBehaviour
{
    [Header("Scenes")]
    [SerializeField] private StoryScene haggleIntroScene;
    [SerializeField] private StoryScene successScene;
    [SerializeField] private StoryScene failScene;

    [Header("UI")]
    [SerializeField] private GameObject stallInnerUIContainer;

    private DialogueManager dialogueManager;
    private int attemptCount = 0;
    private int[] successRates = new int[] { 40, 30, 20 };
    private bool waitingForResult = false;

    private Stall currentHaggledStall;

    public void StartHaggle(DialogueManager manager, Stall stall)
    {
        if (haggleIntroScene == null || manager == null || stall == null)
        {
            Debug.LogWarning("Missing DialogueManager, stall, or haggleIntroScene.");
            return;
        }

        dialogueManager = manager;
        waitingForResult = true;
        currentHaggledStall = stall;   

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

        if (currentHaggledStall != null)
        {
            var selectedItem = currentHaggledStall.GetSelectedItem();
            if (selectedItem != null)
            {
                currentHaggledStall.ApplyHaggleDiscount(selectedItem.id); 
                currentHaggledStall.UpdateSelectedItemUIAfterHaggle();
            }
        }

        Debug.Log("Haggle success: 50% off applied to specific item.");

        if (successScene != null)
        {
            dialogueManager.PlayScene(successScene);
        }
    }

    private void HandleFailure()
    {
        attemptCount++;

        if (currentHaggledStall != null)
        {
            currentHaggledStall.ResetDiscount();  
        }

        Debug.Log($"Haggle failed at attempt {attemptCount}. Discount reset.");

        if (attemptCount >= 3)
        {
            Debug.Log("3 failed attempts. Triggering cooldown.");
            attemptCount = 0;

            if (currentHaggledStall != null && currentHaggledStall.GetStallCooldown() != null)
            {
                currentHaggledStall.GetStallCooldown().TriggerCooldown();

                if (stallInnerUIContainer != null)
                    stallInnerUIContainer.SetActive(false);

                StartCoroutine(ResetAfterCooldown());
            }
        }

        if (failScene != null)
        {
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
        while (currentHaggledStall != null && currentHaggledStall.GetStallCooldown() != null && currentHaggledStall.GetStallCooldown().isCoolingDown)
        {
            yield return null;
        }

        ResetAttempts();
    }

    public void OnDialogueSceneEnd_ShowStallDetails()
    {
        var stallUI = Object.FindAnyObjectByType<StallUI>();
        stallUI.DisplayDetailsAfterHaggle();
    }
}
