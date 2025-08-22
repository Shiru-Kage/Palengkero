using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectedCharacter : MonoBehaviour
{
    [Header("Character Prefabs")]
    [SerializeField] private GameObject[] characterPrefabs;
    public GameObject[] CharacterPrefabs => characterPrefabs;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI ageText;
    [SerializeField] private TextMeshProUGUI industryText;
    [SerializeField] private TextMeshProUGUI incomeText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI difficulty;
    [SerializeField] private Image characterFrameImage;
    [SerializeField] private Animator characterAnimator;
    private GameObject currentDisplayedPrefab;
    public GameObject CurrentDisplayedPrefab => currentDisplayedPrefab;

    public void DisplayCharacter(int index)
    {
        if (index < 0 || index >= characterPrefabs.Length)
        {
            Debug.LogWarning("Character index out of bounds!");
            return;
        }

        GameObject prefab = characterPrefabs[index];
        PlayerData playerData = prefab.GetComponent<PlayerData>();

        if (playerData == null || playerData.Data == null)
        {
            Debug.LogError("Missing PlayerData or CharacterData on prefab!");
            return;
        }

        CharacterData data = playerData.Data;

        nameText.text = data.characterName;
        ageText.text = "Age: " + data.characterAge;
        industryText.text = "Industry: " + data.characterIndustry;
        incomeText.text = "Income: " + data.characterMonthlyIncome + "/Month";
        descriptionText.text = data.characterDescription;
        characterFrameImage.sprite = data.characterSelectFrameSprite;
        difficulty.text = data.characterDifficulty;
        if (characterAnimator != null && data.characterAnimator != null)
        {
            characterAnimator.runtimeAnimatorController = data.characterAnimator;
            characterAnimator.Play("Idle", 0);
        }

        currentDisplayedPrefab = prefab;
    }

    public void SelectCharacter()
    {
        if (currentDisplayedPrefab == null)
        {
            Debug.LogWarning("No character has been displayed yet to select.");
            return;
        }

        CharacterSelectionManager.Instance.SetSelectedPrefab(currentDisplayedPrefab);
        Debug.Log("Character selected: " + currentDisplayedPrefab.name);
    }
}
