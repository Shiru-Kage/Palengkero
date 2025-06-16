using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    private InputSystem_Actions inputActions;
    private Vector2 moveInput;
    public Vector2 MoveInput => moveInput;

    private void Awake()
    {
        inputActions = InputManager.GetInputActions();
    }

    private void OnEnable()
    {
        inputActions.Player.Move.performed += HandleMove;
        inputActions.Player.Move.canceled += HandleMove;
    }

    private void OnDisable()
    {
        inputActions.Player.Move.performed -= HandleMove;
        inputActions.Player.Move.canceled -= HandleMove;
    }

    private void HandleMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
            moveInput = new Vector2(input.x, 0f);
        else
            moveInput = new Vector2(0f, input.y);
    }

    private void Update()
    {
        transform.Translate(moveInput * moveSpeed * Time.deltaTime);
    }

}
