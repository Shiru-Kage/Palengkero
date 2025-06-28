using UnityEngine;
using System.Collections.Generic;

public class NPC_Shopper : MonoBehaviour, ICharacterAnimatorData
{
    [SerializeField] private NPCData npcData;
    public NPCData Data => npcData;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float arrivalThreshold = 0.05f;

    // Idle time range
    [SerializeField] private float minIdleTime = 1f;
    [SerializeField] private float maxIdleTime = 3f;
    private float idleTimer = 0f;

    private Queue<Vector3> pathQueue = new Queue<Vector3>();
    private Vector3? currentTarget;
    private Transform cachedTransform;
    public Vector2 MoveInput { get; private set; }

    private bool isMovingAlongX = true;
    private bool isIdle = false; // Flag to check if the NPC is in idle state

    private void Awake()
    {
        cachedTransform = transform;
    }

    private void Update()
    {
        FollowPath();
    }

    // Set a new target and calculate the path to it
    public void SetTarget(Vector3 worldPos)
    {
        List<Vector3> path = Pathfinder.FindPath(transform.position, worldPos);

        if (path != null && path.Count > 0)
        {
            pathQueue = new Queue<Vector3>(path);
            currentTarget = pathQueue.Dequeue();  // Start moving to the first point in the path
        }
        else
        {
            Debug.LogWarning($"{name} could not find path to {worldPos}");
        }
    }

    public bool HasTarget() => currentTarget.HasValue;

    private void FollowPath()
    {
        if (pathQueue.Count == 0) return; // No path to follow

        if (isIdle)
        {
            // NPC is idling, count down the idle timer
            idleTimer -= Time.deltaTime;
            if (idleTimer <= 0f)
            {
                // End idle state, start roaming again
                isIdle = false;
                RoamAround();
            }
            return; // Don't proceed with moving
        }

        currentTarget = pathQueue.Peek(); // Look at the next target without dequeuing it

        Vector3 direction = currentTarget.Value - cachedTransform.position;
        MoveInput = direction.normalized;

        if (isMovingAlongX)
        {
            MoveInput = new Vector2(MoveInput.x, 0); // Only move along the X-axis

            // If close to the X target, switch to Y movement
            if (Mathf.Abs(cachedTransform.position.x - currentTarget.Value.x) < arrivalThreshold)
            {
                isMovingAlongX = false;
            }
        }
        else // Moving along Y axis
        {
            MoveInput = new Vector2(0, MoveInput.y); // Only move along the Y-axis

            // If close to the Y target, dequeue and move to the next target
            if (Mathf.Abs(cachedTransform.position.y - currentTarget.Value.y) < arrivalThreshold)
            {
                pathQueue.Dequeue();
                isMovingAlongX = true; // Switch back to X movement

                // If we reached the last target, start idle time
                if (pathQueue.Count == 0)
                {
                    idleTimer = Random.Range(minIdleTime, maxIdleTime); // Set idle time
                    isIdle = true; // Start idle state
                }
            }
        }

        // Move towards the target (in either X or Y direction)
        cachedTransform.position = Vector3.MoveTowards(cachedTransform.position, currentTarget.Value, moveSpeed * Time.deltaTime);

        // If we're done with the path, stop movement
        if (pathQueue.Count == 0)
        {
            MoveInput = Vector2.zero;
            currentTarget = null;
        }
    }

    // This method will pick a new random location after idle time ends.
    private void RoamAround()
    {
        Vector2Int randomGridPos = GetRandomWalkableGridPosition();
        Vector3 targetPosition = PathfindingGrid.Instance.GetWorldPosition(randomGridPos.x, randomGridPos.y);
        SetTarget(targetPosition);  // Delegate target setting to NPC_Shopper
    }

    private Vector2Int GetRandomWalkableGridPosition()
    {
        Vector2Int randomGridPos = new Vector2Int(0, 0);
        bool foundWalkable = false;

        // Keep generating random grid positions until we find one that's walkable
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

    public bool IsAtTarget()
    {
        return currentTarget.HasValue && Vector3.Distance(cachedTransform.position, currentTarget.Value) < arrivalThreshold;
    }

    private void OnDrawGizmos()
    {
        if (pathQueue.Count > 0)
        {
            Gizmos.color = Color.cyan;
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
