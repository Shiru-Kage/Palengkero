using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ResolutionManager : MonoBehaviour
{
    public Button aspectRatio16_9Button;
    public Button aspectRatio4_3Button;
    public Button aspectRatio16_10Button;
    public Button fullscreenButton;
    public Button windowedButton;

    private AspectRatio ratio16_9 = new AspectRatio(1920, 1080);
    private AspectRatio ratio4_3 = new AspectRatio(1024, 768);  
    private AspectRatio ratio16_10 = new AspectRatio(1920, 1200);

    public Dictionary<float, List<AspectRatio>> aspectRatios = new Dictionary<float, List<AspectRatio>>()
    {
        { 16f / 9f, new List<AspectRatio> { new AspectRatio(1920, 1080), new AspectRatio(1280, 720), new AspectRatio(3840, 2160) } },
        { 4f / 3f, new List<AspectRatio> { new AspectRatio(1024, 768), new AspectRatio(2048, 1536) } },
        { 16f / 10f, new List<AspectRatio> { new AspectRatio(1920, 1200), new AspectRatio(1280, 800) } },
    };

    private void Start()
    {
        Screen.fullScreen = true;

        aspectRatio16_9Button.onClick.AddListener(() => SetResolution(ratio16_9));
        aspectRatio4_3Button.onClick.AddListener(() => SetResolution(ratio4_3));
        aspectRatio16_10Button.onClick.AddListener(() => SetResolution(ratio16_10));

        fullscreenButton.onClick.AddListener(ToggleFullscreen);
        windowedButton.onClick.AddListener(ToggleWindowed);
    }

    private void SetResolution(AspectRatio ratio)
    {
        Screen.SetResolution(ratio.width, ratio.height, false); 
        Debug.Log($"Resolution set to: {ratio.width}x{ratio.height} (Aspect Ratio: {ratio.GetRatio():0.00})");
    }

    private void ToggleFullscreen()
    {
        Screen.fullScreen = true;

        Debug.Log("Fullscreen mode: Enabled");
    }

    private void ToggleWindowed()
    {
        Screen.fullScreen = false;

        Debug.Log("Fullscreen mode: Disabled (Windowed)");
    }

    private void SetResolutionFromAspectRatio(float aspectRatio)
    {
        if (aspectRatios.ContainsKey(aspectRatio))
        {
            AspectRatio selectedResolution = aspectRatios[aspectRatio][0];
            Screen.SetResolution(selectedResolution.width, selectedResolution.height, false);

            Debug.Log($"Dynamic Resolution set to: {selectedResolution.width}x{selectedResolution.height}");
        }
    }
}

public struct AspectRatio
{
    public int width;
    public int height;

    public AspectRatio(int width, int height)
    {
        this.width = width;
        this.height = height;
    }

    public float GetRatio()
    {
        return (float)width / height;
    }
}
