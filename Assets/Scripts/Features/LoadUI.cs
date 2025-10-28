using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class LoadUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI[] slotTexts;  // Array of TextMeshPro objects for each slot (Slot 1, Slot 2, etc.)

    void Start()
    {
        DisplayCharacterProgress();
    }

    private void DisplayCharacterProgress()
    {
        // Iterate through all save slots
        for (int i = 0; i < slotTexts.Length; i++)
        {
            string displayText = "";

            // Fetch the save data for each slot
            SaveData saveData = SaveSystem.LoadFromSlot(i);

            if (saveData != null)
            {
                // If save data exists, loop through the character progress data
                foreach (var entry in saveData.characterProgressData)
                {
                    displayText += $"Character: \n{entry.characterName}\n\n";

                    // Display the highest unlocked level
                    int highestLevelUnlocked = entry.unlockedLevels.Length > 0 ? entry.unlockedLevels[entry.unlockedLevels.Length - 1] ? entry.unlockedLevels.Length : 0 : 0;
                    displayText += $"Highest Level: {highestLevelUnlocked}\n\n";

                    // Calculate total stars for the character
                    int totalStars = 0;

                    for (int j = 0; j <= highestLevelUnlocked; j++)  // Only show stars for unlocked levels
                    {
                        // Ensure that the stars lists are not accessed out of range
                        if (j < entry.nutritionStars.Count)
                            totalStars += (entry.nutritionStars[j] > 0) ? 1 : 0;
                        if (j < entry.satisfactionStars.Count)
                            totalStars += (entry.satisfactionStars[j] > 0) ? 1 : 0;
                        if (j < entry.savingsStars.Count)
                            totalStars += (entry.savingsStars[j] > 0) ? 1 : 0;
                    }

                    // Display total stars for the character
                    displayText += $"Total Stars: {totalStars}\n\n\n\n\n";  // Max Stars would be based on the number of unlocked levels
                }
            }
            else
            {
                // If no save data for this slot, show a message
                displayText += "No save data.\n";
            }

            // Update the corresponding TextMeshPro slot text
            if (slotTexts[i] != null)
            {
                slotTexts[i].text = displayText;
            }
            else
            {
                Debug.LogError($"Slot {i + 1} TextMeshPro reference is missing!");
            }
        }
    }
}
