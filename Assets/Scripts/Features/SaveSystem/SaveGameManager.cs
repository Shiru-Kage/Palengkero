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

        foreach (var prefab in CharacterSelectionManager.Instance.AllCharacterPrefabs)
        {
            var pd = prefab.GetComponent<PlayerData>();
            if (pd == null || pd.Data == null) continue;

            string charID = pd.Data.characterID;
            string charName = pd.Data.characterName;
            LevelStateManager.Instance.SetSelectedCharacter(charName);

            var characterProgress = new CharacterProgressEntry
            {
                characterID = charID,  
                characterName = charName,
                currentLevelIndex = LevelStateManager.Instance.CurrentLevelIndex,
                unlockedLevels = LevelStateManager.Instance.GetUnlockedLevelsForCurrentCharacter()
            };

            for (int i = 0; i < LevelStateManager.Instance.AllLevels.Length; i++)
                {
                    var levelStars = StarSystem.Instance.GetStarsForLevel(i, charID);

                    characterProgress.nutritionStars.Add(levelStars.nutritionStars);
                    characterProgress.satisfactionStars.Add(levelStars.satisfactionStars);
                    characterProgress.savingsStars.Add(levelStars.savingsStars);
                    
                    int totalStars = levelStars.nutritionStars + levelStars.satisfactionStars + levelStars.savingsStars;
                    characterProgress.levelStars.Add(totalStars);
                }

            saveData.characterProgressData.Add(characterProgress);
        }

        SaveSystem.SaveToSlot(slotIndex, saveData);  
    }
}