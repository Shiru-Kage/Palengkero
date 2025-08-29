using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, ICharacterAnimatorData
{
    [SerializeField] private float moveSpeed = 5f;

    private Interactable currentInteractable;

    private InputSystem_Actions inputActions;
    private Vector2 moveInput;
    public Vector2 MoveInput => moveInput;

    private Transform cachedTransform;

    private Vector2Int currentGridPos;  
    private Vector2Int targetGridPos;
    private bool isMoving = false;      

    private void Awake()
    {
        inputActions = InputManager.GetInputActions(); 
        cachedTransform = transform;

        currentGridPos = PathfindingGrid.Instance.GetGridPosition(transform.position);
        targetGridPos = currentGridPos;
    }

    private void OnEnable()
    {
        if (inputActions != null)
        {
            inputActions.Player.Move.performed += HandleMove;
            inputActions.Player.Move.canceled += HandleMove;
            inputActions.Player.Interact.performed += HandleInteract;
        }
    }

    private void OnDisable()
    {
        if (inputActions != null)
        {
            inputActions.Player.Move.performed -= HandleMove;
            inputActions.Player.Move.canceled -= HandleMove;
            inputActions.Player.Interact.performed -= HandleInteract;
        }
    }

    private void HandleMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();

        if (input == Vector2.zero)
        {
            moveInput = Vector2.zero;
        }
        else
        {
            if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
            {
                moveInput = new Vector2(Mathf.Sign(input.x), 0f); 
            }
            else
            {
                moveInput = new Vector2(0f, Mathf.Sign(input.y));
            }
        }
    }


    private void HandleInteract(InputAction.CallbackContext context)
    {
        if (currentInteractable == null)
        {
            Debug.Log("No interactable in range.");
            return;
        }

        var interactable = currentInteractable;
        Transform origin = interactable.interactionTransform ?? interactable.transform;

        Vector2 center = (Vector2)origin.position + interactable.boxOffset;
        Vector2 halfSize = interactable.boxSize * 0.5f;

        Vector2 playerPos = transform.position;
        bool isInside = Mathf.Abs(playerPos.x - center.x) <= halfSize.x &&
                        Mathf.Abs(playerPos.y - center.y) <= halfSize.y;

        if (isInside)
        {
            interactable.Interact();
        }
        else
        {
            Debug.Log("Too far to interact.");
        }
    }

    private void Update()
    {
        if (moveInput != Vector2.zero && !isMoving)
        {
            Vector2Int direction = new Vector2Int((int)moveInput.x, (int)moveInput.y);

            targetGridPos = currentGridPos + direction;

            if (PathfindingGrid.Instance.IsWalkable(targetGridPos.x, targetGridPos.y))
            {
                isMoving = true;
            }
        }

        if (isMoving)
        {
            Vector3 targetWorldPos = PathfindingGrid.Instance.GetWorldPosition(targetGridPos.x, targetGridPos.y);
            cachedTransform.position = Vector3.MoveTowards(cachedTransform.position, targetWorldPos, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(cachedTransform.position, targetWorldPos) < 0.1f)
            {
                currentGridPos = targetGridPos; 
                isMoving = false;  
                SnapToGrid();      
            }
        }
    }

    private void SnapToGrid()
    {
        Vector3 targetWorldPos = PathfindingGrid.Instance.GetWorldPosition(currentGridPos.x, currentGridPos.y);
        cachedTransform.position = targetWorldPos;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Interactable interactable))
        {
            Debug.Log("Entered trigger with: " + interactable.gameObject.name);
            currentInteractable = interactable;

            var cooldown = other.GetComponent<StallCooldown>();
            if (cooldown != null && cooldown.isCoolingDown)
                return;

            // Special case for stalls (to remember player proximity)
            var stall = other.GetComponent<StallUI>();
            if (stall != null)
            {
                stall.SetHighlight(true);
                stall.isPlayerNearby = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out Interactable interactable) && interactable == currentInteractable)
        {
            var stall = other.GetComponent<StallUI>();
            if (stall != null)
            {
                stall.SetHighlight(false);
                stall.isPlayerNearby = false;
            }

            currentInteractable = null;
        }
    }
}
