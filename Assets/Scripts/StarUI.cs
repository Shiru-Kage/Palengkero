using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StarUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image[] starImages;  // Array to hold the star image references
    [SerializeField] private Sprite fullStarSprite;  // Sprite for a full star
    [SerializeField] private Sprite emptyStarSprite;  // Sprite for an empty star
    [SerializeField] private TextMeshProUGUI totalStarsText;


    private void Start()
    {
        // Ensure the stars are displayed when the UI is first enabled
        UpdateStarsForSelectedLevel(LevelStateManager.Instance.CurrentLevelIndex);
        UpdateTotalStarsText();
    }

    // Method to update the stars based on the selected level index
    public void UpdateStarsForSelectedLevel(int levelIndex)
    {
        int stars = StarSystem.Instance.GetStarsForLevel(levelIndex);  // Get the stars for the current level

        // Loop through the star images and assign the appropriate sprite
        for (int i = 0; i < starImages.Length; i++)
        {
            if (i < stars)
            {
                // Set the full star sprite for awarded stars
                starImages[i].sprite = fullStarSprite;
            }
            else
            {
                // Set the empty star sprite for unearned stars
                starImages[i].sprite = emptyStarSprite;
            }

            // Optionally, you can set the stars' alpha to create a fade effect
            starImages[i].color = new Color(1f, 1f, 1f, 1f);  // Full opacity for visible stars
        }
    }

    public void UpdateTotalStarsText()
    {
        int totalStars = StarSystem.Instance.GetTotalStars();  // Get the total stars from the manager
        int maxStars = LevelStateManager.Instance.AllLevels.Length * 3;  // Max stars (3 per level)

        totalStarsText.text = $"{totalStars}/{maxStars}";
    }
}
