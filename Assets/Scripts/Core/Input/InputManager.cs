using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
public class InputManager : MonoBehaviour
{
    [SerializeField] private InputEvents events;
    public InputEvents Events { get { return events; } }
    private static InputSystem_Actions inputActions;
    [System.Serializable]
    public struct InputEvents
    {
        public UnityEvent<Vector2> OnTap;
    }
    private void Awake()
    {
        if (inputActions == null)
        {
            inputActions = new InputSystem_Actions();
            inputActions.Enable();
        }
    }
    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    public static InputSystem_Actions GetInputActions()
    {
        if (inputActions == null)
        {
            inputActions = new InputSystem_Actions();
            inputActions.Enable();
        }
        return inputActions;
    }
    private void Start()
    {
        inputActions.UI.LocationTap.started += _ => OnClick();
    }

    private void OnClick()
    {
        Vector2 screenPosition = inputActions.UI.LocationTap.ReadValue<Vector2>();
        events.OnTap.Invoke(screenPosition);
    }
    
}
