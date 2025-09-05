using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

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

        // Add CanvasGroup to titleText for fading
        titleTextCanvasGroup = titleText.GetComponent<CanvasGroup>();
        if (titleTextCanvasGroup == null)
        {
            titleTextCanvasGroup = titleText.gameObject.AddComponent<CanvasGroup>(); // If no CanvasGroup, add one
        }

        settingCanvasGroup.alpha = 0;
        preferencesCanvasGroup.alpha = 0;
        aboutCanvasGroup.alpha = 0;
        controlsCanvasGroup.alpha = 0;
        titleTextCanvasGroup.alpha = 0; // Make sure titleText starts hidden

        // Ensure all panels are initially inactive
        settingPanel.SetActive(false);
        preferences.SetActive(false);
        about.SetActive(false);
        controls.SetActive(false);

        // Set event listeners for the buttons
        preferencesButton.onClick.AddListener(ShowPreferences);
        aboutButton.onClick.AddListener(ShowAbout);
        controlsButton.onClick.AddListener(ShowControls);
    }

    // Show Settings Panel
    public void ShowSettings()
    {
        SetUIObjectsActive(false);
        settingPanel.SetActive(true);
        StartCoroutine(FadePanel(settingCanvasGroup, 1f));
        StartCoroutine(FadePanel(preferencesCanvasGroup, 0f));
        StartCoroutine(FadePanel(aboutCanvasGroup, 0f));
        StartCoroutine(FadePanel(controlsCanvasGroup, 0f));
        StartCoroutine(FadePanel(titleTextCanvasGroup, 1f)); // Fade title text in
        SetTitleText("Settings");
    }

    // Show Preferences Panel
    public void ShowPreferences()
    {
        StartCoroutine(SwitchPanels(preferences, preferencesCanvasGroup, "Preferences"));
    }

    // Show About Panel
    public void ShowAbout()
    {
        StartCoroutine(SwitchPanels(about, aboutCanvasGroup, "About"));
    }

    // Show Controls Panel
    public void ShowControls()
    {
        StartCoroutine(SwitchPanels(controls, controlsCanvasGroup, "Controls"));
    }

    // Close all panels and reset UI objects
    public void CloseAllPanels()
    {
        SetUIObjectsActive(true);
        StartCoroutine(FadeAndDeactivatePanel(settingCanvasGroup, settingPanel));
        StartCoroutine(FadeAndDeactivatePanel(preferencesCanvasGroup, preferences));
        StartCoroutine(FadeAndDeactivatePanel(aboutCanvasGroup, about));
        StartCoroutine(FadeAndDeactivatePanel(controlsCanvasGroup, controls));
        StartCoroutine(FadeAndDeactivatePanel(titleTextCanvasGroup, titleText.gameObject)); // Fade out title text

        titleText.text = "";
    }

    // Fade a panel to a target alpha value
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

    // Handle switching panels with fade effect
    private IEnumerator SwitchPanels(GameObject newPanel, CanvasGroup newPanelCanvasGroup, string title)
    {
        // Fade out the currently active panel
        if (preferences.activeSelf)
            yield return StartCoroutine(FadePanel(preferencesCanvasGroup, 0f));
        else if (about.activeSelf)
            yield return StartCoroutine(FadePanel(aboutCanvasGroup, 0f));
        else if (controls.activeSelf)
            yield return StartCoroutine(FadePanel(controlsCanvasGroup, 0f));

        // Deactivate all panels first
        preferences.SetActive(false);
        about.SetActive(false);
        controls.SetActive(false);

        // Fade out the title text before switching
        yield return StartCoroutine(FadePanel(titleTextCanvasGroup, 0f));

        // Activate the new panel and fade it in
        newPanel.SetActive(true);
        yield return StartCoroutine(FadePanel(newPanelCanvasGroup, 1f));

        // Update the title text and fade it in
        SetTitleText(title);
        yield return StartCoroutine(FadePanel(titleTextCanvasGroup, 1f)); // Fade title text in
    }


    // Fade out and deactivate the panel
    private IEnumerator FadeAndDeactivatePanel(CanvasGroup canvasGroup, GameObject panel)
    {
        yield return StartCoroutine(FadePanel(canvasGroup, 0f)); // Fade the panel out
        panel.SetActive(false); // Deactivate panel after fading
    }

    // Set the active state of all UI objects
    private void SetUIObjectsActive(bool isActive)
    {
        foreach (var uiObject in uiObjects)
        {
            uiObject.SetActive(isActive);
        }
    }

    // Update the title text based on the active panel
    private void SetTitleText(string panelName)
    {
        titleText.text = panelName;
    }
}
