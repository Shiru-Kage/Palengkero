using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class Cutscene : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private VideoPlayer videoPlayer; // Reference to the VideoPlayer component
    [SerializeField] private CharacterSelectionManager characterSelectionManager; // Reference to CharacterSelectionManager

    private void Start()
    {
        // Ensure that necessary references are assigned
        if (videoPlayer == null || characterSelectionManager == null)
        {
            Debug.LogError("Missing required references! VideoPlayer or CharacterSelectionManager.");
            return;
        }

        // Register the event to trigger when the video finishes
        videoPlayer.loopPointReached += OnVideoEnd;
    }

    // Method to play the cutscene for the selected character
    public void PlayCutsceneForSelectedCharacter()
    {
        // Ensure the selected character data is available
        if (characterSelectionManager.SelectedCharacterData != null)
        {
            CharacterData selectedCharacter = characterSelectionManager.SelectedCharacterData;

            // Get the video clip directly from the selected character data
            VideoClip cutsceneVideo = selectedCharacter.cutsceneVideo;

            // Check if a valid video clip exists
            if (cutsceneVideo != null)
            {
                // Set the correct video clip for the video player
                videoPlayer.clip = cutsceneVideo;
                videoPlayer.Play(); // Play the selected cutscene video
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

    // Called when the video ends
    private void OnVideoEnd(VideoPlayer vp)
    {
        // Trigger the scene change using SceneChanger
        if (SceneChanger.instance != null)
        {
            // Call the SceneChanger to load the next scene
            SceneChanger.instance.ChangeScene("LevelSelect");  // Replace "NextSceneName" with the appropriate scene name or logic
        }
        else
        {
            Debug.LogError("SceneChanger instance not found!");
        }
    }
}
