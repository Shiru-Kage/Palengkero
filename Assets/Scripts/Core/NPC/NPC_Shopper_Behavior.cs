using UnityEngine;

public class NPC_Shopper_Behavior : MonoBehaviour
{
    private Stall[] allStalls;
    private Stall currentStallTarget;
    private NPC_Shopper shopper;

    private Vector3? currentTarget;

    private void Awake()
    {
        shopper = GetComponent<NPC_Shopper>();
    }

    private void Start()
    {
        allStalls = FindObjectsByType<Stall>(FindObjectsSortMode.None);
        ChooseNewStallTarget();
    }

    public void OnTargetReached()
    {
        if (currentStallTarget == null) return;

        var buttons = currentStallTarget.GetItemButtons();
        if (buttons != null && buttons.Length > 0)
        {
            currentStallTarget.PurchaseItem(Random.Range(0, buttons.Length));
        }

        ChooseNewStallTarget();
    }

    private void ChooseNewStallTarget()
    {
        if (allStalls == null || allStalls.Length == 0) return;

        // Choose one stall randomly
        currentStallTarget = allStalls[Random.Range(0, allStalls.Length)];

        // Use interactionTransform if available, otherwise fallback to transform
        Transform interactionTransform = currentStallTarget.interactionTransform != null
            ? currentStallTarget.interactionTransform
            : currentStallTarget.transform;

        // Properly apply the offset
        Vector3 targetPos = interactionTransform.position + (Vector3)currentStallTarget.boxOffset;

        // Store for debug / gizmos
        currentTarget = new Vector3(targetPos.x, targetPos.y, 0f); // keep it on 2D plane

        // Tell the NPC to move there
        shopper.SetTarget(currentTarget.Value);
    }
    private Stall FindNearestStall()
    {
        Stall nearest = null;
        float minDist = Mathf.Infinity;
        foreach (var stall in allStalls)
        {
            float dist = Vector3.Distance(transform.position, stall.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = stall;
            }
        }
        return nearest;
    }

    private void OnDrawGizmos()
    {
        if (currentTarget.HasValue)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, currentTarget.Value);
            Gizmos.DrawSphere(currentTarget.Value, 0.1f);
        }
    }
}
