using UnityEngine;
using UnityEngine.UI;

public class Stall_Inner_UI : MonoBehaviour
{
    [SerializeField] private Image stall_InnerBG;
    [SerializeField] private Image vendor;
    [SerializeField] private Image stall_InnerRoofBG;

    // Method to update the UI elements based on the StallUI data
    public void UpdateUI(StallUI stallUI)
    {
        Debug.Log("UpdateUI Is being called.");
        if (stallUI != null)
        {
            // Access the stallData through the getter
            var stallData = stallUI.GetStallData();

            // Set the background and roof images from stallData
            stall_InnerBG.sprite = stallData.stallUIBackground;
            stall_InnerRoofBG.sprite = stallData.stallUIRoofBackground;

            // Set the vendor image using the character data from the stall's vendor
            if (stallData.vendor != null)
            {
                Debug.Log("stallData.vendor is being called");
                CharacterData vendorData = stallData.vendor;
                if (vendorData != null && vendorData.characterSprites.Length > 0)
                {
                    vendor.sprite = vendorData.characterSprites[0];  // Display the first sprite from the character data
                }
                else
                {
                    Debug.LogWarning("Vendor CharacterData or Sprites not found.");
                }
            }
            else
            {
                Debug.LogWarning("stallData.vendor is null");
            }
        }
        else
        {
            Debug.LogWarning("stallUI Is null.");
        }
    }
}
