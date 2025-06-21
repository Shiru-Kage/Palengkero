using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SceneContainer : MonoBehaviour
{
    [SerializeField] private StoryScene storedScene;
    public StoryScene StoredScene { get => storedScene; set => storedScene = value;}

    public void PlayCurrentScene(DialogueManager dialogueManager)
    {
        if (storedScene != null)
        {
            dialogueManager.PlayScene(storedScene);
        }
        else
        {
            Debug.LogWarning("No StoryScene assigned to SceneContainer.");
        }
    }
}