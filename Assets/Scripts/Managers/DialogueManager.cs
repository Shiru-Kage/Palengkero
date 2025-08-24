using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [Header("Scene")]
    [SerializeField] private GameScene currentScene;
    public GameScene CurrentScene { get => currentScene; set => currentScene = value; }

    [Header("Managers")]
    [SerializeField] private DialogueBoxController dialogueController;
    //[SerializeField] private CutsceneBackground backgroundChanger;


    public Events events;

    private State state = State.IDLE;

    private enum State
    {
        IDLE, ANIMATE
    }

    [System.Serializable]
    public struct Events
    {
        public UnityEvent OnSceneStart;
        public UnityEvent OnSceneEnd;
    }
    /*
    void Awake()
    {
        if (backgroundChanger != null)
        {
            backgroundChanger.gameObject.SetActive(false); 
        }
    }*/
    void Start()
    {
        if (currentScene == null || string.IsNullOrEmpty(currentScene.name))
        {
            HideDialogueBox();
            return;
        }

        if (currentScene is StoryScene storyScene)
        {
            Debug.Log("Playing this scene for the first time.");
            ShowDialogueBox();
            dialogueController.PlayScene(storyScene);
        }

        events.OnSceneStart.Invoke();
    }

    public void HandleInput(Vector2 position)
    {
        if (currentScene is StoryScene currentStoryScene)
        {
            HandleStorySceneInput(currentStoryScene);
        }
    }

    private void HandleStorySceneInput(StoryScene currentStoryScene) {
        int index = dialogueController.GetCurrentSentenceIndex();

        if (index < 0 || index >= currentStoryScene.sentences.Count) {
            Debug.LogWarning("Invalid sentence index. Cannot continue dialogue.");
            return;
        }

        StoryScene.Sentence currentSentence = currentStoryScene.sentences[index];

        if (state == State.IDLE && dialogueController.IsCompleted()) {
            HandleIdleState(currentStoryScene, currentSentence);
        } else if (state == State.IDLE && dialogueController.IsPrinting()) {
            dialogueController.SkipPrinting();
        }
    }


    private void HandleIdleState(StoryScene currentStoryScene, StoryScene.Sentence currentSentence)
    {
        if (dialogueController.IsLastSentence() && currentSentence.dialogueChoice.Count <= 0)
        {
            PlayScene(currentStoryScene.nextScene);
        }
        else
        {
            HandleDialogueChoices(currentSentence);
        }
    }

    private void HandleDialogueChoices(StoryScene.Sentence currentSentence)
    {
        dialogueController.PlayNextSentence();
    }

    public void PlayScene(GameScene scene)
    {
        StartCoroutine(SwitchScene(scene));
        if (currentScene != null)
        {
            ShowDialogueBox();
        }
        dialogueController.events.OnDialogueEnd.AddListener(HandleDialogueEnd);
    }


    private IEnumerator SwitchScene(GameScene scene)
    {
        state = State.ANIMATE;
        currentScene = scene;

        if (scene is StoryScene storyScene)
        {
            //backgroundChanger.gameObject.SetActive(true); 
            dialogueController.ClearText();
            dialogueController.PlayScene(storyScene);
            //backgroundChanger.SetImage(storyScene.background);
            state = State.IDLE;
            
        }
        else
        {
            HideDialogueBox();
        }

        //events.OnSceneEnd.Invoke();
        yield return null;
    }

    public void HideDialogueBox()
    {
        GameObject dialogueUI = dialogueController.GetCurrentDialogueBox();
        GameObject dialogueVendorUI = dialogueController.GetVendorDialogueBox();
        if (dialogueUI != null)
        {
            dialogueUI.gameObject.SetActive(false);

            //backgroundChanger.gameObject.SetActive(false); 
        }
        dialogueVendorUI.gameObject.SetActive(false);
    }

    public void ShowDialogueBox()
    {
        GameObject dialogueUI = dialogueController.GetCurrentDialogueBox();
        if (dialogueUI != null)
        {
            dialogueUI.gameObject.SetActive(true);
            //backgroundChanger.gameObject.SetActive(true); 
        }
        if (currentScene is StoryScene storyScene)
        {
            StoryScene.Sentence currentSentence = storyScene.sentences[dialogueController.GetCurrentSentenceIndex()];
            CharacterData activeSpeaker = GetActiveSpeaker(currentSentence);

            if (activeSpeaker != null)
            {
                dialogueController.ToggleVendorDialogueBox(activeSpeaker.characterIndustry == "Vendor");
            }
            else
            {
                Debug.LogWarning("Active Speaker is null");
            }
        }
    }

    private CharacterData GetActiveSpeaker(StoryScene.Sentence sentence)
    {
        if (sentence.speakers.Length > 0)
        {
            return sentence.speakers[0]; 
        }
        return null;
    }


    private void HandleDialogueEnd()
    {
        dialogueController.events.OnDialogueEnd.RemoveListener(HandleDialogueEnd);
        if (currentScene == null)
        {
            HideDialogueBox();
        }
        events.OnSceneEnd.Invoke();
    }

    public void ResetScene()
    {
        currentScene = null;
    }
}
