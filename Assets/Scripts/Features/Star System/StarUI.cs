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
        StarSystem.LevelStars levelStars = StarSystem.Instance.GetStarsForLevel(levelIndex, characterID); 

        // Reset all stars to empty
        for (int i = 0; i < starImages.Length; i++)
        {
            starImages[i].sprite = emptyStarSprite;
            starImages[i].color = new Color(1f, 1f, 1f, 1f);  // Full opacity
        }

        // Update the star images based on how many objectives are met (nutrition, satisfaction, savings)
        if (levelStars.nutritionStars > 0)
        {
            starImages[0].sprite = fullStarSprite;
        }
        if (levelStars.satisfactionStars > 0)
        {
            starImages[1].sprite = fullStarSprite;
        }
        if (levelStars.savingsStars > 0)
        {
            starImages[2].sprite = fullStarSprite;
        }
    }

    public void UpdateTotalStarsText(string characterID)
    {
        int totalStars = 0;
        int maxStars = 0;

        for (int i = 0; i < LevelStateManager.Instance.AllLevels.Length; i++)
        {
            StarSystem.LevelStars levelStars = StarSystem.Instance.GetStarsForLevel(i, characterID);  
            // Count how many stars are earned per level (nutrition + satisfaction + savings)
            totalStars += levelStars.nutritionStars + levelStars.satisfactionStars + levelStars.savingsStars;
            maxStars += 3;  // Max stars per level (nutrition, satisfaction, savings)
        }

        if (totalStarsText != null)
        {
            totalStarsText.text = $"{totalStars}/{maxStars}";
        }
    }
}

