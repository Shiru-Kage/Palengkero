using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneContainer : MonoBehaviour
{
    [Header("Stored Scenes")]
    [SerializeField] private StoryScene[] storyScenes;  // Store multiple scenes

    private SelectedCharacter selectedCharacter; // Reference to SelectedCharacter

    // Reference to the character selection manager to get the selected character prefab
    private void Awake()
    {
        // Find the SelectedCharacter instance if it's not already assigned
        selectedCharacter = Object.FindAnyObjectByType<SelectedCharacter>();

        if (selectedCharacter == null)
        {
            Debug.LogError("SelectedCharacter not found in the scene.");
        }
    }

    // Play the scene for the selected character
    public void PlaySceneForSelectedCharacter(DialogueManager dialogueManager)
    {
        if (selectedCharacter != null)
        {
            // Get the index of the selected character's prefab
            GameObject selectedPrefab = selectedCharacter.CurrentDisplayedPrefab; // Access the currently selected prefab

            if (selectedPrefab != null)
            {
                // Get the index of the selected character
                int characterIndex = GetCharacterIndexFromPrefab(selectedPrefab);
                
                if (characterIndex >= 0 && characterIndex < storyScenes.Length)
                {
                    StoryScene selectedScene = storyScenes[characterIndex];
                    if (selectedScene != null)
                    {
                        dialogueManager.PlayScene(selectedScene); // Play the selected scene
                        Debug.Log("Playing scene for selected character.");
                    }
                    else
                    {
                        Debug.LogWarning("No scene found for the selected character.");
                    }
                }
                else
                {
                    Debug.LogWarning("Invalid character index or no scene for this character.");
                }
            }
            else
            {
                Debug.LogError("Selected prefab is null.");
            }
        }
        else
        {
            Debug.LogWarning("SelectedCharacter instance is null.");
        }
    }

    // Helper function to map selected character prefab to scene index
    private int GetCharacterIndexFromPrefab(GameObject selectedPrefab)
    {
        // Assuming the character prefabs and storyScenes are in the same order
        for (int i = 0; i < selectedCharacter.CharacterPrefabs.Length; i++)
        {
            if (selectedPrefab == selectedCharacter.CharacterPrefabs[i])
            {
                return i; // Return the index of the selected character's prefab
            }
        }
        return -1; // Return -1 if the prefab is not found
    }
}
