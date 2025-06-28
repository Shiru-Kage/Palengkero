using UnityEngine;
using System.Collections.Generic;

public class NPC_Shopper_Behavior : MonoBehaviour
{
    private Stall[] allStalls;
    private Stall currentStallTarget;
    private NPC_Shopper shopper;
    private bool isRoaming = true;
    private Color gizmoColor = Color.cyan;

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
        else
        {
            // NPC is on the way to a stall
            if (currentStallTarget != null && shopper.IsAtTarget())
            {
                // Once NPC reaches the stall's box offset, decide whether to buy
                DecideOnAction();
            }
        }
    }

    private void DecideOnAction()
    {
        // NPC is at the stall, decide whether to buy something
        if (Random.Range(0, 100) < shopper.Data.buyLikelihood)
        {
            // NPC decides to buy an item from this stall
            ChooseItemToBuy();
        }

        // After attempting to buy (or not), go back to roaming
        isRoaming = true;  
    }

    private void RoamAround()
    {
        Vector2Int randomGridPos = GetRandomWalkableGridPosition();
        Vector3 targetPosition = PathfindingGrid.Instance.GetWorldPosition(randomGridPos.x, randomGridPos.y);
        shopper.SetTarget(targetPosition);  // Delegate target setting to NPC_Shopper

        // Randomly decide if NPC will visit a stall
        if (Random.Range(0, 100) < shopper.Data.buyLikelihood)
        {
            Stall stallToVisit = allStalls[Random.Range(0, allStalls.Length)];

            // Set target to stall’s box offset
            SetTargetToStallBoxOffset(stallToVisit);
            currentStallTarget = stallToVisit;

            isRoaming = false; // Stop roaming to visit the stall
        }
    }

    private void SetTargetToStallBoxOffset(Stall stall)
    {
        Vector3 targetPosition = stall.transform.position + (Vector3)stall.boxOffset;
        shopper.SetTarget(targetPosition);  // Move towards the stall's box offset
    }

    private void ChooseItemToBuy()
    {
        // NPC will buy an item based on preferences after arriving at the stall
        Stall stall = currentStallTarget;
        if (stall != null)
        {
            ItemData itemToBuy = null;

            // Preference logic to pick an item based on NPC's preferences
            if (Random.Range(0, 100) < shopper.Data.preferCheapItemsChance)
            {
                itemToBuy = GetCheapItem(stall);
            }
            else if (Random.Range(0, 100) < shopper.Data.preferHighNutritionChance)
            {
                itemToBuy = GetHighNutritionItem(stall);
            }
            else if (Random.Range(0, 100) < shopper.Data.preferHighSatisfactionChance)
            {
                itemToBuy = GetHighSatisfactionItem(stall);
            }

            // If an item was selected, attempt to buy it
            if (itemToBuy != null)
            {
                stall.PurchaseItemForNPC(stall.GetItemIndex(itemToBuy)); // Attempt to buy item
                Debug.Log($"{shopper.name} successfully bought {itemToBuy.itemName} from the stall!");
            }
        }

        // After buying (or not), pick a new random grid position to roam
        RoamAround();  
    }

    private ItemData GetCheapItem(Stall stall)
    {
        // Find item with lowest price in the stall
        ItemData cheapestItem = null;
        foreach (var item in stall.GetAssignedItems())
        {
            if (cheapestItem == null || item.price < cheapestItem.price)
            {
                cheapestItem = item;
            }
        }
        return cheapestItem;
    }

    private ItemData GetHighNutritionItem(Stall stall)
    {
        // Find item with highest nutrition in the stall
        ItemData bestItem = null;
        foreach (var item in stall.GetAssignedItems())
        {
            if (bestItem == null || item.nutrition > bestItem.nutrition)
            {
                bestItem = item;
            }
        }
        return bestItem;
    }

    private ItemData GetHighSatisfactionItem(Stall stall)
    {
        // Find item with highest satisfaction in the stall
        ItemData bestItem = null;
        foreach (var item in stall.GetAssignedItems())
        {
            if (bestItem == null || item.satisfaction > bestItem.satisfaction)
            {
                bestItem = item;
            }
        }
        return bestItem;
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

    private void OnDrawGizmos()
    {
        // Use the dynamic gizmoColor when drawing gizmos
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, 0.2f);  // Draw a sphere at the NPC's position for visual aid
    }
}
