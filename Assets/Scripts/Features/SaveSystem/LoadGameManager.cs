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

    foreach (CharacterProgressEntry entry in saveData.characterProgressData)
    {
        if (entry.characterName != null)
        {
            LevelStateManager.Instance.SetSelectedCharacter(entry.characterName);
            LevelStateManager.Instance.SetLevelIndex(entry.currentLevelIndex);
            LevelStateManager.Instance.SetUnlockedLevelsForCurrentCharacter(entry.unlockedLevels);

            for (int i = 0; i < entry.levelStars.Count; i++)
            {
                StarSystem.Instance.AssignStarsForLevel(i, entry.characterID, entry.levelStars[i]);
            }
        }
    }

    CharacterProgressEntry selectedEntry = saveData.characterProgressData
        .Find(entry => entry.characterID == CharacterSelectionManager.Instance.SelectedCharacterID);

    if (selectedEntry != null)
    {
        LevelStateManager.Instance.SetSelectedCharacter(selectedEntry.characterName);
        LevelStateManager.Instance.SetLevelIndex(selectedEntry.currentLevelIndex);
        LevelStateManager.Instance.SetUnlockedLevelsForCurrentCharacter(selectedEntry.unlockedLevels);

        // Load stars for the selected character
        for (int i = 0; i < selectedEntry.levelStars.Count; i++)
        {
            StarSystem.Instance.AssignStarsForLevel(i, selectedEntry.characterID, selectedEntry.levelStars[i]);
        }
    }

    SceneChanger.instance.ChangeScene("CharacterSelect");
}


    public void ResetAllCharacterLevelData()
    {
        LevelStateManager.Instance.ResetAllCharacterLevelData();
        StarSystem.Instance.ResetStarsForAllLevels();
        Debug.Log("All character level data has been reset.");
    }
}
