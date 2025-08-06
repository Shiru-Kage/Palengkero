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

        foreach (var prefab in CharacterSelectionManager.Instance.AllCharacterPrefabs)
        {
            var pd = prefab.GetComponent<PlayerData>();
            if (pd == null || pd.Data == null) continue;

            string charName = pd.Data.characterName;

            LevelStateManager.Instance.SetSelectedCharacter(charName);

            saveData.characterProgressData.Add(new CharacterProgressEntry
            {
                characterName = charName,
                currentLevelIndex = LevelStateManager.Instance.CurrentLevelIndex,
                unlockedLevels = LevelStateManager.Instance.GetUnlockedLevelsForCurrentCharacter()
            });
        }

        SaveSystem.SaveToSlot(slotIndex, saveData);
    }
}

