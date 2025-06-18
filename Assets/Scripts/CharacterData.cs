using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class LevelObjective
{
    public int weeklyBudget;
    public int savingsGoal;
}

[CreateAssetMenu(fileName = "New Character Data", menuName = "Character/Character Data")]
public class CharacterData : ScriptableObject
{
    public string characterName;
    public string characterIndustry;
    public int characterAge;
    public string characterMonthlyIncome;
    public int characterWeeklyBudget;
    public Sprite characterSprite;
    [TextArea(3, 10)]
    public string characterDescription;

    public List<LevelObjective> levelObjectives = new List<LevelObjective>();
}
