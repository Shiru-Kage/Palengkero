using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "New Character Cutscene Data", menuName = "Character/Character Cutscene Data")]
public class Character_Cutscenes : ScriptableObject
{
    public string openingCutsceneName;
    public VideoClip openingCutscene;
    public string[] endingCutsceneName;
    public VideoClip[] endingCutscenes;
}
