using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
public class LevelSelectUI : MonoBehaviour
{
    public static LevelSelectUI Instance { get; private set; }

    [Header("Level Buttons")]
    [SerializeField] private Button[] levelButtons;
    [SerializeField] private Button playButton;

    [Header("UI References")]
    [SerializeField] private Image levelSprite;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI levelDescriptionText;
    [SerializeField] private LevelObjectiveUI objectiveUI;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void OnEnable()
    {
        UpdateLevelButtons();
    }

    public void UpdateLevelUI(LevelData currentLevel)
    {
        if (CharacterSelectionManager.Instance == null || CharacterSelectionManager.Instance.SelectedCharacterData == null)
        {
            Debug.LogError("No selected character found.");
            return;
        }

        CharacterData selectedCharacter = CharacterSelectionManager.Instance.SelectedCharacterData;
        CharacterObjective objective = currentLevel.GetObjectiveFor(selectedCharacter);

        if (levelText != null)
            levelText.text = "Level " + currentLevel.levelNumber;

        if (levelDescriptionText != null)
            levelDescriptionText.text = currentLevel.levelDescription ?? "No description available.";

        if (levelSprite != null)
            levelSprite.sprite = currentLevel.levelIcon;

        if (objectiveUI != null)
            objectiveUI.UpdateObjectiveUI(objective);
        else
            Debug.LogWarning("Objective UI reference missing in LevelSelectUI.");
        UpdateLevelButtons();
    }


    private void UpdateLevelButtons()
    {
        if (LevelStateManager.Instance == null)
        {
            Debug.LogWarning("LevelStateManager not yet ready, skipping UpdateLevelButtons.");
            return;
        }
        int maxEverUnlocked = LevelStateManager.Instance.GetMaxEverUnlockedLevelIndexForCurrentCharacter();
        for (int i = 0; i < levelButtons.Length; i++)
        {
            levelButtons[i].interactable = (i <= maxEverUnlocked);
        }

        int currentIndex = LevelStateManager.Instance.CurrentLevelIndex;
        playButton.interactable = LevelStateManager.Instance.IsLevelUnlocked(currentIndex);
    }



    public void RefreshLevelButtons()
    {
        UpdateLevelButtons();
    }

    public void UpdateButtonsAfterLevelUnlock()
    {
        UpdateLevelButtons();
    }
}
