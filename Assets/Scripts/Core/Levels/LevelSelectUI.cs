using UnityEngine;
using UnityEngine.UI;

public class LevelSelectUI : MonoBehaviour
{
    public static LevelSelectUI Instance { get; private set; }

    [Header("Level Buttons")]
    [SerializeField] private Button[] levelButtons;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        UpdateLevelButtons();
    }

    public void UpdateLevelButtons()
    {
        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (LevelStateManager.Instance.IsLevelUnlocked(i))
            {
                levelButtons[i].interactable = true; 
            }
            else
            {
                levelButtons[i].interactable = false; 
            }
        }
    }

    public void RefreshLevelButtons()
    {
        UpdateLevelButtons();
    }

    public void UpdateButtonsAfterLevelUnlock()
    {
        UpdateLevelButtons();
    }
}
