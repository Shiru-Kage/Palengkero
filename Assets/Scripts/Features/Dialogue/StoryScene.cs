using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialgoue", menuName = "Dialogue/New Dialgoue Scene")]
public class StoryScene : GameScene
{
    public List<Sentence> sentences;
    public Sprite background;
    public GameScene nextScene;
    [System.Serializable]
    public struct Sentence
    {
        public string text;
        public CharacterData[] speakers;
        //public int spriteIndex;
        public AudioClip dialogueAudio;
        public DialogueBoxActionType dialogueBoxActionTypes;
        public enum DialogueBoxActionType
        {
            NONE, SHAKE, SIDEWAYS, HIDE, SHOW
        }
        public List<DialogueChoice> dialogueChoice;
        [System.Serializable]
        public struct DialogueChoice
        {
            public string text;
            public GameScene nextDialogue;
        }
    }
}
public class GameScene : ScriptableObject{}