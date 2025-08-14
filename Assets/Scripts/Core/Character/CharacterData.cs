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
    public Sprite characterSprite;
    public Sprite characterFrameSprite;
    [TextArea(3, 10)]
    public string characterDescription;
    public Color textColor;
    public AudioClip voiceClip;
    public RuntimeAnimatorController characterAnimator;
    public VideoClip cutsceneVideo; 
}
