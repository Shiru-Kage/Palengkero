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
    [SerializeField] private List<TutorialStep> tutorialSteps;

    [SerializeField] private int nextSceneIndex;

    [Header("UI Integration")]
    [SerializeField] private TutorialUI tutorialUI;
    private Coroutine hideTutorialCoroutine = null;

    [System.Serializable]
    public class TutorialStep
    {
        public bool pause; 
        public bool hideAfterDelay;
        public int moveToNext; 
        public float timer; 
        public UnityEvent onTrigger;  
        public UnityEvent onStepComplete;  
        public string requiredAction;  
        public string instructionText;
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

    public void StartTutorial()
    {
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

        currentStep.onTrigger?.Invoke();

        Time.timeScale = currentStep.pause ? 0 : 1; 

        if (currentStep.timer > 0)
        {
            StartCoroutine(RemoveTutorialDelay(currentStep.timer, currentStep));
        }

        tutorialUI.ShowTutorialStep(currentStep); 
        if (currentStep.hideAfterDelay)
        {
            hideTutorialCoroutine = StartCoroutine(HideTutorialAfterDelay(5f));
        }
    }

    private void HandleEnd(TutorialStep step)
    {
        if (step.moveToNext >= 0)
        {
            Next();
        }

        step.onStepComplete?.Invoke();
    }

    private IEnumerator RemoveTutorialDelay(float delay, TutorialStep step)
    {
        yield return new WaitForSeconds(delay);
        HandleEnd(step);
    }

    public void NotifyAction(string action)
    {
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
        tutorialUI.HideTutorial();
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
