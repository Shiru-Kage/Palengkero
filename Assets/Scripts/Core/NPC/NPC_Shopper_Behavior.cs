using UnityEngine;

public class NPC_Shopper_Behavior : MonoBehaviour
{
    private Stall[] allStalls;
    private Stall currentStallTarget;
    private NPC_Shopper shopper;
    private bool isRoaming = true;

    private void Awake()
    {
        shopper = GetComponent<NPC_Shopper>();
    }

    private void Start()
    {
        allStalls = FindObjectsByType<Stall>(FindObjectsSortMode.None);
        RoamAround(); // Start roaming at the beginning
    }

    private void Update()
    {
        if (isRoaming)
        {
            // Handle NPC's roaming logic (move towards target set by shopper)
            if (!shopper.HasTarget())
            {
                RoamAround();
            }
        }
    }
    private void DecideOnAction()
    {
        if (currentStallTarget != null)
        {
            // NPC is at a stall, decide whether to buy something
            ChooseItemToBuy();
        }
        else
        {
            // NPC is not at a stall, pick a new random location
            RoamAround();
        }

        isRoaming = true; // Resume roaming after making a decision
    }

    private void RoamAround()
    {
        Vector2Int randomGridPos = GetRandomWalkableGridPosition();
        Vector3 targetPosition = PathfindingGrid.Instance.GetWorldPosition(randomGridPos.x, randomGridPos.y);
        shopper.SetTarget(targetPosition);  // Delegate target setting to NPC_Shopper
    }

    private Vector2Int GetRandomWalkableGridPosition()
    {
        Vector2Int randomGridPos = new Vector2Int(0, 0);
        bool foundWalkable = false;

        while (!foundWalkable)
        {
            randomGridPos = new Vector2Int(Random.Range(0, PathfindingGrid.Instance.GetGridSize().x), Random.Range(0, PathfindingGrid.Instance.GetGridSize().y));

            if (PathfindingGrid.Instance.IsWalkable(randomGridPos.x, randomGridPos.y))
            {
                foundWalkable = true;
            }
        }

        return randomGridPos;
    }

    private void ChooseItemToBuy()
    {

        if (Random.Range(0, 100) < shopper.Data.preferCheapItemsChance)
        {
            // NPC decides to buy from the stall
            currentStallTarget.PurchaseItem(0); // Example of making a purchase
        }

        RoamAround();  // After buying (or not), pick a new random grid position
    }

    private void OnDrawGizmos()
    {
        // Gizmo logic for drawing paths, if needed
    }
}
