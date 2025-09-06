using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
public class TutorialUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private TextMeshProUGUI tutorialText; 
    [SerializeField] private Button nextStep;
    [SerializeField] private Button previousStep;

    private void Awake()
    {
        HideAllElements(); 
    }

    public void ShowTutorialStep(TutorialManager.TutorialStep step)
    {
        tutorialPanel.SetActive(true);
        tutorialText.text = step.instructionText;  

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

    public void HideTutorial()
    {
        tutorialPanel.SetActive(false);
    }
}
