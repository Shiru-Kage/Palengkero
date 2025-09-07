using UnityEngine;

[CreateAssetMenu(fileName = "TutorialPage", menuName = "Tutorial/Page")]
public class TutorialPage : ScriptableObject
{
    [TextArea(3, 6)]
    public string pageText;   // Text to display
    public Sprite pageImage;  // Image to show (optional)
}
