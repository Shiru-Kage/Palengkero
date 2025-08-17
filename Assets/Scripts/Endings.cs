using UnityEngine;
using UnityEngine.Video;
using System.Collections;
using System.Linq;

public class Endings : MonoBehaviour
{
    // Reference to the VideoPlayer component to play the ending cutscene
    public VideoPlayer videoPlayer;

    private string selectedCharacterID;
    private int totalStars;
    private int[] levelStars; // Array of level stars for the selected character

    void Start()
    {
        // Assume selectedCharacterID is set elsewhere (e.g., from CharacterSelectionManager)
        selectedCharacterID = CharacterSelectionManager.Instance.SelectedCharacterID;

        // Get the total stars and level-specific stars for the selected character
        totalStars = StarSystem.Instance.GetTotalStarsForCharacter(selectedCharacterID);
        levelStars = new int[LevelStateManager.Instance.AllLevels.Length];
        for (int i = 0; i < LevelStateManager.Instance.AllLevels.Length; i++)
        {
            levelStars[i] = StarSystem.Instance.GetStarsForLevel(i, selectedCharacterID);
        }

        // Call method to determine and play the ending
        PlayEndingBasedOnStars();
    }

    void PlayEndingBasedOnStars()
    {
        // Check for each ending condition based on total stars and the specific level star requirements

        if (IsSmartSaver())
        {
            Debug.Log("Playing 'Smart Saver' Ending");
            PlayEnding("SmartSaverEnding"); // Replace with actual video or animation
        }
        else if (IsBareMinimumSurvivor())
        {
            Debug.Log("Playing 'Bare Minimum Survivor' Ending");
            PlayEnding("BareMinimumSurvivorEnding"); // Replace with actual video or animation
        }
        else if (IsStrugglingSpender())
        {
            Debug.Log("Playing 'Struggling Spender' Ending");
            PlayEnding("StrugglingSpenderEnding"); // Replace with actual video or animation
        }
        else if (IsOverworkedMalnourished())
        {
            Debug.Log("Playing 'Overworked & Malnourished' Ending");
            PlayEnding("OverworkedMalnourishedEnding"); // Replace with actual video or animation
        }
        else
        {
            Debug.Log("Playing Default Ending");
            PlayEnding("DefaultEnding"); // Replace with a default ending video or animation
        }
    }

    bool IsSmartSaver()
    {
        // Check if the player earned 15 stars, with full stars in all categories (Nutrition, Satisfaction, Savings)
        if (totalStars == 15 && levelStars.All(stars => stars == 3) && AllLevelsMetFullCriteria())
        {
            return true;
        }
        return false;
    }

    bool IsBareMinimumSurvivor()
    {
        // Check if total stars are between 7 and 14 and no levels have 0 stars, but may have occasional 1 or 2 stars
        if (totalStars >= 7 && totalStars <= 14 && levelStars.Count(stars => stars == 0) == 0)
        {
            return true;
        }
        return false;
    }

    bool IsStrugglingSpender()
    {
        // Check if total stars are 0–6 or 0 stars in 3+ levels or missing both Nutrition and Savings stars in ≥ 3 levels
        if (totalStars <= 6 || levelStars.Count(stars => stars == 0) >= 3 || levelStars.Where((stars, index) => !HasNutritionAndSavingsStars(index)).Count() >= 3)
        {
            return true;
        }
        return false;
    }

    bool IsOverworkedMalnourished()
    {
        // Check if the player earned Savings star in 4+ levels and Nutrition star in ≤ 1 level
        if (levelStars.Count(stars => stars >= 1) >= 4 && levelStars.Count(stars => stars == 0) >= 4)
        {
            return true;
        }
        return false;
    }

    bool AllLevelsMetFullCriteria()
    {
        // Check if all levels meet Nutrition, Satisfaction, and Savings stars
        for (int i = 0; i < levelStars.Length; i++)
        {
            if (levelStars[i] != 3)
                return false;
        }
        return true;
    }

    bool HasNutritionAndSavingsStars(int levelIndex)
    {
        // Check if both Nutrition and Savings stars are met for a level
        // Assuming that star levels are 0 (none), 1 (Nutrition), 2 (Satisfaction), 3 (All)
        return levelStars[levelIndex] > 0;
    }

    void PlayEnding(string endingName)
    {
        // Play the corresponding ending video
        VideoClip clip = Resources.Load<VideoClip>($"Endings/{endingName}");
        if (clip != null && videoPlayer != null)
        {
            videoPlayer.clip = clip;
            videoPlayer.Play();
        }
        else
        {
            Debug.LogError($"Ending video {endingName} not found!");
        }
    }
}
