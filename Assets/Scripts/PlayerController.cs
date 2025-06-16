using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    private InputSystem_Actions inputActions;
    private Vector2 moveInput;

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
        moveInput = context.ReadValue<Vector2>();
    }

    private void Update()
    {
        transform.Translate(moveInput * moveSpeed * Time.deltaTime);
    }

}
