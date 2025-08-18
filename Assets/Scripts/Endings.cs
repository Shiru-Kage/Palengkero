using UnityEngine;
using UnityEngine.Video;
using System.Linq;

public class Endings : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private GameObject statistics;

    private string selectedCharacterID;
    private int totalStars;
    private StarSystem.LevelStars[] levelStars; // Changed to store LevelStars struct
    private CharacterData selectedCharacterData;

    void Start()
    {
        selectedCharacterID = CharacterSelectionManager.Instance.SelectedCharacterID;
        selectedCharacterData = CharacterSelectionManager.Instance.GetPrefabByCharacterID(selectedCharacterID)?.GetComponent<PlayerData>().Data;

        if (selectedCharacterData == null)
        {
            Debug.LogError("Character data not found!");
            return;
        }

        // Get the total stars and level-specific stars for the selected character
        totalStars = StarSystem.Instance.GetTotalStarsForCharacter(selectedCharacterID);
        levelStars = new StarSystem.LevelStars[LevelStateManager.Instance.AllLevels.Length];

        for (int i = 0; i < LevelStateManager.Instance.AllLevels.Length; i++)
        {
            levelStars[i] = StarSystem.Instance.GetStarsForLevel(i, selectedCharacterID); // Fetch LevelStars for each level
        }

        // Check if all levels are completed
        if (AreAllLevelsCompleted())
        {
            PlayEndingBasedOnStars();
        }
        else
        {
            Debug.Log("All levels are not completed yet. No ending will be played.");
        }

        videoPlayer.loopPointReached += OnVideoEnd;
    }

    bool AreAllLevelsCompleted()
    {
        // Check if the player has completed all levels by having at least 1 star per level
        return levelStars.All(stars => stars.nutritionStars > 0 || stars.satisfactionStars > 0 || stars.savingsStars > 0);
    }

    void PlayEndingBasedOnStars()
    {
        if (IsSmartSaver())
        {
            Debug.Log("Playing 'Smart Saver' Ending");
            PlayEnding("SmartSaverEnding");
        }
        else if (IsBareMinimumSurvivor())
        {
            Debug.Log("Playing 'Bare Minimum Survivor' Ending");
            PlayEnding("BareMinimumSurvivorEnding");
        }
        else if (IsStrugglingSpender())
        {
            Debug.Log("Playing 'Struggling Spender' Ending");
            PlayEnding("StrugglingSpenderEnding");
        }
        else if (IsOverworkedMalnourished())
        {
            Debug.Log("Playing 'Overworked & Malnourished' Ending");
            PlayEnding("OverworkedMalnourishedEnding");
        }
    }

    bool IsSmartSaver()
    {
        // Check if the player earned 15 stars, with full stars in all categories (Nutrition, Satisfaction, Savings)
        return totalStars == 15 && levelStars.All(stars => stars.nutritionStars == 1 && stars.satisfactionStars == 1 && stars.savingsStars == 1);
    }

    bool IsBareMinimumSurvivor()
    {
        // Check if total stars are between 7 and 14 and no levels have 0 stars
        return totalStars >= 7 && totalStars <= 14 && levelStars.Count(stars => stars.nutritionStars + stars.satisfactionStars + stars.savingsStars == 0) == 0;
    }

    bool IsStrugglingSpender()
    {
        // Check if total stars are 0–6 or has 0-star performance in 3+ levels
        return totalStars <= 6 || levelStars.Count(stars => stars.nutritionStars + stars.satisfactionStars + stars.savingsStars == 0) >= 3;
    }

    bool IsOverworkedMalnourished()
    {
        // Check if the player earned Savings star in 4+ levels and Nutrition star in ≤ 1 level
        return levelStars.Count(stars => stars.savingsStars > 0) >= 4 && levelStars.Count(stars => stars.nutritionStars > 0) <= 1;
    }

    void PlayEnding(string endingName)
    {
        if (selectedCharacterData.endingCutscenes != null && selectedCharacterData.endingCutscenes.Length > 0)
        {
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
        switch (endingName)
        {
            case "SmartSaverEnding": return 0;
            case "BareMinimumSurvivorEnding": return 1;
            case "StrugglingSpenderEnding": return 2;
            case "OverworkedMalnourishedEnding": return 3;
            default: return -1;
        }
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        videoPlayer.gameObject.SetActive(false);
        statistics.gameObject.SetActive(true);
    }
}
