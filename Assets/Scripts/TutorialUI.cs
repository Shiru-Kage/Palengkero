using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
public class TutorialUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject tutorialPanel;  // Main tutorial UI container
    [SerializeField] private TextMeshProUGUI tutorialText;  // The text box that displays tutorial instructions
    [SerializeField] private Button nextStep;
    [SerializeField] private Button previousStep;

    private void Awake()
    {
        HideAllElements();  // Initially hide all elements
    }

    // Display the tutorial step with instruction text
    public void ShowTutorialStep(TutorialManager.TutorialStep step)
    {
        tutorialPanel.SetActive(true);
        tutorialText.text = step.instructionText;  // Show the text immediately

        // Only show the buttons if the timer is set to -1
        if (step.timer <= -1 && string.IsNullOrEmpty(step.requiredAction))
        {
            nextStep.gameObject.SetActive(true);
            previousStep.gameObject.SetActive(true);
        }
        else
        {
            nextStep.gameObject.SetActive(false);
            previousStep.gameObject.SetActive(false);
        }
    }

    // Hide all tutorial elements
    private void HideAllElements()
    {
        tutorialPanel.SetActive(false);
        nextStep.gameObject.SetActive(false);
        previousStep.gameObject.SetActive(false);
    }

    public IEnumerator HideTutorialPanelAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        HideTutorial();
    }


    // Hide the tutorial UI after the tutorial is complete
    public void HideTutorial()
    {
        tutorialPanel.SetActive(false);
    }
}
