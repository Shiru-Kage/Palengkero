using UnityEngine;
public class SaveGameManager : MonoBehaviour
{
    public static SaveGameManager Instance { get; private set; }

    private int pendingSlotIndex = -1; 

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveCurrentGame(int slotIndex)
    {
        if (SaveSystem.SaveExists(slotIndex))
        {
            pendingSlotIndex = slotIndex;
            return;
        }

        PerformSave(slotIndex);
    }

    public void AutoSave()
    {
        int autoSaveSlotIndex = 4; 
        PerformSave(autoSaveSlotIndex); 
    }

    private void PerformSave(int slotIndex)
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
            Debug.Log($"Saving character progress: {charName}, Level: {LevelStateManager.Instance.CurrentLevelIndex}, Stars: {characterProgress.levelStars}");
        }

        SaveSystem.SaveToSlot(slotIndex, saveData);
        Debug.Log($"Game saved to slot {slotIndex}");

        LevelSelectUI.Instance.RefreshLevelButtons();
    }

    public void OnConfirmSave()
    {
        if (pendingSlotIndex >= 0)
        {
            PerformSave(pendingSlotIndex);
            pendingSlotIndex = -1;
        }
    }

    public void OnCancelSave()
    {
        pendingSlotIndex = -1;
    }
}