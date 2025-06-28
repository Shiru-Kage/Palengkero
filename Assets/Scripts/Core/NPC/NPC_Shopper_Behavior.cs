using UnityEngine;

public class NPC_Shopper_Behavior : MonoBehaviour
{
    private Stall[] allStalls;
    private Stall currentStallTarget;
    private NPC_Shopper shopper;
    private NPCState currentState;
    private Color gizmoColor = Color.green;

    private bool hasPurchasedItem = false;

    private enum NPCState
    {
        Roaming,
        MovingToStall,
        BuyingItem
    }

    private void Awake()
    {
        shopper = GetComponent<NPC_Shopper>();
        shopper.OnIdleStarted += HandleIdleStarted;
    }

    private void Start()
    {
        allStalls = FindObjectsByType<Stall>(FindObjectsSortMode.None);
        currentState = NPCState.Roaming;
    }

    private void OnDestroy()
    {
        if (shopper != null)
            shopper.OnIdleStarted -= HandleIdleStarted;
    }

    private void Update()
    {
        switch (currentState)
        {
            case NPCState.MovingToStall:
                HandleMovingToStall();
                break;

            case NPCState.BuyingItem:
                HandleBuyingItem();
                break;
        }
    }

    private void HandleIdleStarted()
    {
        if (Random.Range(0, 100) < shopper.Data.buyLikelihood)
        {
            // pick a stall with at least one item in stock
            Stall stallToVisit = ChooseStallWithStock();
            if (stallToVisit != null)
            {
                SetTargetToStallBoxOffset(stallToVisit);
                currentStallTarget = stallToVisit;
                shopper.LockTarget();
                currentState = NPCState.MovingToStall;
            }
            else
            {
                Debug.Log($"{shopper.name} found no stalls with available stock.");
                currentState = NPCState.Roaming;
            }
        }
    }

    private Stall ChooseStallWithStock()
    {
        var availableStalls = new System.Collections.Generic.List<Stall>();

        foreach (var stall in allStalls)
        {
            if (stall == null) continue;

            bool hasStock = false;
            var items = stall.GetAssignedItems();
            for (int i = 0; i < items.Length; i++)
            {
                var (_, stock) = stall.GetItemAndStock(i);
                if (stock > 0)
                {
                    hasStock = true;
                    break;
                }
            }

            if (hasStock)
                availableStalls.Add(stall);
        }

        if (availableStalls.Count > 0)
        {
            return availableStalls[Random.Range(0, availableStalls.Count)];
        }

        return null;
    }

    private void HandleMovingToStall()
    {
        if (shopper.IsAtTarget())
        {
            currentState = NPCState.BuyingItem;
        }
    }

    private void HandleBuyingItem()
    {
        if (currentStallTarget != null && !hasPurchasedItem)
        {
            Vector3 stallTargetPosition = currentStallTarget.GetApproachPoint();

            if (Vector3.Distance(shopper.transform.position, stallTargetPosition) < 0.1f)
            {
                bool success = TryChooseItemToBuy();
                if (success)
                {
                    hasPurchasedItem = true;
                }

                shopper.UnlockTarget();
                currentState = NPCState.Roaming;
                hasPurchasedItem = false;
            }
        }
    }

    private void SetTargetToStallBoxOffset(Stall stall)
    {
        Vector3 targetPosition = stall.GetApproachPoint();
        shopper.SetTarget(targetPosition);
    }

    private bool TryChooseItemToBuy()
    {
        if (currentStallTarget == null) return false;

        ItemData itemToBuy = null;

        // preference order
        if (Random.Range(0, 100) < shopper.Data.preferCheapItemsChance)
        {
            itemToBuy = GetCheapItem(currentStallTarget);
        }
        else if (Random.Range(0, 100) < shopper.Data.preferHighNutritionChance)
        {
            itemToBuy = GetHighNutritionItem(currentStallTarget);
        }
        else if (Random.Range(0, 100) < shopper.Data.preferHighSatisfactionChance)
        {
            itemToBuy = GetHighSatisfactionItem(currentStallTarget);
        }

        // check stock
        if (itemToBuy != null)
        {
            int index = currentStallTarget.GetItemIndex(itemToBuy);
            var (_, stock) = currentStallTarget.GetItemAndStock(index);
            if (stock <= 0)
            {
                itemToBuy = GetAnyAvailableItem(currentStallTarget);
            }
        }
        else
        {
            itemToBuy = GetAnyAvailableItem(currentStallTarget);
        }

        if (itemToBuy != null)
        {
            int itemIndex = currentStallTarget.GetItemIndex(itemToBuy);
            bool purchaseSuccess = currentStallTarget.PurchaseItemForNPC(itemIndex);
            if (purchaseSuccess)
            {
                Debug.Log($"{shopper.name} successfully bought {itemToBuy.itemName} from the stall!");
                return true;
            }
            else
            {
                Debug.Log($"{shopper.name} tried to buy {itemToBuy.itemName} but stock was empty.");
            }
        }
        else
        {
            Debug.Log($"{shopper.name} found no items to buy at {currentStallTarget.name}");
        }

        return false;
    }

    private ItemData GetCheapItem(Stall stall)
    {
        ItemData cheapest = null;
        foreach (var item in stall.GetAssignedItems())
        {
            if (cheapest == null || item.price < cheapest.price)
                cheapest = item;
        }
        return cheapest;
    }

    private ItemData GetHighNutritionItem(Stall stall)
    {
        ItemData best = null;
        foreach (var item in stall.GetAssignedItems())
        {
            if (best == null || item.nutrition > best.nutrition)
                best = item;
        }
        return best;
    }

    private ItemData GetHighSatisfactionItem(Stall stall)
    {
        ItemData best = null;
        foreach (var item in stall.GetAssignedItems())
        {
            if (best == null || item.satisfaction > best.satisfaction)
                best = item;
        }
        return best;
    }

    private ItemData GetAnyAvailableItem(Stall stall)
    {
        var items = stall.GetAssignedItems();
        for (int i = 0; i < items.Length; i++)
        {
            var (item, stock) = stall.GetItemAndStock(i);
            if (item != null && stock > 0)
                return item;
        }
        return null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, 0.2f);
    }
}
