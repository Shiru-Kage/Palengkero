using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class CharacterSelectionManager : MonoBehaviour
{
    public static CharacterSelectionManager Instance { get; private set; }
    public GameObject SelectedCharacterPrefab { get; private set; }
    public string SelectedCharacterID { get; private set; }
    public RuntimeCharacter SelectedRuntimeCharacter { get; private set; }
    public CharacterData SelectedCharacterData
    {
        get
        {
            if (SelectedCharacterPrefab == null) return null;
            PlayerData pd = SelectedCharacterPrefab.GetComponent<PlayerData>();
            return pd != null ? pd.Data : null;
        }
    }
    [SerializeField] private List<GameObject> characterPrefabs;
    public List<GameObject> AllCharacterPrefabs => characterPrefabs;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetSelectedPrefab(GameObject prefab)
    {
        SelectedCharacterPrefab = prefab;

        PlayerData pd = prefab.GetComponent<PlayerData>();
        if (pd != null && pd.Data != null)
        {
            SelectedRuntimeCharacter = new RuntimeCharacter(pd.Data);
            SelectedCharacterID = pd.Data.characterID;
            LevelStateManager.Instance.SetSelectedCharacter(pd.Data.characterName);
        }
        else
        {
            SelectedRuntimeCharacter = null;
            SelectedCharacterID = null;
        }
    }

    public void ResetRuntimeCharacterBudget()
    {
        if (SelectedRuntimeCharacter != null && SelectedCharacterData != null)
        {
            SelectedRuntimeCharacter.currentWeeklyBudget = SelectedCharacterData.characterWeeklyBudget;
        }
    }

    public GameObject GetPrefabByCharacterID(string characterID)
    {
        foreach (var prefab in characterPrefabs)
        {
            var pd = prefab.GetComponent<PlayerData>();
            if (pd != null && pd.Data != null && pd.Data.characterID == characterID)
            {
                return prefab;
            }
        }

        Debug.LogError("Character ID not found: " + characterID);
        return null;
    }
}

public class RuntimeCharacter
{
    public CharacterData characterData;
    public int currentWeeklyBudget;

    public RuntimeCharacter(CharacterData data)
    {
        characterData = data;
        currentWeeklyBudget = data.characterWeeklyBudget;
    }
}
