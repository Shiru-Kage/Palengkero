using UnityEngine;

public class CharacterSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private Transform spawnPoint;

    public void SpawnSelectedCharacter()
    {
        GameObject selectedPrefab = CharacterSelectionManager.Instance.SelectedCharacterPrefab;

        if (selectedPrefab == null)
        {
            Debug.LogError("No character selected! Make sure one is selected before loading this scene.");
            return;
        }

        Instantiate(selectedPrefab, spawnPoint.position, spawnPoint.rotation);
    }
}