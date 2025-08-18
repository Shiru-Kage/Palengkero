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

    var saveData = new SaveData();

    // Loop through all character prefabs and save their progress
    foreach (var prefab in CharacterSelectionManager.Instance.AllCharacterPrefabs)
    {
        var pd = prefab.GetComponent<PlayerData>();
        if (pd == null || pd.Data == null) continue;

        string charID = pd.Data.characterID;
        string charName = pd.Data.characterName;
        LevelStateManager.Instance.SetSelectedCharacter(charName);

        var characterProgress = new CharacterProgressEntry
        {
            characterID = charID,  // Store the characterID
            characterName = charName,
            currentLevelIndex = LevelStateManager.Instance.CurrentLevelIndex,
            unlockedLevels = LevelStateManager.Instance.GetUnlockedLevelsForCurrentCharacter()
        };

        // Save stars for each level of the character
        for (int i = 0; i < LevelStateManager.Instance.AllLevels.Length; i++)
            {
                // Get LevelStars for this character and level
                var levelStars = StarSystem.Instance.GetStarsForLevel(i, charID);
                
                characterProgress.nutritionStars.Add(levelStars.nutritionStars);
                characterProgress.satisfactionStars.Add(levelStars.satisfactionStars);
                characterProgress.savingsStars.Add(levelStars.savingsStars);
                
                // Calculate the total stars for the level
                int totalStars = levelStars.nutritionStars + levelStars.satisfactionStars + levelStars.savingsStars;
                characterProgress.levelStars.Add(totalStars);
            }

        saveData.characterProgressData.Add(characterProgress);
    }

    SaveSystem.SaveToSlot(slotIndex, saveData);  // Save all character data
}


}

