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
            LevelData currentLevelData = LevelStateManager.Instance.GetCurrentLevelData();

            Sprite selectedBackground = GetBackgroundForCurrentTimeOfDay(currentLevelData.backgroundType, stallData);
            stall_InnerBG.sprite = selectedBackground;
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
    
    private Sprite GetBackgroundForCurrentTimeOfDay(BackgroundType backgroundType, StallData stallData)
    {
        switch (backgroundType)
        {
            case BackgroundType.Morning:
                return stallData.morningBackgrounds.Length > 0 ? stallData.morningBackgrounds[0] : null;
            case BackgroundType.Afternoon:
                return stallData.afternoonBackgrounds.Length > 0 ? stallData.afternoonBackgrounds[0] : null;
            case BackgroundType.Night:
                return stallData.nightBackgrounds.Length > 0 ? stallData.nightBackgrounds[0] : null;
            default:
                Debug.LogWarning("Unknown BackgroundType. Defaulting to Morning.");
                return stallData.morningBackgrounds.Length > 0 ? stallData.morningBackgrounds[0] : null;
        }
    }
}
