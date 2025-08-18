using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StarSystem : MonoBehaviour
{
    public static StarSystem Instance { get; private set; }

    // Dictionary to store the stars for each level for each character
    private Dictionary<string, Dictionary<int, LevelStars>> characterLevelStars = new Dictionary<string, Dictionary<int, LevelStars>>(); 

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
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateStarUI(CharacterSelectionManager.Instance.SelectedCharacterID);
    }

    // Structure to hold stars for each objective (nutrition, satisfaction, savings)
    public struct LevelStars
    {
        public int nutritionStars;
        public int satisfactionStars;
        public int savingsStars;
    }

    // Assign stars based on objectives met
    public void AssignStarsForLevel(int levelIndex, string characterID, bool metNutrition, bool metSatisfaction, bool metSavings)
    {
        LevelStars levelStars = new LevelStars();

        // Assign stars based on which objectives are met
        levelStars.nutritionStars = metNutrition ? 1 : 0;
        levelStars.satisfactionStars = metSatisfaction ? 1 : 0;
        levelStars.savingsStars = metSavings ? 1 : 0;

        // Ensure the total stars don't exceed the max of 3
        int totalStars = levelStars.nutritionStars + levelStars.satisfactionStars + levelStars.savingsStars;
        totalStars = Mathf.Clamp(totalStars, 0, MAX_STARS);

        // Store the stars for this level and character
        if (!characterLevelStars.ContainsKey(characterID))
        {
            characterLevelStars[characterID] = new Dictionary<int, LevelStars>();
        }

        characterLevelStars[characterID][levelIndex] = levelStars;

        UpdateStarUI(characterID);
    }

    private void UpdateStarUI(string characterID)
    {
        StarUI starUI = Object.FindAnyObjectByType<StarUI>();
        if (starUI == null)
        {
            return;
        }
        for (int i = 0; i < LevelStateManager.Instance.AllLevels.Length; i++)
        {
            starUI.UpdateStarsForSelectedLevel(i, characterID);
        }
        starUI.UpdateTotalStarsText(characterID);
    }

    // Get the stars for a specific level
    public LevelStars GetStarsForLevel(int levelIndex, string characterID)
    {
        if (characterLevelStars.ContainsKey(characterID) && characterLevelStars[characterID].ContainsKey(levelIndex))
        {
            return characterLevelStars[characterID][levelIndex];
        }
        return new LevelStars();  // Default to no stars
    }

    // Get the total stars for a character (sum of all levels)
    public int GetTotalStarsForCharacter(string characterID)
    {
        int totalStars = 0;
        if (characterLevelStars.ContainsKey(characterID))
        {
            foreach (var levelStars in characterLevelStars[characterID].Values)
            {
                totalStars += levelStars.nutritionStars + levelStars.satisfactionStars + levelStars.savingsStars;
            }
        }
        return totalStars;
    }

    // Check if a specific objective has been met for a level
    public bool HasNutritionStarForLevel(int levelIndex, string characterID)
    {
        return GetStarsForLevel(levelIndex, characterID).nutritionStars > 0;
    }

    public bool HasSatisfactionStarForLevel(int levelIndex, string characterID)
    {
        return GetStarsForLevel(levelIndex, characterID).satisfactionStars > 0;
    }

    public bool HasSavingsStarForLevel(int levelIndex, string characterID)
    {
        return GetStarsForLevel(levelIndex, characterID).savingsStars > 0;
    }

    // Reset stars for all levels for a specific character
    public void ResetStarsForCharacter(string characterID)
    {
        if (characterLevelStars.ContainsKey(characterID))
        {
            characterLevelStars[characterID].Clear();
            Debug.Log($"Stars reset for character: {characterID}");
        }
    }

    public void ResetStarsForAllLevels()
    {
        characterLevelStars.Clear();
        Debug.Log("All stars have been reset for all characters.");
    }
}
