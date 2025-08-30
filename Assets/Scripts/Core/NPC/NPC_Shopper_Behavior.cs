using UnityEngine;
using System.Collections.Generic;

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

    private List<PrecomputedTarget> precomputedTargets = new List<PrecomputedTarget>();

    private class PrecomputedTarget
    {
        public Stall stall;
        public ItemData item;
        public PrecomputedTarget(Stall s, ItemData i) { stall = s; item = i; }
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
        PrecomputeTargets();
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
            case NPCState.MovingToStall: HandleMovingToStall(); break;
            case NPCState.BuyingItem: HandleBuyingItem(); break;
        }
    }

    private void PrecomputeTargets()
    {
        precomputedTargets.Clear();

        foreach (var stall in allStalls)
        {
            if (stall == null) continue;
            var items = stall.GetAssignedItems();
            if (items == null) continue;

            foreach (var item in items)
            {
                if (item == null) continue;

                bool isPreferred = false;
                if (Random.Range(0, 100) < shopper.Data.preferCheapItemsChance && item.price > 0) isPreferred = true;
                if (Random.Range(0, 100) < shopper.Data.preferHighNutritionChance && item.nutrition > 0) isPreferred = true;
                if (Random.Range(0, 100) < shopper.Data.preferHighSatisfactionChance && item.satisfaction > 0) isPreferred = true;

                if (isPreferred)
                    precomputedTargets.Add(new PrecomputedTarget(stall, item));
            }
        }
    }

    private void HandleIdleStarted()
    {
        if (Random.Range(0, 100) >= shopper.Data.buyLikelihood)
        {
            currentState = NPCState.Roaming;
            return;
        }

        Stall stallToVisit = ChooseStallWithStock();

        if (stallToVisit == null)
        {
            foreach (var stall in allStalls)
            {
                if (stall == null) continue;
                var items = stall.GetAssignedItems();
                if (items == null) continue;

                foreach (var item in items)
                {
                    var (_, stock) = stall.GetItemAndStock(stall.GetItemIndex(item));
                    if (stock > 0 && stall.CanReserve(this))
                    {
                        stallToVisit = stall;
                        break;
                    }
                }
                if (stallToVisit != null) break;
            }
        }

        if (stallToVisit != null)
        {
            Debug.Log($"[{shopper.name}] Decided to buy. Heading to {stallToVisit.name}.");
            SetTargetToStallBoxOffset(stallToVisit);
            currentStallTarget = stallToVisit;
            shopper.LockTarget();
            currentState = NPCState.MovingToStall;
        }
        else
        {
            Debug.Log($"[{shopper.name}] Could not find available stall.");
            currentState = NPCState.Roaming;
        }
    }

    private Stall ChooseStallWithStock()
    {
        precomputedTargets.RemoveAll(t =>
            t.stall == null ||
            t.item == null ||
            t.stall.GetItemAndStock(t.stall.GetItemIndex(t.item)).Item2 <= 0 ||
            !t.stall.CanReserve(this)
        );

        if (precomputedTargets.Count == 0) return null;

        var chosen = precomputedTargets[Random.Range(0, precomputedTargets.Count)];
        return chosen.stall.CanReserve(this) ? chosen.stall : null;
    }

    private void HandleMovingToStall()
    {
        if (HasReachedStall())
        {
            Debug.Log($"[{shopper.name}] reached {currentStallTarget.name}, attempting to buy...");
            currentState = NPCState.BuyingItem;
        }
    }

    private bool HasReachedStall()
    {
        if (currentStallTarget == null) return false;
        return Vector3.Distance(shopper.transform.position, currentStallTarget.GetApproachPoint()) < 0.1f;
    }

    private void HandleBuyingItem()
    {
        if (currentStallTarget == null) return;

        if (!hasPurchasedItem)
        {
            bool success = TryChooseItemToBuy();
            if (success)
            {
                hasPurchasedItem = true;
                PrecomputeTargets();
            }

            currentStallTarget.ReleaseReservation(this);
            shopper.UnlockTarget();
            currentState = NPCState.Roaming;
            hasPurchasedItem = false;
        }
    }

    private void SetTargetToStallBoxOffset(Stall stall)
    {
        shopper.SetTarget(stall.GetApproachPoint());
    }

    private bool TryChooseItemToBuy()
    {
        if (currentStallTarget == null) return false;

        ItemData itemToBuy = null;
        var assignedItems = currentStallTarget.GetAssignedItems();
        if (assignedItems == null || assignedItems.Length == 0) return false;

        // Preference check
        if (Random.Range(0, 100) < shopper.Data.preferCheapItemsChance)
            itemToBuy = GetCheapItem(currentStallTarget);
        else if (Random.Range(0, 100) < shopper.Data.preferHighNutritionChance)
            itemToBuy = GetHighNutritionItem(currentStallTarget);
        else if (Random.Range(0, 100) < shopper.Data.preferHighSatisfactionChance)
            itemToBuy = GetHighSatisfactionItem(currentStallTarget);

        int itemIndex = (itemToBuy != null) ? currentStallTarget.GetItemIndex(itemToBuy) : -1;

        // Fallback to any available item
        if (itemToBuy == null || itemIndex == -1 || currentStallTarget.GetItemAndStock(itemIndex).Item2 <= 0)
        {
            var availableItems = new List<ItemData>();
            foreach (var item in assignedItems)
            {
                if (item != null && currentStallTarget.GetItemAndStock(currentStallTarget.GetItemIndex(item)).Item2 > 0)
                    availableItems.Add(item);
            }

            if (availableItems.Count == 0)
            {
                Debug.LogWarning($"[{shopper.name}] could not buy because stall {currentStallTarget.name} has no available items.");
                return false;
            }

            itemToBuy = availableItems[Random.Range(0, availableItems.Count)];
            itemIndex = currentStallTarget.GetItemIndex(itemToBuy);
            Debug.Log($"[{shopper.name}] Picking random available item {itemToBuy.itemName} from {currentStallTarget.name}");
        }

        bool success = currentStallTarget.PurchaseItem(itemIndex, BuyerType.NPC);
        if (success)
        {
            Debug.Log($"{shopper.name} successfully bought {itemToBuy.itemName} from {currentStallTarget.name}");
            var stallUI = currentStallTarget.GetComponent<StallUI>();
            if (stallUI != null)
                stallUI.SendMessage("PlayPurchaseEffect", itemToBuy, SendMessageOptions.DontRequireReceiver);
            return true;
        }

        Debug.LogWarning($"[{shopper.name}] failed to purchase {itemToBuy.itemName} from {currentStallTarget.name}");
        return false;
    }

    private ItemData GetCheapItem(Stall stall)
    {
        ItemData cheapest = null;
        foreach (var item in stall.GetAssignedItems())
        {
            if (item == null) continue;
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
            if (item == null) continue;
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
            if (item == null) continue;
            if (best == null || item.satisfaction > best.satisfaction)
                best = item;
        }
        return best;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, 0.2f);
    }
}
