using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }
    private int currentProgress = 0;

    [SerializeField] private GameObject defaultCharacterPrefab;
    [SerializeField] private List<TutorialStep> tutorialSteps;  // Correct variable name

    [SerializeField] private int nextSceneIndex;

    [Header("UI Integration")]
    [SerializeField] private TutorialUI tutorialUI;

    private bool isTutorialActive = false;
    public bool IsTutorialActive() => isTutorialActive;
    private Coroutine hideTutorialCoroutine = null;

    [System.Serializable]
    public class TutorialStep
    {
        public bool pause;  // Whether to pause time during this step
        public bool hideAfterDelay;
        public int moveToNext;  // Step index to move to next (or -1 to stay)
        public float timer;  // Time to wait before moving to the next step
        public UnityEvent onTrigger;  // Actions to trigger at the start of this step
        public UnityEvent onStepComplete;  // Actions to trigger when the step is completed
        public string requiredAction;  // The action needed to proceed (e.g., "MovePlayer")
        public string instructionText;  // The tutorial instruction text
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (isTutorialActive)
        {
            StartTutorial();
        }
    }

    public void StartTutorial()
    {
        isTutorialActive = true;
        SelectDefaultCharacter();
        currentProgress = 0;
        HandleTutorial();
    }

    public void Next()
    {
        Time.timeScale = 1;
        currentProgress++;
        HandleTutorial();
    }

    public void CompleteTutorial(int tutorialIndex)
    {
        if (currentProgress != tutorialIndex) return;
        HandleEnd(tutorialSteps[tutorialIndex]);
    }
    public void Previous()
    {
        Time.timeScale = 1;
        currentProgress--;
        HandleTutorial();
    }
    private void HandleTutorial()
    {
        if (currentProgress >= tutorialSteps.Count) return;

        TutorialStep currentStep = tutorialSteps[currentProgress];

        if (hideTutorialCoroutine != null)
        {
            StopCoroutine(hideTutorialCoroutine);
        }

        currentStep.onTrigger?.Invoke(); // Trigger any actions (e.g., highlighting UI)

        Time.timeScale = currentStep.pause ? 0 : 1;  // Pause the game during this step

        if (currentStep.timer > 0)
        {
            StartCoroutine(RemoveTutorialDelay(currentStep.timer, currentStep));
        }

        tutorialUI.ShowTutorialStep(currentStep);  // Update tutorial UI
        if (currentStep.hideAfterDelay)
        {
            hideTutorialCoroutine = StartCoroutine(HideTutorialAfterDelay(5f));  // Set 3 seconds or any other duration
        }
    }

    private void HandleEnd(TutorialStep step)
    {
        if (step.moveToNext >= 0)
        {
            Next();
        }

        step.onStepComplete?.Invoke(); // Call onStepComplete actions after the tutorial ends
    }

    private IEnumerator RemoveTutorialDelay(float delay, TutorialStep step)
    {
        yield return new WaitForSeconds(delay);
        HandleEnd(step);
    }

    public void NotifyAction(string action)
    {
        if (!isTutorialActive) return;

        if (currentProgress < 0 || currentProgress >= tutorialSteps.Count) return;

        var step = tutorialSteps[currentProgress];
        if (step.requiredAction == action)
        {
            step.onStepComplete?.Invoke();
            Next();
        }
    }
    private IEnumerator HideTutorialAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        tutorialUI.HideTutorial(); // Hide the tutorial UI after the delay
    }
    private void SelectDefaultCharacter()
    {
        if (defaultCharacterPrefab != null)
        {
            CharacterSelectionManager.Instance.SetSelectedPrefab(defaultCharacterPrefab);
            Debug.Log("Default character selected for the tutorial.");
        }
        else
        {
            Debug.LogError("Default character prefab not assigned in TutorialManager.");
        }
    }

    public int GetCurrentProgress()
    {
        return currentProgress;
    }
    
    public int GetTutorialStepsCount()
    {
        return tutorialSteps.Count;
    }
}
