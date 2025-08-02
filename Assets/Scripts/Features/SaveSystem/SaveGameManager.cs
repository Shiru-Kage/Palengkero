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
            characterID = CharacterSelectionManager.Instance.SelectedCharacterID,
            currentLevelIndex = LevelStateManager.Instance.CurrentLevelIndex,
            unlockedLevels = LevelStateManager.Instance.GetUnlockedLevelsForCurrentCharacter()
        };

        SaveSystem.SaveToSlot(slotIndex, saveData);
    }
}
