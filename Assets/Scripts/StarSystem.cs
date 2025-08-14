using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarSystem : MonoBehaviour
{
    public static StarSystem Instance { get; private set; }
    private Dictionary<int, int> levelStars = new Dictionary<int, int>();
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

    public void AssignStarsForLevel(int levelIndex, bool metNutrition, bool metSatisfaction, bool metSavings)
    {
        int stars = 0;

        if (metNutrition) stars++;
        if (metSatisfaction) stars++;
        if (metSavings) stars++;

        stars = Mathf.Clamp(stars, 0, MAX_STARS);

        if (levelStars.ContainsKey(levelIndex))
        {
            levelStars[levelIndex] = stars; 
        }
        else
        {
            levelStars.Add(levelIndex, stars);
        }
    }

    public void AssignStarsForLevel(int levelIndex, int stars)
    {
        stars = Mathf.Clamp(stars, 0, MAX_STARS);

        if (levelStars.ContainsKey(levelIndex))
        {
            levelStars[levelIndex] = stars;
        }
        else
        {
            levelStars.Add(levelIndex, stars);
        }

        Debug.Log($"Level {levelIndex} Stars (Loaded): {stars}");
    }

    public int GetStarsForLevel(int levelIndex)
    {
        return levelStars.ContainsKey(levelIndex) ? levelStars[levelIndex] : 0;
    }

    public int GetTotalStars()
    {
        int totalStars = 0;
        foreach (var stars in levelStars.Values)
        {
            totalStars += stars;
        }
        return totalStars;
    }

    public bool HasMaxStarsForLevel(int levelIndex)
    {
        return GetStarsForLevel(levelIndex) == MAX_STARS;
    }

    public void ResetStarsForAllLevels()
    {
        List<int> keys = new List<int>(levelStars.Keys); 
        foreach (int levelIndex in keys)
        {
            levelStars[levelIndex] = 0;
        }

        Debug.Log("All stars have been reset to 0.");
    }
}
