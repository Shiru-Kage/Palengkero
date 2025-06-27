using UnityEngine;
using TMPro;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI weeklyBudgetText;

    [Header("Objective UI Handler")]
    [SerializeField] private LevelObjectiveUI objectiveUI;

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

        StartCoroutine(SetupLevel());
    }

    private IEnumerator SetupLevel()
    {
        // Step 1: Spawn character and stalls
        SpawnCharacterAndStalls();

        // Step 2: Wait until all stalls and their colliders are ready
        yield return new WaitUntil(() =>
        {
            var allStalls = FindObjectsByType<Stall>(FindObjectsSortMode.None);

            foreach (var stall in allStalls)
            {
                var collider = stall.GetComponent<Collider2D>();
                if (collider == null || !collider.enabled)
                {
                    Debug.LogWarning($"Stall '{stall.name}' is missing or has disabled Collider2D.");
                    return false;
                }
            }
            return true;
        });

        // Step 3: Wait one physics frame to finalize registration
        yield return new WaitForFixedUpdate();

        // Step 4: Generate grid
        if (PathfindingGrid.Instance != null)
        {
            PathfindingGrid.Instance.GenerateGrid();
            Debug.Log("✅ Pathfinding grid generated after all stall colliders were detected.");
        }
        else
        {
            Debug.LogWarning("❌ PathfindingGrid.Instance is null.");
        }
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
