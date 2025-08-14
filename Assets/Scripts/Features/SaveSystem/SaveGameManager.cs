using UnityEngine;

public class SaveGameManager : MonoBehaviour
{
    public void SaveCurrentGame(int slotIndex)
{
    if (CharacterSelectionManager.Instance == null || LevelStateManager.Instance == null)
    {
        Debug.LogError("Missing required managers to save game.");
        return;
    }

    var saveData = new SaveData
    {
        characterID = CharacterSelectionManager.Instance.SelectedCharacterID
    };
    
    string selectedCharacterID = CharacterSelectionManager.Instance.SelectedCharacterID;
    Debug.Log($"Selected character ID: {selectedCharacterID}");

    // Iterate over all character prefabs, but only save data for the selected character
    foreach (var prefab in CharacterSelectionManager.Instance.AllCharacterPrefabs)
    {
        var pd = prefab.GetComponent<PlayerData>();
        if (pd == null || pd.Data == null) continue;

        string charName = pd.Data.characterName;
        string charID = pd.Data.characterID; // Assuming this is the unique ID for each character

        // Check if this character is the selected one
        if (charID == selectedCharacterID)
        {
            Debug.Log($"Saving stars for {charName}");  // Only save stars for the selected character
            
            var characterProgressEntry = new CharacterProgressEntry
            {
                characterName = charName,
                currentLevelIndex = LevelStateManager.Instance.CurrentLevelIndex,
                unlockedLevels = LevelStateManager.Instance.GetUnlockedLevelsForCurrentCharacter()
            };

            // Add the stars for each level for the selected character
            for (int i = 0; i < LevelStateManager.Instance.AllLevels.Length; i++)
            {
                int stars = StarSystem.Instance.GetStarsForLevel(i);  // Get stars for each level
                characterProgressEntry.levelStars.Add(stars);
            }

            // Only add data for the selected character
            saveData.characterProgressData.Add(characterProgressEntry);

            break;  // Once we've found the selected character, break the loop
        }
        else
        {
            // Skip all other characters
            Debug.Log($"Skipping stars for {charName} as it is not selected");
        }
    }

    saveData.totalStars = CalculateTotalStars(saveData);  // Calculate total stars for the selected character
    SaveSystem.SaveToSlot(slotIndex, saveData);  // Save the data
}

private int CalculateTotalStars(SaveData saveData)
{
    int totalStars = 0;
    foreach (var characterProgress in saveData.characterProgressData)
    {
        foreach (var stars in characterProgress.levelStars)
        {
            totalStars += stars;
        }
    }
    return totalStars;
}

}

