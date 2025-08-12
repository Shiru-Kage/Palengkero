using System.Security.AccessControl;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Level Data", menuName = "Level/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Level Settings")]
    public Sprite levelIcon;
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
    public float minSpawnInterval = 10f;
    public float maxSpawnInterval = 15f;

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
}