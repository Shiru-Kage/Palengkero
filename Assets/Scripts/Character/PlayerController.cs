using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
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

        // Axis prioritization (horizontal vs vertical)
        moveInput = Mathf.Abs(input.x) > Mathf.Abs(input.y)
            ? new Vector2(input.x, 0f)
            : new Vector2(0f, input.y);
    }

    private void HandleInteract(InputAction.CallbackContext context)
    {
        if (currentInteractable != null)
        {
            currentInteractable.Interact();
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
            currentInteractable = interactable;

            if (interactable is Stall stall)
                stall.SetBlinking(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out Interactable interactable) && interactable == currentInteractable)
        {
            if (interactable is Stall stall)
                stall.SetBlinking(false);

            currentInteractable = null;
        }
    }
}
