using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;

    private InputSystem_Actions inputActions;
    private bool isPaused = false;

    private void Awake()
    {
        inputActions = InputManager.GetInputActions();
    }

    private void OnEnable()
    {
        inputActions.UI.Menu.performed += HandleMenu;
        inputActions.UI.Enable();
    }

    private void OnDisable()
    {
        inputActions.UI.Menu.performed -= HandleMenu;
        inputActions.UI.Disable();
    }

    private void HandleMenu(InputAction.CallbackContext context)
    {
        TogglePause();
    }

    public void TogglePause()
    {
        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    public void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;

        inputActions.Player.Disable();
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

        inputActions.Player.Enable();
    }

    public bool IsPaused()
    {
        return isPaused;
    }
}
