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
    [SerializeField] private Animator characterAnimator;
    private string endingName;


    private float originalMusicVolume;
    private float originalSFXVolume;

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

        totalStars = StarSystem.Instance.GetTotalStarsForCharacter(selectedCharacterID);
        levelStars = new StarSystem.LevelStars[LevelStateManager.Instance.AllLevels.Length];

        for (int i = 0; i < LevelStateManager.Instance.AllLevels.Length; i++)
        {
            levelStars[i] = StarSystem.Instance.GetStarsForLevel(i, selectedCharacterID);
        }

        if (AreAllLevelsCompleted())
        {
            PlayEndingBasedOnStars();
        }
        else
        {
            Debug.Log("All levels are not completed yet. No ending will be played.");
        }

        videoPlayer.loopPointReached += OnVideoEnd;
        if (characterAnimator != null && selectedCharacterData.characterAnimator != null)
        {
            characterAnimator.runtimeAnimatorController = selectedCharacterData.characterAnimator;
            characterAnimator.Play("Idle", 0);
        }
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
            endingName = "Smart Saver";
            PlayEnding(EndingType.SmartSaver);
        }
        else if (IsOverSpender())
        {
            Debug.Log("Playing 'Over Spender' Ending");
            endingName = "Over Spender";
            PlayEnding(EndingType.OverSpender);
        }
        else
        {
            bool isOverworkedMalnourished = IsOverworkedMalnourished();
            
            if (isOverworkedMalnourished)
            {
                Debug.Log("Playing 'Overworked & Malnourished' Ending");
                endingName = "Neglected Health"; 
                PlayEnding(EndingType.OverworkedMalnourished);
            }
            else if (IsBareMinimumSurvivor())
            {
                Debug.Log("Playing 'Bare Minimum Survivor' Ending");
                endingName = "Bare Minimum Survivor";
                PlayEnding(EndingType.BareMinimumSurvivor);
            }
        }
    }

    public string GetEndingName()
    {
        return endingName;
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
        return totalStars >= 6 && totalStars <= 14 &&
            levelStars.Any(stars => stars.nutritionStars > 0) &&
            levelStars.Any(stars => stars.satisfactionStars > 0) &&
            levelStars.Any(stars => stars.savingsStars > 0) &&
            !IsOverworkedMalnourished();
    }

    bool IsOverSpender()
    {
        return totalStars <= 10 && levelStars.All(stars => stars.savingsStars == 0);
    }

    bool IsOverworkedMalnourished()
    {
        int totalSavingsStars = 0;
        int totalNutritionStars = 0;
        int totalSatisfactionStars = 0;

        bool[] unlockedLevels = LevelStateManager.Instance.GetUnlockedLevelsForCurrentCharacter();

        for (int i = 0; i < levelStars.Length; i++)
        {
            totalSavingsStars += levelStars[i].savingsStars;
            totalNutritionStars += levelStars[i].nutritionStars;
            totalSatisfactionStars += levelStars[i].satisfactionStars;
        }

        bool hasSufficientSavings = totalSavingsStars >= 1;  
        bool hasNeglectedNutrition = totalNutritionStars <= 1; 
        bool hasNeglectedSatisfaction = totalSatisfactionStars <= 1; 

        return hasSufficientSavings && hasNeglectedNutrition && hasNeglectedSatisfaction;
    }


    void PlayEnding(EndingType type)
    {
        if (cutscenes == null || cutscenes.endingCutscenes == null) return;

        originalMusicVolume = AudioManager.Instance.musicVolume;
        originalSFXVolume = AudioManager.Instance.sfxVolume;

        AudioManager.Instance.SetMusicVolume(0.1f); 
        AudioManager.Instance.SetSFXVolume(0.1f);

        var ending = cutscenes.endingCutscenes.FirstOrDefault(e => e.endingType == type);

        if (ending != null && ending.cutsceneVideo != null)
        {
            videoPlayer.clip = ending.cutsceneVideo;
            videoPlayer.Play();

            ArchiveManager.Instance.UnlockCutscene(selectedCharacterData.characterName, ending.cutsceneName);
        }
        else
        {
            Debug.LogError($"Ending '{type}' not found for {selectedCharacterData.characterName}!");
        }
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        AudioManager.Instance.SetMusicVolume(originalMusicVolume);
        AudioManager.Instance.SetSFXVolume(originalSFXVolume);
        videoPlayer.gameObject.SetActive(false);
        statistics.gameObject.SetActive(true);
    }
}
