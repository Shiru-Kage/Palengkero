using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public List<CharacterProgressEntry> characterProgressData = new();  // Tracks all character progress data
    public int totalStars;  // You can calculate this based on all character progress if needed
}

[System.Serializable]
public class CharacterProgressEntry
{
    public string characterID;  // Unique character identifier
    public string characterName;  // Character name
    public int currentLevelIndex;  // The index of the level the character is currently at
    public bool[] unlockedLevels;  // Array to store unlocked levels (true/false per level)
    public List<int> levelStars = new List<int>();  // This stores the total stars for each level (sum of nutrition, satisfaction, savings)
    public List<int> nutritionStars = new List<int>();  // Stores nutrition stars per level
    public List<int> satisfactionStars = new List<int>();  // Stores satisfaction stars per level
    public List<int> savingsStars = new List<int>();  // Stores savings stars per level
}
