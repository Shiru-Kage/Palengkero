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
    [SerializeField] private Transform spawnPoint;
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

        if (stallPrefabs == null || stallPrefabs.Length == 0 || haggleSystem == null || spawnPoint == null ||
            stallInnerUIContainer == null || stallUICanvasObject == null)
        {
            Debug.LogError("StallManager is missing required references.");
            return;
        }

        totalAssignedItems = 0;

        for (int i = 0; i < numberOfStalls; i++)
        {
            GameObject stallPrefab = stallPrefabs[i % stallPrefabs.Length];

            Vector3 position = spawnPoint.position;

            GameObject stallInstance = Instantiate(stallPrefab, position, spawnPoint.rotation, stallCanvas);

            Stall stall = stallInstance.GetComponent<Stall>();
            StallUI stallUI = stallInstance.GetComponent<StallUI>();
            StallCooldown stallCooldown = stallInstance.GetComponentInChildren<StallCooldown>();

            if (stall == null || stallUI == null)
            {
                Debug.LogError($"Stall prefab missing Stall or StallUI component (Stall #{i + 1}).");
                continue;
            }

            stallUI.AssignUIContainer(stallInnerUIContainer);
            stallUI.SetUICanvasObject(stallUICanvasObject);
            stallUI.SetupUIReferences();

            Button[] itemButtons = stallUI.GetItemButtons();

            stall.Initialize(haggleSystem, stallUICanvasObject, itemButtons);

            allStalls.Add(stall);

            int stallAssignedItems = stall.GetTotalAssignedItemCount();
            totalAssignedItems += stallAssignedItems;
        }

        Debug.Log($"[StallManager] Total assigned items across all stalls: {totalAssignedItems}");
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
