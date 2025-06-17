using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectedCharacter : MonoBehaviour
{
    [Header("Character Data")]
    public CharacterData[] characters; 

    [Header("UI References")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI ageText;
    public TextMeshProUGUI industryText;
    public TextMeshProUGUI incomeText;
    public TextMeshProUGUI budgetText;
    public TextMeshProUGUI descriptionText;
    public Image characterImage;
    public void DisplayCharacter(int index)
    {
        if (index < 0 || index >= characters.Length)
        {
            Debug.LogWarning("Character index out of bounds!");
            return;
        }

        CharacterData data = characters[index];

        nameText.text = data.characterName;
        ageText.text = "Age: " + data.characterAge.ToString();
        industryText.text = "Industry: " + data.characterIndustry;
        incomeText.text = "Monthly Income: " + data.characterMonthlyIncome;
        budgetText.text = "Weekly Budget: " + data.characterWeeklyBudget.ToString();
        descriptionText.text = data.characterDescription;
        characterImage.sprite = data.characterSprite;
    }
}
