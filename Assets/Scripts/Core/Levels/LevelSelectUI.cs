using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class LevelSelectUI : MonoBehaviour
{
    public static LevelSelectUI Instance { get; private set; }

    [Header("Level Buttons")]
    [SerializeField] private Button[] levelButtons;

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

    private void Start()
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
    }


    public void UpdateLevelButtons()
    {
        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (LevelStateManager.Instance.IsLevelUnlocked(i))
            {
                levelButtons[i].interactable = true;
            }
            else
            {
                levelButtons[i].interactable = false;
            }
        }
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
