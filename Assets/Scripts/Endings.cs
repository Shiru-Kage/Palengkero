using UnityEngine;
using UnityEngine.Video;
using System.Linq;

public class Endings : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private GameObject statistics;

    private string selectedCharacterID;
    private int totalStars;
    private StarSystem.LevelStars[] levelStars;
    private CharacterData selectedCharacterData;
    private Character_Cutscenes cutscenes;

    void Start()
    {
        selectedCharacterID = CharacterSelectionManager.Instance.SelectedCharacterID;
        selectedCharacterData = CharacterSelectionManager.Instance
            .GetPrefabByCharacterID(selectedCharacterID)
            ?.GetComponent<PlayerData>().Data;

        if (selectedCharacterData == null)
        {
            Debug.LogError("Character data not found!");
            return;
        }

        cutscenes = selectedCharacterData.cutscene;

        // Get stars
        totalStars = StarSystem.Instance.GetTotalStarsForCharacter(selectedCharacterID);
        levelStars = new StarSystem.LevelStars[LevelStateManager.Instance.AllLevels.Length];

        for (int i = 0; i < LevelStateManager.Instance.AllLevels.Length; i++)
        {
            levelStars[i] = StarSystem.Instance.GetStarsForLevel(i, selectedCharacterID);
        }

        // Play ending if completed
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
        return levelStars.All(stars =>
            stars.nutritionStars > 0 ||
            stars.satisfactionStars > 0 ||
            stars.savingsStars > 0
        );
    }

    void PlayEndingBasedOnStars()
    {
        if (IsSmartSaver())
        {
            Debug.Log("Playing 'Smart Saver' Ending");
            PlayEnding(EndingType.SmartSaver);
        }
        else if (IsOverSpender())
        {
            Debug.Log("Playing 'Over Spender' Ending");
            PlayEnding(EndingType.OverSpender);
        }
        else if (IsOverworkedMalnourished())
        {
            Debug.Log("Playing 'Overworked & Malnourished' Ending");
            PlayEnding(EndingType.OverworkedMalnourished);
        }
        else if (IsBareMinimumSurvivor())
        {
            Debug.Log("Playing 'Bare Minimum Survivor' Ending");
            PlayEnding(EndingType.BareMinimumSurvivor);
        }
    }

    bool IsSmartSaver()
    {
        return totalStars == 15 &&
            levelStars.All(stars =>
                stars.nutritionStars == 1 &&
                stars.satisfactionStars == 1 &&
                stars.savingsStars == 1
            );
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

    void PlayEnding(EndingType type)
    {
        if (cutscenes == null || cutscenes.endingCutscenes == null) return;

        var ending = cutscenes.endingCutscenes.FirstOrDefault(e => e.endingType == type);

        if (ending != null && ending.cutsceneVideo != null)
        {
            videoPlayer.clip = ending.cutsceneVideo;
            videoPlayer.Play();

            // ✅ Mark as unlocked in Archives
            ArchiveManager.Instance.UnlockCutscene(selectedCharacterData.characterName, ending.cutsceneName);
        }
        else
        {
            Debug.LogError($"Ending '{type}' not found for {selectedCharacterData.characterName}!");
        }
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        videoPlayer.gameObject.SetActive(false);
        statistics.gameObject.SetActive(true);
    }
}
