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

    private void Awake()
    {
        inputActions = InputManager.GetInputActions(); // global/static access
        cachedTransform = transform;
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

        moveInput = Mathf.Abs(input.x) > Mathf.Abs(input.y)
            ? new Vector2(input.x, 0f)
            : new Vector2(0f, input.y);
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
        cachedTransform.Translate(moveInput * moveSpeed * Time.deltaTime);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Interactable interactable))
        {
            Debug.Log("Entered trigger with: " + interactable.gameObject.name);
            currentInteractable = interactable;

            var stall = other.GetComponent<StallUI>();
            if (stall != null)
            {
                StallCooldown cooldown = stall.GetComponent<StallCooldown>();
                if (cooldown != null && !cooldown.isCoolingDown)
                {
                    stall.SetBlinking(true);
                    stall.isPlayerNearby = true;
                }
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
                stall.SetBlinking(false);
                stall.isPlayerNearby = false;
            }

            currentInteractable = null;
        }
    }
}
