using System.Security.AccessControl;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Level Data", menuName = "Level/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Level Settings")]
    public Sprite levelIcon;
    public GameObject levelTileMap;
    public AudioClip backgroundMusic;
    public string levelName;
    public int levelNumber;
    public int numberOfStalls = 1;
    public bool levelCleared;
    [TextArea(5, 10)]
    public string levelDescription;
    public List<CharacterObjective> objectivesPerCharacter = new List<CharacterObjective>();

    [Header("Level NPC Settings")]
    public int minNPCToSpawn = 1;
    public int maxNPCToSpawn = 5;
    public int NPCperLevel = 10;
    public float minSpawnInterval = 10f;
    public float maxSpawnInterval = 15f;
    public int[] npcTypeLikelihoods;
    public GameObject[] typeOfNPCs;

    [Header("Stall Settings")]
    [Tooltip("Minimum stock of items available in the stall.")]
    public int minStallItemStock = 1;
    [Tooltip("Maximum stock of items available in the stall.")]
    public int maxStallItemStock = 5;

    [Header("Background Settings")]
    public BackgroundType backgroundType;

    public CharacterObjective GetObjectiveFor(CharacterData characterData)
    {
        return objectivesPerCharacter.Find(obj => obj.character == characterData);
    }
}

[System.Serializable]
public class CharacterObjective
{
    public CharacterData character;
    public int nutritionGoal;
    public int satisfactionGoal;
    public int savingsGoal;

    public float levelSavings = 0f; 
}

public enum BackgroundType
{
    Morning,
    Afternoon,
    Night
}