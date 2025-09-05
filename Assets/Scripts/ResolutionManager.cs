using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ResolutionManager : MonoBehaviour
{
    // UI Buttons to switch between aspect ratios and toggle fullscreen/windowed mode
    public Button aspectRatio16_9Button;
    public Button aspectRatio4_3Button;
    public Button aspectRatio16_10Button;
    public Button fullscreenButton;
    public Button windowedButton;

    // Define aspect ratios and corresponding resolutions
    private AspectRatio ratio16_9 = new AspectRatio(1920, 1080); // 16:9
    private AspectRatio ratio4_3 = new AspectRatio(1024, 768);  // 4:3
    private AspectRatio ratio16_10 = new AspectRatio(1920, 1200); // 16:10

    // Dictionary to hold available resolutions for each aspect ratio
    public Dictionary<float, List<AspectRatio>> aspectRatios = new Dictionary<float, List<AspectRatio>>()
    {
        { 16f / 9f, new List<AspectRatio> { new AspectRatio(1920, 1080), new AspectRatio(1280, 720), new AspectRatio(3840, 2160) } },
        { 4f / 3f, new List<AspectRatio> { new AspectRatio(1024, 768), new AspectRatio(2048, 1536) } },
        { 16f / 10f, new List<AspectRatio> { new AspectRatio(1920, 1200), new AspectRatio(1280, 800) } },
    };

    private void Start()
    {
        // Set up listeners for buttons
        aspectRatio16_9Button.onClick.AddListener(() => SetResolution(ratio16_9));
        aspectRatio4_3Button.onClick.AddListener(() => SetResolution(ratio4_3));
        aspectRatio16_10Button.onClick.AddListener(() => SetResolution(ratio16_10));

        // Fullscreen and Windowed toggle buttons
        fullscreenButton.onClick.AddListener(ToggleFullscreen);
        windowedButton.onClick.AddListener(ToggleWindowed);
    }

    // Set resolution based on the selected aspect ratio
    private void SetResolution(AspectRatio ratio)
    {
        // Change the resolution to match the aspect ratio
        Screen.SetResolution(ratio.width, ratio.height, false); // false for windowed mode

        // Log the current resolution change
        Debug.Log($"Resolution set to: {ratio.width}x{ratio.height} (Aspect Ratio: {ratio.GetRatio():0.00})");
    }

    // Toggle between fullscreen and windowed mode
    private void ToggleFullscreen()
    {
        // Set the screen to fullscreen
        Screen.fullScreen = true;

        // Log the fullscreen toggle
        Debug.Log("Fullscreen mode: Enabled");
    }

    // Toggle between windowed and fullscreen mode
    private void ToggleWindowed()
    {
        // Set the screen to windowed
        Screen.fullScreen = false;

        // Log the windowed toggle
        Debug.Log("Fullscreen mode: Disabled (Windowed)");
    }

    // Set resolution dynamically based on the aspect ratio
    private void SetResolutionFromAspectRatio(float aspectRatio)
    {
        if (aspectRatios.ContainsKey(aspectRatio))
        {
            // Select the first available resolution for the selected aspect ratio
            AspectRatio selectedResolution = aspectRatios[aspectRatio][0];
            Screen.SetResolution(selectedResolution.width, selectedResolution.height, false);

            // Log the dynamic resolution change
            Debug.Log($"Dynamic Resolution set to: {selectedResolution.width}x{selectedResolution.height}");
        }
    }
}

// Helper struct to represent aspect ratio and resolution
public struct AspectRatio
{
    public int width;
    public int height;

    public AspectRatio(int width, int height)
    {
        this.width = width;
        this.height = height;
    }

    // Get the aspect ratio (width/height)
    public float GetRatio()
    {
        return (float)width / height;
    }
}
