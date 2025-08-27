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
    private int[] successRates = new int[] { 40, 30, 20 }; //{ 40, 30, 20 };
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
            }
        }

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

        if (failScene != null)
        {
            dialogueManager.PlayScene(failScene);
        }

        if (attemptCount >= 3)
        {
            attemptCount = 0;
            dialogueManager.events.OnSceneEnd.AddListener(() =>
            {
                if (dialogueManager.CurrentScene == failScene)
                {
                    StartCoroutine(WaitForDialogueToFinish());
                }
            });
        }
    }
    private IEnumerator WaitForDialogueToFinish()
    {
        DialogueBoxController dialogueController = Object.FindFirstObjectByType<DialogueBoxController>();
        while (!dialogueController.IsCompleted())
        {
            yield return null;
        }
        while (dialogueManager.CurrentScene != null)
        {
            yield return null;
        }
        if (attemptCount == 0)
        {
            stallInnerUIContainer.SetActive(false);
        }
        if (currentHaggledStall != null && currentHaggledStall.GetStallCooldown() != null && attemptCount == 0)
        {
            currentHaggledStall.GetStallCooldown().TriggerCooldown();
            StartCoroutine(ResetAfterCooldown());
        }
    }

    private void ResetAttempts()
    {
        attemptCount = 0;
    }

    private IEnumerator ResetAfterCooldown()
    {
        while (currentHaggledStall != null && currentHaggledStall.GetStallCooldown() != null && currentHaggledStall.GetStallCooldown().isCoolingDown)
        {
            yield return null;
        }

        ResetAttempts();
    }

    private void OnFailSceneEnd_ShowStallDetails()
    {
        var stallUI = Object.FindAnyObjectByType<StallUI>();
        stallUI.HideDetailsAfterHaggle();
    }
}
