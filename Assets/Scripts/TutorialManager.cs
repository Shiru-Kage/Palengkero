using UnityEngine;
using System.Collections.Generic;

public class TutorialManager : MonoBehaviour
{
    [Header("Default Character Settings")]
    [SerializeField] private GameObject defaultCharacterPrefab; 
    [SerializeField] private List<TutorialStep> steps;
    private int currentStepIndex = -1;
    private bool isTutorialActive = false;
    public static TutorialManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void StartTutorial()
    {
        isTutorialActive = true;
        SelectDefaultCharacter();
        currentStepIndex = -1;
        AdvanceStep();
    }

    private void AdvanceStep()
    {
        if (!isTutorialActive) return;

        currentStepIndex++;
        if (currentStepIndex >= steps.Count)
        {
            Debug.Log("Tutorial completed!");
            EndTutorial();
            return;
        }

        var step = steps[currentStepIndex];
        Debug.Log($"Starting step: {step.instructionText}");
        step.onStepStart?.Invoke();
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

    public void NotifyAction(string action)
    {
        if (!isTutorialActive) return;

        if (currentStepIndex < 0 || currentStepIndex >= steps.Count) return;

        var step = steps[currentStepIndex];
        if (step.requiredAction == action)
        {
            step.onStepComplete?.Invoke();
            AdvanceStep();
        }
    }

    private void EndTutorial()
    {
        isTutorialActive = false;
    }
}
