using UnityEngine;
using TMPro;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI weeklyBudgetText;

    [Header("Objective UI Handler")]
    [SerializeField] private LevelObjectiveUI objectiveUI;   // ✅ NEW: Reference to the LevelObjectiveUI script

    [Header("Spawners")]
    [SerializeField] private CharacterSpawner characterSpawner;
    [SerializeField] private StallManager stallManager;

    private void Start()
    {
        if (LevelStateManager.Instance != null)
        {
            SetLevel(LevelStateManager.Instance.CurrentLevelIndex);
        }
        else
        {
            Debug.LogWarning("LevelStateManager not found. Defaulting to index 0.");
            SetLevel(0);
        }

        SpawnCharacterAndStalls();
    }

    public void SetLevel(int levelIndex)
    {
        LevelData[] levels = LevelStateManager.Instance?.AllLevels;

        if (levels == null || levels.Length == 0)
        {
            Debug.LogError("No level data available from LevelStateManager.");
            return;
        }

        if (levelIndex < 0 || levelIndex >= levels.Length)
        {
            Debug.LogError("Invalid level index: " + levelIndex);
            return;
        }

        LevelStateManager.Instance.SetLevelIndex(levelIndex);
        CharacterSelectionManager.Instance?.ResetRuntimeCharacterBudget();
        UpdateUI(levels[levelIndex]);
    }

    private void UpdateUI(LevelData currentLevel)
    {
        CharacterSelectionManager manager = CharacterSelectionManager.Instance;

        if (manager == null || manager.SelectedCharacterData == null || manager.SelectedRuntimeCharacter == null)
        {
            Debug.LogError("No selected character found.");
            return;
        }

        CharacterData selectedCharacter = manager.SelectedCharacterData;
        RuntimeCharacter runtimeCharacter = manager.SelectedRuntimeCharacter;

        weeklyBudgetText.text = $"PHP {runtimeCharacter.currentWeeklyBudget}.00";

        CharacterObjective objective = currentLevel.GetObjectiveFor(selectedCharacter);

        // ✅ NEW: Use LevelObjectiveUI to display goal texts
        if (objectiveUI != null)
        {
            objectiveUI.UpdateObjectiveUI(objective);
        }
        else
        {
            Debug.LogWarning("ObjectiveUI reference missing on LevelManager.");
        }
    }

    public void UpdateBudgetDisplay()
    {
        RuntimeCharacter runtimeCharacter = CharacterSelectionManager.Instance?.SelectedRuntimeCharacter;
        if (runtimeCharacter != null)
        {
            weeklyBudgetText.text = $"PHP {runtimeCharacter.currentWeeklyBudget}.00";
        }
    }

    private void SpawnCharacterAndStalls()
    {
        if (characterSpawner != null)
        {
            characterSpawner.SpawnSelectedCharacter();
        }
        else
        {
            Debug.LogWarning("CharacterSpawner reference missing on LevelManager.");
        }

        if (stallManager != null)
        {
            stallManager.SpawnStalls();
        }
        else
        {
            Debug.LogWarning("StallManager reference missing on LevelManager.");
        }
    }
}
