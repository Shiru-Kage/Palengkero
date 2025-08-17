using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StarUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image[] starImages; 
    [SerializeField] private Sprite fullStarSprite;  
    [SerializeField] private Sprite emptyStarSprite;  
    [SerializeField] private TextMeshProUGUI totalStarsText;


    private void Start()
    {
        UpdateStarsForSelectedLevel(LevelStateManager.Instance.CurrentLevelIndex, CharacterSelectionManager.Instance.SelectedCharacterID);
        UpdateTotalStarsText(CharacterSelectionManager.Instance.SelectedCharacterID);
    }

    public void UpdateStarsForSelectedLevel(int levelIndex, string characterID)
    {
        int stars = StarSystem.Instance.GetStarsForLevel(levelIndex, characterID); 

        for (int i = 0; i < starImages.Length; i++)
        {
            if (i < stars)
            {
                starImages[i].sprite = fullStarSprite;
            }
            else
            {
                starImages[i].sprite = emptyStarSprite;
            }

            starImages[i].color = new Color(1f, 1f, 1f, 1f); 
        }
    }

    public void UpdateTotalStarsText(string characterID)
    {
        int totalStars = 0;
        int maxStars = 0;

        for (int i = 0; i < LevelStateManager.Instance.AllLevels.Length; i++)
        {
            totalStars += StarSystem.Instance.GetStarsForLevel(i, characterID);  
            maxStars += 3; 
        }

        if (totalStarsText != null)
        {
            totalStarsText.text = $"{totalStars}/{maxStars}";
        }
    }
}
