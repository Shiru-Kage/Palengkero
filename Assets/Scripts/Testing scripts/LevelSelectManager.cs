using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelSelectManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Transform levelListParent;
    [SerializeField] private GameObject levelButtonPrefab;

    private CharacterData selectedCharacter;

    private void Start()
    {
        selectedCharacter = CharacterSelectionManager.Instance.SelectedCharacterData;

        if (selectedCharacter == null)
        {
            Debug.LogError("No character selected! Returning to character select.");
            // LoadCharacterSelectScene(); // optional
            return;
        }

        GenerateLevelButtons();
    }

    private void GenerateLevelButtons()
    {
        for (int i = 0; i < selectedCharacter.levelObjectives.Count; i++)
        {
            GameObject buttonGO = Instantiate(levelButtonPrefab, levelListParent);
            int levelIndex = i;

            LevelObjective objective = selectedCharacter.levelObjectives[levelIndex];
            string buttonText = $"Level {levelIndex + 1}\nBudget: ₱{objective.weeklyBudget}\nSavings Goal: ₱{objective.savingsGoal}";

            buttonGO.GetComponentInChildren<TextMeshProUGUI>().text = buttonText;
            buttonGO.GetComponent<Button>().onClick.AddListener(() => StartLevel(levelIndex));
        }
    }

    private void StartLevel(int levelIndex)
    {
        Debug.Log($"Loading Level {levelIndex + 1} for {selectedCharacter.characterName}");
        //PlayerProgress.Instance.SetCurrentLevel(levelIndex); // Optional, if you track this
        UnityEngine.SceneManagement.SceneManager.LoadScene($"Level{levelIndex + 1}");
    }
}
