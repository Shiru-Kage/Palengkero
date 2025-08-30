using UnityEngine;
using UnityEngine.UI;

public class Stall_Inner_UI : MonoBehaviour
{
    [SerializeField] private Image stall_InnerBG;
    [SerializeField] private Image vendor;
    [SerializeField] private Image stall_InnerRoofBG;

    public void UpdateUI(StallUI stallUI)
    {
        if (stallUI != null)
        {
            var stallData = stallUI.GetStallData();

            stall_InnerBG.sprite = stallData.stallUIBackground;
            stall_InnerRoofBG.sprite = stallData.stallUIRoofBackground;

            if (stallData.vendor != null)
            {
                CharacterData vendorData = stallData.vendor;
                if (vendorData != null && vendorData.characterSprites.Length > 0)
                {
                    vendor.sprite = vendorData.characterSprites[0]; 
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
