using UnityEngine;
using System.Collections.Generic;

public class NPC_Shopper : MonoBehaviour, ICharacterAnimatorData
{
    [SerializeField] private CharacterData npcData;
    public CharacterData Data => npcData;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float arrivalThreshold = 0.05f;

    [SerializeField] private float minIdleTime = 1f;
    [SerializeField] private float maxIdleTime = 3f;
    private float idleTimer = 0f;

    private Queue<Vector3> pathQueue = new Queue<Vector3>();
    private Vector3? currentTarget;
    private Transform cachedTransform;
    public Vector2 MoveInput { get; private set; }

    private bool isMovingAlongX = true;
    private bool isIdle = false;
    public bool IsIdle => isIdle;

    private bool isTargetLocked = false;

    public event System.Action OnIdleStarted;

    private void Awake()
    {
        cachedTransform = transform;
        RoamAround();
    }

    private void Update()
    {
        FollowPath();
    }

    public void SetTarget(Vector3 worldPos)
    {
        if (isTargetLocked) return;

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

    public bool HasTarget() => currentTarget.HasValue;

    private void FollowPath()
    {
        // Handle idle countdown first
        if (isIdle)
        {
            idleTimer -= Time.deltaTime;
            if (idleTimer <= 0f)
            {
                isIdle = false;
                RoamAround();
                LockTarget();
            }
            return;
        }

        if (pathQueue.Count == 0) return;

        currentTarget = pathQueue.Peek();
        Vector3 direction = currentTarget.Value - cachedTransform.position;
        MoveInput = direction.normalized;

        if (isMovingAlongX)
        {
            MoveInput = new Vector2(MoveInput.x, 0);

            if (Mathf.Abs(cachedTransform.position.x - currentTarget.Value.x) < arrivalThreshold)
            {
                isMovingAlongX = false;
            }
        }
        else
        {
            MoveInput = new Vector2(0, MoveInput.y);

            if (Mathf.Abs(cachedTransform.position.y - currentTarget.Value.y) < arrivalThreshold)
            {
                pathQueue.Dequeue();
                isMovingAlongX = true;

                if (pathQueue.Count == 0)
                {
                    idleTimer = Random.Range(minIdleTime, maxIdleTime);
                    isIdle = true;
                    OnIdleStarted?.Invoke();
                }
            }
        }

        cachedTransform.position = Vector3.MoveTowards(cachedTransform.position, currentTarget.Value, moveSpeed * Time.deltaTime);

        if (pathQueue.Count == 0)
        {
            MoveInput = Vector2.zero;
            currentTarget = null;
            UnlockTarget();
        }
    }

    public void RoamAround()
    {
        if (!isTargetLocked)
        {
            Vector2Int randomGridPos = GetRandomWalkableGridPosition();
            Vector3 targetPosition = PathfindingGrid.Instance.GetWorldPosition(randomGridPos.x, randomGridPos.y);
            SetTarget(targetPosition);
        }
    }

    private Vector2Int GetRandomWalkableGridPosition()
    {
        Vector2Int randomGridPos;
        bool foundWalkable = false;

        do
        {
            randomGridPos = new Vector2Int(
                Random.Range(0, PathfindingGrid.Instance.GetGridSize().x),
                Random.Range(0, PathfindingGrid.Instance.GetGridSize().y)
            );

            if (PathfindingGrid.Instance.IsWalkable(randomGridPos.x, randomGridPos.y))
            {
                foundWalkable = true;
            }
        } while (!foundWalkable);

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

    public void LockTarget() => isTargetLocked = true;
    public void UnlockTarget() => isTargetLocked = false;
}
