using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "New Character Cutscene Data", menuName = "Character/Character Cutscene Data")]
public class Character_Cutscenes : ScriptableObject
{
    [Header("Opening")]
    public string openingCutsceneName;
    public VideoClip openingCutscene;
    public Sprite openingIcon;

    [Header("Endings")]
    public EndingCutscene[] endingCutscenes;
}

[System.Serializable]
public class EndingCutscene
{
    public EndingType endingType;
    public string cutsceneName;
    public VideoClip cutsceneVideo;
    public Sprite cutsceneIcon;
}
public enum EndingType
{
    SmartSaver,
    BareMinimumSurvivor,
    OverSpender,
    OverworkedMalnourished
}
