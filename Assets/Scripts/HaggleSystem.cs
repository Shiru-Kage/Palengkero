using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class HaggleSystem : MonoBehaviour
{
    [Header("Scenes")]
    [SerializeField] private StoryScene haggleIntroScene; // Played first
    [SerializeField] private StoryScene successScene;     // Played if haggle success
    [SerializeField] private StoryScene failScene;        // Played if haggle failed

    [Header("Cooldown")]
    [SerializeField] private StallCooldown stallCooldown;

    [Header("UI")]
    [SerializeField] private GameObject stallInnerUI;
    [SerializeField] private GameObject stallInnerUIContainer;

    private DialogueManager dialogueManager;
    private int attemptCount = 0;
    private int[] successRates = new int[] { 50, 40, 30 };
    private bool waitingForResult = false;

    public void StartHaggle(DialogueManager manager)
    {
        if (haggleIntroScene == null || manager == null)
        {
            Debug.LogWarning("Missing DialogueManager or haggleIntroScene.");
            return;
        }

        dialogueManager = manager;
        waitingForResult = true;

        // Subscribe to OnDialogueEnd only once
        dialogueManager.events.OnSceneEnd.AddListener(HandleResultAfterIntro);

        // Play intro haggle scene
        manager.PlayScene(haggleIntroScene);
    }

    private void HandleResultAfterIntro()
    {
        if (!waitingForResult) return;

        waitingForResult = false;
        dialogueManager.events.OnSceneEnd.RemoveListener(HandleResultAfterIntro);

        int chance = successRates[Mathf.Clamp(attemptCount, 0, successRates.Length - 1)];
        bool success = Random.Range(0, 100) < chance;

        if (success)
        {
            Debug.Log($"Haggle success at attempt {attemptCount + 1} ({chance}%)");
            if (successScene != null)
            {
                dialogueManager.events.OnSceneEnd.AddListener(ShowStallInnerUI);
                dialogueManager.PlayScene(successScene);
            }
        }
        else
        {
            Debug.Log($"Haggle failed at attempt {attemptCount + 1} ({chance}%)");
            attemptCount++;

            if (attemptCount >= 3)
            {
                Debug.Log("3 failed attempts. Triggering cooldown.");
                attemptCount = 0;
                if (stallCooldown != null)
                    stallCooldown.TriggerCooldown();
                stallInnerUIContainer.SetActive(false);
                StartCoroutine(ResetAfterCooldown());
            }

            if (failScene != null)
            {
                dialogueManager.events.OnSceneEnd.AddListener(ShowStallInnerUI);
                dialogueManager.PlayScene(failScene);
            }
        }
    }

    private void ShowStallInnerUI()
    {
        dialogueManager.events.OnSceneEnd.RemoveListener(ShowStallInnerUI);

        if (stallInnerUI != null)
            stallInnerUI.SetActive(true);
    }

    private void ResetAttempts()
    {
        attemptCount = 0;
        Debug.Log("Attempt count reset after cooldown.");
    }

    private IEnumerator ResetAfterCooldown()
    {
        while (stallCooldown.isCoolingDown)
        {
            yield return null; // Wait until cooldown completes
        }

        ResetAttempts();
    }
}
