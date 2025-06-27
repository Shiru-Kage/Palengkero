using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Level Data", menuName = "Level/Level Data")]
public class LevelData : ScriptableObject
{
    public Sprite levelIcon;
    public string levelName;
    public int levelNumber;
    public List<CharacterObjective> objectivesPerCharacter = new List<CharacterObjective>();
    [TextArea(5, 10)]
    public string levelDescription;

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