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
        else if (IsOverSpender())  
        {
            Debug.Log("Playing 'Struggling Spender' Ending");
            PlayEnding("OverSpenderEnding");
        }
        else if (IsOverworkedMalnourished()) 
        {
            Debug.Log("Playing 'Overworked & Malnourished' Ending");
            PlayEnding("OverworkedMalnourishedEnding");

        }
        else if (IsBareMinimumSurvivor())
        {
            Debug.Log("Playing 'Bare Minimum Survivor' Ending");
            PlayEnding("BareMinimumSurvivorEnding");
        }
    }

    bool IsSmartSaver()
    {
        return totalStars == 15 && levelStars.All(stars => stars.nutritionStars == 1 && stars.satisfactionStars == 1 && stars.savingsStars == 1);
    }

    bool IsBareMinimumSurvivor()
    {
        return totalStars >= 6 && totalStars <= 14;
    }
    bool IsOverSpender()
    {
        return totalStars <= 10 &&
            levelStars.All(stars => stars.savingsStars == 0);
    }
    bool IsOverworkedMalnourished()
    {
        bool hasSufficientSavings = levelStars.Any(stars => stars.savingsStars >= 1);
        bool hasNeglectedNutrition = levelStars.All(stars => stars.nutritionStars <= 1);
        bool hasNeglectedSatisfaction = levelStars.All(stars => stars.satisfactionStars <= 1);

        return hasSufficientSavings && hasNeglectedNutrition && hasNeglectedSatisfaction;
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
            case "OverSpenderEnding": return 2;
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
