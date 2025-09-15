using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
public class TextTutorialManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] private Image tutorialImage;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button previousButton;

    [Header("Tutorial Objects")]
    [SerializeField] private GameObject tutorialObject;
    [SerializeField] private GameObject readableTutorialObject;

    [Header("Pages")]
    [SerializeField] private List<TutorialPage> pages = new List<TutorialPage>();
    

    private int currentPageIndex = 0;

    public void StartTutorial()
    {
        currentPageIndex = 0;
        ShowPage(currentPageIndex);
    }

    public void NextPage()
    {
        if (currentPageIndex < pages.Count - 1)
        {
            currentPageIndex++;
            ShowPage(currentPageIndex);
        }
        else
        {
            if (tutorialObject != null) tutorialObject.SetActive(false);
            if (readableTutorialObject != null) readableTutorialObject.SetActive(false);

            currentPageIndex = 0;
        }
    }

    public void PreviousPage()
    {
        if (currentPageIndex > 0)
        {
            currentPageIndex--;
            ShowPage(currentPageIndex);
        }
    }

    private void ShowPage(int index)
    {
        TutorialPage page = pages[index];

        // Update text
        tutorialText.text = page.pageText;

        // Update image
        if (page.pageImage != null)
        {
            tutorialImage.sprite = page.pageImage;
            tutorialImage.gameObject.SetActive(true);
        }
        else
        {
            tutorialImage.gameObject.SetActive(false);
        }

        previousButton.gameObject.SetActive(index > 0);
        nextButton.gameObject.SetActive(true);
    }
}

