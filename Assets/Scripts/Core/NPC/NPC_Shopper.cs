using UnityEngine;
using System.Collections.Generic;

public class NPC_Shopper : MonoBehaviour, ICharacterAnimatorData
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float arrivalThreshold = 0.05f;

    [Header("Pathfinding")]
    [SerializeField] private bool showDebugPath = true;

    [SerializeField] private NPCData npcData;
    public NPCData Data => npcData;

    private Queue<Vector3> pathQueue = new Queue<Vector3>();
    private Vector3? currentTarget;
    private Transform cachedTransform;

    public Vector2 MoveInput { get; private set; }

    private void Awake()
    {
        cachedTransform = transform;
    }

    private void Update()
    {
        FollowPath();
    }

    public void SetTarget(Vector3 worldPos)
    {
        List<Vector3> path = Pathfinder.FindPath(transform.position, worldPos);

        if (path != null && path.Count > 0)
        {
            pathQueue = new Queue<Vector3>(path);
            currentTarget = pathQueue.Dequeue();
        }
        else
        {
            Debug.LogWarning($"{name} could not find path to {worldPos}");
        }
    }

    private void FollowPath()
    {
        if (!currentTarget.HasValue) return;

        Vector3 direction = (currentTarget.Value - cachedTransform.position);
        MoveInput = direction.normalized;

        cachedTransform.position = Vector3.MoveTowards(
            cachedTransform.position,
            currentTarget.Value,
            moveSpeed * Time.deltaTime
        );

        if (PathfindingGrid.Instance != null)
        {
            Vector2Int gridPos = PathfindingGrid.Instance.GetGridPosition(currentTarget.Value);
            bool isWalkable = PathfindingGrid.Instance.IsWalkable(gridPos.x, gridPos.y);
            Debug.Log($"[NPC {name}] Moving to {currentTarget.Value} (Grid {gridPos}) — Walkable: {isWalkable}");
        }

        if (Vector3.Distance(cachedTransform.position, currentTarget.Value) < arrivalThreshold)
        {
            if (pathQueue.Count > 0)
            {
                currentTarget = pathQueue.Dequeue();
            }
            else
            {
                currentTarget = null;
                MoveInput = Vector2.zero;

                // Inform behavior script
                GetComponent<NPC_Shopper_Behavior>()?.OnTargetReached();
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!showDebugPath) return;

        Gizmos.color = Color.cyan;

        if (currentTarget.HasValue)
            Gizmos.DrawWireSphere(currentTarget.Value, 0.1f);

        if (pathQueue != null && pathQueue.Count > 0)
        {
            Vector3 previous = transform.position;
            foreach (var point in pathQueue)
            {
                Gizmos.DrawLine(previous, point);
                Gizmos.DrawWireSphere(point, 0.08f);
                previous = point;
            }
        }
    }
}
