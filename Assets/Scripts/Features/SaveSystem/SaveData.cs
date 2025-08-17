using UnityEngine;
using System.Collections.Generic;


[System.Serializable]
public class SaveData
{
    public List<CharacterProgressEntry> characterProgressData = new();
    public int totalStars;
}

[System.Serializable]
public class CharacterProgressEntry
{
    public string characterID; 
    public string characterName; 
    public int currentLevelIndex;
    public bool[] unlockedLevels;
    public List<int> levelStars = new List<int>(); 
}