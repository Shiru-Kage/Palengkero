using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    [Header("Settings References")]
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private SettingsManager settings;
    [Header("Audio")]
    [SerializeField] private AudioClip pauseSound;

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
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
            PlayPauseSound();
        }
    }

    public void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        ApplyTimePause(true);
        isPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        settings.CloseAllPanels();
        ApplyTimePause(false);
        isPaused = false;
    }
    public void ApplyTimePause(bool pause)
    {
        Time.timeScale = pause ? 0f : 1f;

        if (pause)
        {
            inputActions.Player.Disable();
        }
        else
        {
            inputActions.Player.Enable();
        }
    }

    public bool IsPaused()
    {
        return isPaused;
    }

    private void PlayPauseSound()
    {
        if (pauseSound != null)
        {
            AudioManager.Instance.PlaySFX(pauseSound);
        }
    }
}
