using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadGameManager : MonoBehaviour
{
    [Header("Required Prefabs")]
    [SerializeField] private GameObject characterSelectionManagerPrefab;
    [SerializeField] private GameObject levelStateManagerPrefab;

    public void LoadGameSlot(int slotIndex)
    {
        SaveData saveData = SaveSystem.LoadFromSlot(slotIndex);
        if (saveData == null)
        {
            Debug.LogWarning($"No save data found for slot {slotIndex}");
            return;
        }

        var characterManager = CharacterSelectionManager.Instance;
        if (characterManager == null)
        {
            GameObject charGO = Instantiate(characterSelectionManagerPrefab);
            characterManager = charGO.GetComponent<CharacterSelectionManager>();
        }

        var levelManager = LevelStateManager.Instance;
        if (levelManager == null)
        {
            GameObject levelGO = Instantiate(levelStateManagerPrefab);
            levelManager = levelGO.GetComponent<LevelStateManager>();
        }

        characterManager.LoadCharacterFromID(saveData.characterID);
        levelManager.SetSelectedCharacter(saveData.characterID);
        levelManager.SetLevelIndex(saveData.currentLevelIndex);
        levelManager.SetUnlockedLevelsForCurrentCharacter(saveData.unlockedLevels);

        SceneChanger.instance.ChangeScene("LevelSelect");
    }
}
