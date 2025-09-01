using UnityEngine;
using UnityEngine.Video;
[CreateAssetMenu(fileName = "New Character Data", menuName = "Character/Character Data")]
public class CharacterData : ScriptableObject
{
    public string characterID;
    public string characterName;
    public string characterIndustry;
    public string characterDifficulty;
    public int characterAge;
    public string characterMonthlyIncome;
    public int characterWeeklyBudget;
    public Sprite[] characterSprites;
    public Sprite characterSelectFrameSprite;
    [TextArea(3, 10)]
    public string characterDescription;
    public Color textColor;
    public AudioClip voiceClip;
    public RuntimeAnimatorController characterAnimator;
    public Character_Cutscenes cutscene;
    
    [Header("NPC Behavior Settings")]
    
    [Header("Preferences")]
    [HideInInspector]
    [Range(0, 100)]
    public int preferCheapItemsChance = 0;

    [HideInInspector]
    [Range(0, 100)]
    public int preferHighNutritionChance = 0;

    [HideInInspector]
    [Range(0, 100)]
    public int preferHighSatisfactionChance = 0;
    
    [Header("Buying likelihood")]
    [Range(0, 100)]
    public int buyLikelihood = 50;
}
