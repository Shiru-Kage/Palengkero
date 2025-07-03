using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class Cutscene : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private CharacterSelectionManager characterSelectionManager;
    [Header("UI")]
    [SerializeField] private Button skipButton;
    private void Start()
    {
        if (videoPlayer == null || characterSelectionManager == null)
        {
            Debug.LogError("Missing required references! VideoPlayer or CharacterSelectionManager.");
            return;
        }
        skipButton.gameObject.SetActive(false);
        videoPlayer.loopPointReached += OnVideoEnd;
    }

    public void PlayCutsceneForSelectedCharacter()
    {
        if (characterSelectionManager.SelectedCharacterData != null)
        {
            CharacterData selectedCharacter = characterSelectionManager.SelectedCharacterData;

            VideoClip cutsceneVideo = selectedCharacter.cutsceneVideo;

            if (cutsceneVideo != null)
            {
                videoPlayer.clip = cutsceneVideo;
                videoPlayer.Play();
                skipButton.gameObject.SetActive(true);
                Debug.Log("Playing cutscene for " + selectedCharacter.characterName);
            }
            else
            {
                Debug.LogError("No video found for the selected character.");
            }
        }
        else
        {
            Debug.LogError("No selected character data found.");
        }
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        if (SceneChanger.instance != null)
        {
            skipButton.gameObject.SetActive(false);
            SceneChanger.instance.ChangeScene("LevelSelect");
        }
        else
        {
            Debug.LogError("SceneChanger instance not found!");
        }
    }
    
    public void SkipCutscene()
    {
        videoPlayer.Stop();
        OnVideoEnd(videoPlayer);
    }
}
