using UnityEngine;
public class LoadGameManager : MonoBehaviour
{
    [SerializeField] private GameObject continueProgress;
    private void Start()
    {
        CheckContinueButtonState();
    }
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
                    bool metNutrition = entry.nutritionStars[i] > 0;
                    bool metSatisfaction = entry.satisfactionStars[i] > 0;
                    bool metSavings = entry.savingsStars[i] > 0;
                    StarSystem.Instance.AssignStarsForLevel(i, entry.characterID, metNutrition, metSatisfaction, metSavings);
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

            for (int i = 0; i < selectedEntry.levelStars.Count; i++)
            {
                bool metNutrition = selectedEntry.nutritionStars[i] > 0;
                bool metSatisfaction = selectedEntry.satisfactionStars[i] > 0;
                bool metSavings = selectedEntry.savingsStars[i] > 0;

                StarSystem.Instance.AssignStarsForLevel(i, selectedEntry.characterID, metNutrition, metSatisfaction, metSavings);
            }
        }
        LevelStateManager.Instance.SetSkipCutsceneOnLoad(true);

        SceneChanger.instance.ChangeScene("CharacterSelect");
    }

    public void CheckContinueButtonState()
    {
        if (SaveSystem.SaveExists(4))
        {
            continueProgress.SetActive(true);
        }
        else
        {
            continueProgress.SetActive(false);
        }
    }

    public void ResetAllCharacterLevelData()
    {
        LevelStateManager.Instance.ResetAllCharacterLevelData();
        StarSystem.Instance.ResetStarsForAllLevels();
        Debug.Log("All character level data has been reset.");
    }
}
