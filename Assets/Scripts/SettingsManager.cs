using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class SettingsManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject settingPanel;
    [SerializeField] private GameObject[] uiObjects;
    [SerializeField] private GameObject preferences;
    [SerializeField] private GameObject about;
    [SerializeField] private GameObject controls;
    [SerializeField] private TextMeshProUGUI titleText;

    [Header("Fade Settings")]
    [SerializeField] private float fadeDuration = 0.5f;

    [Header("Button References")]
    [SerializeField] private Button preferencesButton;
    [SerializeField] private Button aboutButton;
    [SerializeField] private Button controlsButton;

    private CanvasGroup settingCanvasGroup;
    private CanvasGroup preferencesCanvasGroup;
    private CanvasGroup aboutCanvasGroup;
    private CanvasGroup controlsCanvasGroup;
    private CanvasGroup titleTextCanvasGroup;

    private void Awake()
    {
        settingCanvasGroup = settingPanel.GetComponent<CanvasGroup>();
        preferencesCanvasGroup = preferences.GetComponent<CanvasGroup>();
        aboutCanvasGroup = about.GetComponent<CanvasGroup>();
        controlsCanvasGroup = controls.GetComponent<CanvasGroup>();

        titleTextCanvasGroup = titleText.GetComponent<CanvasGroup>();
        if (titleTextCanvasGroup == null)
        {
            titleTextCanvasGroup = titleText.gameObject.AddComponent<CanvasGroup>();
        }

        settingCanvasGroup.alpha = 0;
        preferencesCanvasGroup.alpha = 0;
        aboutCanvasGroup.alpha = 0;
        controlsCanvasGroup.alpha = 0;
        titleTextCanvasGroup.alpha = 0; 

        settingPanel.SetActive(false);
        preferences.SetActive(false);
        about.SetActive(false);
        controls.SetActive(false);

        preferencesButton.onClick.AddListener(ShowPreferences);
        aboutButton.onClick.AddListener(ShowAbout);
        controlsButton.onClick.AddListener(ShowControls);
    }

    public void ShowSettings()
    {
        SetUIObjectsActive(false);
        settingPanel.SetActive(true);
        titleText.gameObject.SetActive(true);
        StartCoroutine(FadePanel(settingCanvasGroup, 1f));
        StartCoroutine(FadePanel(preferencesCanvasGroup, 0f));
        StartCoroutine(FadePanel(aboutCanvasGroup, 0f));
        StartCoroutine(FadePanel(controlsCanvasGroup, 0f));
        StartCoroutine(FadePanel(titleTextCanvasGroup, 1f));
        SetTitleText("Settings");
    }

    public void ShowPreferences()
    {
        StartCoroutine(SwitchPanels(preferences, preferencesCanvasGroup, "Preferences"));
    }

    public void ShowAbout()
    {
        StartCoroutine(SwitchPanels(about, aboutCanvasGroup, "About"));
    }

    public void ShowControls()
    {
        StartCoroutine(SwitchPanels(controls, controlsCanvasGroup, "Controls"));
    }

    public void CloseAllPanels()
    {
        SetUIObjectsActive(true);
        StartCoroutine(FadeAndDeactivatePanel(settingCanvasGroup, settingPanel));
        StartCoroutine(FadeAndDeactivatePanel(preferencesCanvasGroup, preferences));
        StartCoroutine(FadeAndDeactivatePanel(aboutCanvasGroup, about));
        StartCoroutine(FadeAndDeactivatePanel(controlsCanvasGroup, controls));
        StartCoroutine(FadeAndDeactivatePanel(titleTextCanvasGroup, titleText.gameObject));
    }

    private IEnumerator FadePanel(CanvasGroup canvasGroup, float targetAlpha)
    {
        float startAlpha = canvasGroup.alpha;
        float timeElapsed = 0f;

        while (timeElapsed < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, timeElapsed / fadeDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
    }

    private IEnumerator SwitchPanels(GameObject newPanel, CanvasGroup newPanelCanvasGroup, string title)
    {
        // Fade out the currently active panel and title text simultaneously
        List<Coroutine> fadingCoroutines = new List<Coroutine>();

        if (preferences.activeSelf)
        {
            fadingCoroutines.Add(StartCoroutine(FadePanel(preferencesCanvasGroup, 0f)));
            fadingCoroutines.Add(StartCoroutine(FadePanel(titleTextCanvasGroup, 0f)));
        }
        else if (about.activeSelf)
        {
            fadingCoroutines.Add(StartCoroutine(FadePanel(aboutCanvasGroup, 0f)));
            fadingCoroutines.Add(StartCoroutine(FadePanel(titleTextCanvasGroup, 0f)));
        }
        else if (controls.activeSelf)
        {
            fadingCoroutines.Add(StartCoroutine(FadePanel(controlsCanvasGroup, 0f)));
            fadingCoroutines.Add(StartCoroutine(FadePanel(titleTextCanvasGroup, 0f)));
        }

        // Wait for both fade operations to finish simultaneously
        foreach (var fadeCoroutine in fadingCoroutines)
        {
            yield return fadeCoroutine;
        }

        // Deactivate all panels after fading out
        preferences.SetActive(false);
        about.SetActive(false);
        controls.SetActive(false);

        // Activate the new panel and fade it in along with the title text
        newPanel.SetActive(true);
        SetTitleText(title);

        // Start both fade-in coroutines for the new panel and title text
        var fadePanelIn = StartCoroutine(FadePanel(newPanelCanvasGroup, 1f));
        var fadeTitleIn = StartCoroutine(FadePanel(titleTextCanvasGroup, 1f));

        // Wait for both fade-in operations to finish simultaneously
        yield return fadePanelIn;
        yield return fadeTitleIn;

        // Set the new title text
    }

    private IEnumerator FadeAndDeactivatePanel(CanvasGroup canvasGroup, GameObject panel)
    {
        yield return StartCoroutine(FadePanel(canvasGroup, 0f)); 
        panel.SetActive(false); 
    }

    private void SetUIObjectsActive(bool isActive)
    {
        foreach (var uiObject in uiObjects)
        {
            uiObject.SetActive(isActive);
        }
    }
    private void SetTitleText(string panelName)
    {
        titleText.text = panelName;
    }
}
