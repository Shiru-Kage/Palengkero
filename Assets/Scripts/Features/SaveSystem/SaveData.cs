using UnityEngine;
using System.Collections.Generic;


[System.Serializable]
public class SaveData
{
    public string characterID;
    public List<CharacterProgressEntry> characterProgressData = new();
}

[System.Serializable]
public class CharacterProgressEntry
{
    public string characterName;
    public int currentLevelIndex;
    public bool[] unlockedLevels;
}
