using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarSystem : MonoBehaviour
{
    public static StarSystem Instance { get; private set; }
    private Dictionary<string, Dictionary<int, int>> characterLevelStars = new Dictionary<string, Dictionary<int, int>>(); 
    private const int MAX_STARS = 3;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
    }

    public void AssignStarsForLevel(int levelIndex, string characterID, bool metNutrition, bool metSatisfaction, bool metSavings)
    {
        int stars = 0;

        if (metNutrition) stars++;
        if (metSatisfaction) stars++;
        if (metSavings) stars++;

        stars = Mathf.Clamp(stars, 0, MAX_STARS);

        if (!characterLevelStars.ContainsKey(characterID))
        {
            characterLevelStars[characterID] = new Dictionary<int, int>();
        }

        characterLevelStars[characterID][levelIndex] = stars;

        UpdateStarUI(characterID);
    }

    public void AssignStarsForLevel(int levelIndex, string characterID, int stars)
    {
        stars = Mathf.Clamp(stars, 0, MAX_STARS);

        if (!characterLevelStars.ContainsKey(characterID))
        {
            characterLevelStars[characterID] = new Dictionary<int, int>();
        }

        characterLevelStars[characterID][levelIndex] = stars;

        UpdateStarUI(characterID);
    }

    private void UpdateStarUI(string characterID)
    {
        StarUI starUI = Object.FindAnyObjectByType<StarUI>(); 
        if (starUI != null)
        {
            for (int i = 0; i < LevelStateManager.Instance.AllLevels.Length; i++)
            {
                starUI.UpdateStarsForSelectedLevel(i, characterID);
            }
            starUI.UpdateTotalStarsText(characterID); 
        }
    }

    public int GetStarsForLevel(int levelIndex, string characterID)
    {
        if (characterLevelStars.ContainsKey(characterID) && characterLevelStars[characterID].ContainsKey(levelIndex))
        {
            return characterLevelStars[characterID][levelIndex];
        }
        return 0;
    }

    public int GetTotalStarsForCharacter(string characterID)
    {
        int totalStars = 0;
        if (characterLevelStars.ContainsKey(characterID))
        {
            foreach (var stars in characterLevelStars[characterID].Values)
            {
                totalStars += stars;
            }
        }
        return totalStars;
    }

    public bool HasMaxStarsForLevel(int levelIndex, string characterID)
    {
        return GetStarsForLevel(levelIndex, characterID) == MAX_STARS;
    }

    public void ResetStarsForAllLevels()
    {
        characterLevelStars.Clear();
        Debug.Log("All stars have been reset for all characters.");
    }

    public void ResetStarsForCharacter(string characterID)
    {
        if (characterLevelStars.ContainsKey(characterID))
        {
            characterLevelStars[characterID].Clear();
            Debug.Log($"Stars reset for character: {characterID}");
        }
    }
}
