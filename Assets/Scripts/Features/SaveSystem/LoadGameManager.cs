using UnityEngine;

public class LoadGameManager : MonoBehaviour
{
    public void LoadGameSlot(int slotIndex)
    {
        if (!SaveSystem.SaveExists(slotIndex))
        {
            Debug.LogWarning($"No save file exists in slot {slotIndex}. Scene will not change.");
            return;
        }

        SaveData saveData = SaveSystem.LoadFromSlot(slotIndex);
        if (saveData == null)
        {
            Debug.LogWarning($"Failed to load save data from slot {slotIndex}");
            return;
        }

        // First, loop through all characters and set their unlocked levels
        foreach (CharacterProgressEntry entry in saveData.characterProgressData)
        {
            if (entry.characterName != saveData.characterID)
            {
                // Set the unlocked levels first before assigning stars
                LevelStateManager.Instance.SetSelectedCharacter(entry.characterName);
                LevelStateManager.Instance.SetLevelIndex(entry.currentLevelIndex);
                LevelStateManager.Instance.SetUnlockedLevelsForCurrentCharacter(entry.unlockedLevels);
            }
        }

        // Then, load the selected character data and handle stars
        CharacterProgressEntry selectedEntry = saveData.characterProgressData
            .Find(entry => entry.characterName == saveData.characterID);

        if (selectedEntry != null)
        {
            // Set the unlocked levels for the selected character
            LevelStateManager.Instance.SetSelectedCharacter(selectedEntry.characterName);
            LevelStateManager.Instance.SetLevelIndex(selectedEntry.currentLevelIndex);
            LevelStateManager.Instance.SetUnlockedLevelsForCurrentCharacter(selectedEntry.unlockedLevels);

            // Now, assign stars to the levels for the selected character
            for (int i = 0; i < selectedEntry.levelStars.Count; i++)
            {
                StarSystem.Instance.AssignStarsForLevel(i, selectedEntry.levelStars[i]);
            }
        }

        // Finally, load the selected character from the ID
        CharacterSelectionManager.Instance.LoadCharacterFromID(saveData.characterID);

        Debug.Log("Preloaded save data for slot " + slotIndex + ". Transitioning to CharacterSelect...");

        SceneChanger.instance.ChangeScene("CharacterSelect");
    }

    public void ResetAllCharacterLevelData()
    {
        LevelStateManager.Instance.ResetAllCharacterLevelData();
        StarSystem.Instance.ResetStarsForAllLevels();
        Debug.Log("All character level data has been reset.");
    }
}
