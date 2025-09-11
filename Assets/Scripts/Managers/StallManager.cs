using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class StallManager : MonoBehaviour
{
    public static StallManager Instance { get; private set; }
    private List<Stall> allStalls = new List<Stall>();

    [Header("References")]
    [SerializeField] private GameObject[] stallPrefabs;
    [SerializeField] private HaggleSystem haggleSystem;
    [SerializeField] private Transform stallCanvas;
    [SerializeField] private GameObject stallUICanvasObject;
    [SerializeField] private Transform stallInnerUIContainer;
    [SerializeField] private GameObject endLevelScreen;

    [Header("Spawn Settings")]
    [SerializeField] private int numberOfRows = 3; 
    [SerializeField] private int numberOfColumns = 3; 
    [SerializeField] private Vector3 cellOffset = new Vector3(0.5f, 0.5f, 0f);
    [SerializeField] private Vector3 cellSpacing = new Vector3(1f, 1f, 0f);
    private int numberOfStalls;

    private int totalAssignedItems; 

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SpawnStalls()
    {
        LevelData currentLevelData = LevelStateManager.Instance.GetCurrentLevelData();

        if (currentLevelData != null)
        {
            numberOfStalls = currentLevelData.numberOfStalls; 
        }
        else
        {
            Debug.LogError("LevelData is missing. Stall spawn settings will not be applied.");
            return;
        }

        if (stallPrefabs == null || stallPrefabs.Length == 0 || haggleSystem == null ||
            stallInnerUIContainer == null || stallUICanvasObject == null)
        {
            Debug.LogError("StallManager is missing required references.");
            return;
        }

        totalAssignedItems = 0;

        int stallsSpawned = 0;

        for (int row = 0; row < numberOfRows && stallsSpawned < numberOfStalls; row++)
        {
            for (int col = 0; col < numberOfColumns && stallsSpawned < numberOfStalls; col++)
            {
                Vector3 worldPosition = GetCellWorldPosition(row, col);

                int stallIndex = row * numberOfColumns + col;
                if (stallIndex >= stallPrefabs.Length) break;

                GameObject stallPrefab = stallPrefabs[stallIndex];
                GameObject stallInstance = Instantiate(stallPrefab, worldPosition, Quaternion.identity, stallCanvas);

                Stall stall = stallInstance.GetComponent<Stall>();
                StallUI stallUI = stallInstance.GetComponent<StallUI>();
                StallCooldown stallCooldown = stallInstance.GetComponentInChildren<StallCooldown>();

                stallUI.AssignUIContainer(stallInnerUIContainer);
                stallUI.SetUICanvasObject(stallUICanvasObject);
                stallUI.SetupUIReferences();

                Button[] itemButtons = stallUI.GetItemButtons();

                stall.Initialize(haggleSystem, stallUICanvasObject, itemButtons);

                allStalls.Add(stall);

                int stallAssignedItems = stall.GetTotalAssignedItemCount();
                totalAssignedItems += stallAssignedItems;

                stallsSpawned++;  

                if (stallsSpawned >= numberOfStalls) break; 
            }
        }

        Debug.Log($"[StallManager] Total assigned items across all stalls: {totalAssignedItems}");
    }


    private Vector3 GetCellWorldPosition(int row, int col)
    {
        Vector2Int gridSize = PathfindingGrid.Instance.GetGridSize();
        
        row = Mathf.Clamp(row, 0, gridSize.y - 1);
        col = Mathf.Clamp(col, 0, gridSize.x - 1);

        Vector3 cellCenter = PathfindingGrid.Instance.GetWorldPosition(col, row);
        Vector3 spacingAdjustment = new Vector3(col * cellSpacing.x, row * cellSpacing.y, 0f);
        
        return cellCenter + cellOffset + spacingAdjustment;
    }

    public void ReduceGlobalItemCount(int amount = 1)
    {
        totalAssignedItems -= amount;
        if (totalAssignedItems <= 0)
        {
            totalAssignedItems = 0;
            var summary = FindAnyObjectByType<LevelSummarySequence>();
            if (summary != null)
            {
                endLevelScreen.SetActive(true);
                summary.BeginSummarySequence();
            }
        }
    }

    public Stall RequestAvailableStall(NPC_Shopper_Behavior npc)
    {
        var available = allStalls.FindAll(s => !s.IsReserved && s.GetTotalAssignedItemCount() > 0);
        if (available.Count == 0) return null;

        Stall chosen = available[Random.Range(0, available.Count)];
        chosen.ReserveFor(npc);
        return chosen;
    }

    public void ToggleMovement(bool enable)
    {
        var player = Object.FindFirstObjectByType<PlayerController>();
        if (player != null)
        {
            player.ToggleMovement(enable);
        }
    }

    public int GetRemainingGlobalItems() => totalAssignedItems;
}
