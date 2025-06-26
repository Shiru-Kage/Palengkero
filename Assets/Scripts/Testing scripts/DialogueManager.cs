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
            Transform dialogueUI = dialogueController.transform.Find("Dialogue Box");
            if (dialogueUI != null) dialogueUI.gameObject.SetActive(true);

            if (dialogueUI.gameObject.activeSelf)
            {
                dialogueController.PlayScene(storyScene);
            }
            else
            {
                return;
            }
        }

        events.OnSceneStart.Invoke();
    }

    void Update()
    {
        if (currentScene != null)
        {
            ShowDialogueBox();
        }
        else
        {
            HideDialogueBox();
        }
    }

    public void HandleInput(Vector2 position)
    {
        Debug.Log("Pressing me");

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

        // Ensure OnSceneEnd only fires when the dialogue ends
        dialogueController.events.OnDialogueEnd.AddListener(HandleDialogueEnd);
    }


    private IEnumerator SwitchScene(GameScene scene)
    {
        state = State.ANIMATE;
        currentScene = scene;

        if (scene is StoryScene storyScene)
        {
            dialogueController.ClearText();
            dialogueController.PlayScene(storyScene);
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
        Transform dialogueUI = dialogueController.transform.Find("Dialogue Box");
        if (dialogueUI != null)
        {
            dialogueUI.gameObject.SetActive(false);
        }
    }

    public void ShowDialogueBox()
    {
        Transform dialogueUI = dialogueController.transform.Find("Dialogue Box");
        if (dialogueUI != null)
        {
            dialogueUI.gameObject.SetActive(true);
        }
    }

    private void HandleDialogueEnd()
    {
        // Unsubscribe to avoid firing multiple times
        dialogueController.events.OnDialogueEnd.RemoveListener(HandleDialogueEnd);
        HideDialogueBox();
        events.OnSceneEnd.Invoke();
    }

    public void ResetScene()
    {
        currentScene = null;
    }
}
