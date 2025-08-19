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

    private GameObject currentTilemapObject;

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
        SpawnCharacterAndStalls();

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

        yield return new WaitForFixedUpdate();

        if (PathfindingGrid.Instance != null)
        {
            PathfindingGrid.Instance.GenerateGrid();
        }
        else
        {
            Debug.LogWarning("PathfindingGrid.Instance is null.");
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
        ItemDatabaseManager.Instance.AdjustPricesBasedOnLevel(levelIndex);
        CharacterSelectionManager.Instance?.ResetRuntimeCharacterBudget();
        UpdateUI(levels[levelIndex]);
        InstantiateTilemap(levels[levelIndex]);
    }

    private void InstantiateTilemap(LevelData currentLevel)
    {
        if (currentTilemapObject != null)
        {
            Destroy(currentTilemapObject);  // Destroy the previous level's Tilemap if any
        }

        if (currentLevel.levelTileMap != null)
        {
            // Find the Grid GameObject in the scene
            Transform gridTransform = GameObject.Find("Grid")?.transform;

            if (gridTransform != null)
            {
                // Instantiate the levelTileMap under the "Grid" object
                currentTilemapObject = Instantiate(currentLevel.levelTileMap, gridTransform.position, Quaternion.identity);
                currentTilemapObject.transform.SetParent(gridTransform);  // Set the parent to "Grid"
            }
            else
            {
                Debug.LogWarning("Grid GameObject not found in the scene.");
            }
        }
        else
        {
            Debug.LogWarning("No Tilemap assigned in LevelData.");
        }
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
