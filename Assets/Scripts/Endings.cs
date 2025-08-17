using UnityEngine;
using UnityEngine.Video;
using System.Linq;

public class Endings : MonoBehaviour
{
    // Reference to the VideoPlayer component to play the ending cutscene
    public VideoPlayer videoPlayer;

    private string selectedCharacterID;
    private int totalStars;
    private int[] levelStars; // Array of level stars for the selected character
    private bool[] nutritionStars; // Track nutrition stars for each level
    private bool[] satisfactionStars; // Track satisfaction stars for each level
    private bool[] savingsStars; // Track savings stars for each level
    private CharacterData selectedCharacterData;

    void Start()
    {
        // Assume selectedCharacterID is set elsewhere (e.g., from CharacterSelectionManager)
        selectedCharacterID = CharacterSelectionManager.Instance.SelectedCharacterID;

        // Get the CharacterData for the selected character
        selectedCharacterData = CharacterSelectionManager.Instance.GetPrefabByCharacterID(selectedCharacterID)?.GetComponent<PlayerData>().Data;

        if (selectedCharacterData == null)
        {
            Debug.LogError("Character data not found!");
            return;
        }

        // Get the total stars and level-specific stars for the selected character
        totalStars = StarSystem.Instance.GetTotalStarsForCharacter(selectedCharacterID);
        levelStars = new int[LevelStateManager.Instance.AllLevels.Length];
        nutritionStars = new bool[LevelStateManager.Instance.AllLevels.Length];
        satisfactionStars = new bool[LevelStateManager.Instance.AllLevels.Length];
        savingsStars = new bool[LevelStateManager.Instance.AllLevels.Length];

        for (int i = 0; i < LevelStateManager.Instance.AllLevels.Length; i++)
        {
            levelStars[i] = StarSystem.Instance.GetStarsForLevel(i, selectedCharacterID);
            // Assuming nutritionStars, satisfactionStars, and savingsStars are set based on level-specific star logic
            // You might have to adjust these based on your game logic (e.g., how stars are awarded for each category).
            nutritionStars[i] = levelStars[i] > 0; // Assuming 1st star represents Nutrition
            satisfactionStars[i] = levelStars[i] > 1; // Assuming 2nd star represents Satisfaction
            savingsStars[i] = levelStars[i] == 3; // Assuming 3rd star represents Savings
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
        // Check if total stars are between 7 and 14 and no levels have 0 stars
        if (totalStars >= 7 && totalStars <= 14 && levelStars.Count(stars => stars == 0) == 0)
        {
            return true;
        }
        return false;
    }

    bool IsStrugglingSpender()
    {
        // Check if total stars are 0–6 or has 0-star performance in 3+ levels
        if (totalStars <= 6 || levelStars.Count(stars => stars == 0) >= 3 || levelStars.Where((stars, index) => !HasNutritionAndSavingsStars(index)).Count() >= 3)
        {
            return true;
        }
        return false;
    }

    bool IsOverworkedMalnourished()
    {
        // Check if the player earned Savings star in 4+ levels and Nutrition star in ≤ 1 level
        if (savingsStars.Count(stars => stars) >= 4 && nutritionStars.Count(stars => stars) <= 1)
        {
            return true;
        }
        return false;
    }

    bool AllLevelsMetFullCriteria()
    {
        // Check if all levels meet Nutrition, Satisfaction, and Savings stars (i.e., 3 stars per level)
        for (int i = 0; i < levelStars.Length; i++)
        {
            if (levelStars[i] != 3)
                return false;
        }
        return true;
    }

    bool HasNutritionAndSavingsStars(int levelIndex)
    {
        // Check if both Nutrition and Savings stars are met for a level (Nutrition is 1, Savings is 3)
        return nutritionStars[levelIndex] && savingsStars[levelIndex];
    }

    void PlayEnding(string endingName)
    {
        // Play the corresponding ending video
        if (selectedCharacterData.endingCutscenes != null && selectedCharacterData.endingCutscenes.Length > 0)
        {
            // Assuming each ending video is assigned sequentially, and "endingName" matches with an index
            int endingIndex = GetEndingIndex(endingName);
            if (endingIndex >= 0 && endingIndex < selectedCharacterData.endingCutscenes.Length)
            {
                VideoClip clip = selectedCharacterData.endingCutscenes[endingIndex];
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
        else
        {
            Debug.LogError("No ending cutscenes available for this character!");
        }
    }

    int GetEndingIndex(string endingName)
    {
        // Assuming you have an index for each ending, use the endingName to determine the index
        switch (endingName)
        {
            case "SmartSaverEnding": return 0;
            case "BareMinimumSurvivorEnding": return 1;
            case "StrugglingSpenderEnding": return 2;
            case "OverworkedMalnourishedEnding": return 3;
            case "DefaultEnding": return 4;
            default: return -1; // Invalid ending name
        }
    }
}
