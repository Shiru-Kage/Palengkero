using UnityEngine;
using UnityEngine.UI;

public class StallManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject stallPrefab;
    [SerializeField] private HaggleSystem haggleSystem;
    [SerializeField] private Transform stallCanvas;
    [SerializeField] private Transform stallInnerUIContainer;    
    [SerializeField] private GameObject stallInnerUICanvasObject; 

    [Header("Spawn Settings")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private int numberOfStalls = 1;                    

    public void SpawnStalls()
    {
        if (stallPrefab == null || haggleSystem == null || spawnPoint == null ||
            stallInnerUIContainer == null || stallInnerUICanvasObject == null)
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
            stallUI.SetUICanvasObject(stallInnerUICanvasObject);
            stallUI.SetupUIReferences();

            Button[] itemButtons = stallUI.GetItemButtons();

            stall.Initialize(haggleSystem, stallInnerUICanvasObject, itemButtons);

            Debug.Log($"Stall #{i + 1} spawned at {position}");
        }
    }
}
