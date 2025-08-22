using UnityEngine;
using UnityEngine.UI;

public class StallManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject stallPrefab;
    [SerializeField] private HaggleSystem haggleSystem;
    [SerializeField] private Transform stallCanvas;
    [SerializeField] private GameObject stallUICanvasObject; 
    [SerializeField] private Transform stallInnerUIContainer;

    [Header("Spawn Settings")]
    [SerializeField] private Transform spawnPoint;
    private int numberOfStalls;                   

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

        if (stallPrefab == null || haggleSystem == null || spawnPoint == null ||
            stallInnerUIContainer == null || stallUICanvasObject == null)
        {
            Debug.LogError("StallManager is missing required references.");
            return;
        }

        for (int i = 0; i < numberOfStalls; i++)
        {
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
        }
    }
}
