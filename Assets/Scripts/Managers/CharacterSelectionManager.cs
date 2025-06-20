using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class CharacterSelectionManager : MonoBehaviour
{
    public static CharacterSelectionManager Instance { get; private set; }

    public GameObject SelectedCharacterPrefab { get; private set; }

    public CharacterData SelectedCharacterData
    {
        get
        {
            if (SelectedCharacterPrefab == null) return null;
            PlayerData pd = SelectedCharacterPrefab.GetComponent<PlayerData>();
            return pd != null ? pd.Data : null;
        }
    }

    [Header("Scenes where this should be destroyed")]
    [SerializeField] private List<string> scenesToDestroyOn = new List<string>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scenesToDestroyOn.Contains(scene.name))
        {
            Destroy(gameObject);
            Instance = null;
        }
    }

    public void SetSelectedPrefab(GameObject prefab)
    {
        SelectedCharacterPrefab = prefab;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
