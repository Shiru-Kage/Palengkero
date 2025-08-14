using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarSystem : MonoBehaviour
{
    public static StarSystem Instance { get; private set; }

    // Dictionary to store stars for each level index
    private Dictionary<int, int> levelStars = new Dictionary<int, int>();

    // The maximum number of stars that can be awarded for a level
    private const int MAX_STARS = 3;

    private void Awake()
    {
        // Implement singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Keep across scenes
        }
    }

    // Method to calculate and assign stars for a level based on objectives
    public void AssignStarsForLevel(int levelIndex, bool metNutrition, bool metSatisfaction, bool metSavings)
    {
        int stars = 0;

        // Count the number of completed objectives
        if (metNutrition) stars++;
        if (metSatisfaction) stars++;
        if (metSavings) stars++;

        // Ensure stars do not exceed the max (e.g., 3 stars max per level)
        stars = Mathf.Clamp(stars, 0, MAX_STARS);

        // Store the stars for this level
        if (levelStars.ContainsKey(levelIndex))
        {
            levelStars[levelIndex] = stars;  // Update if already exists
        }
        else
        {
            levelStars.Add(levelIndex, stars);
        }

        Debug.Log($"Level {levelIndex} Stars: {stars}");
    }

    // Method to get the stars for a particular level
    public int GetStarsForLevel(int levelIndex)
    {
        return levelStars.ContainsKey(levelIndex) ? levelStars[levelIndex] : 0;
    }

    // Method to check if the player has earned all stars for the level
    public bool HasMaxStarsForLevel(int levelIndex)
    {
        return GetStarsForLevel(levelIndex) == MAX_STARS;
    }
}
