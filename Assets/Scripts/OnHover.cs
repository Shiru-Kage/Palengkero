using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OnHover : MonoBehaviour
{
    private Image img;
    private Color originalColor;
    private Color darkenedColor;
    private bool isHovered = false;

    void Start()
    {
        img = GetComponent<Image>();
        originalColor = img.color;
        darkenedColor = originalColor * 0.7f;
    }

    void Update()
    {
        bool currentlyHovered = IsPointerOverThisUI();

        if (currentlyHovered && !isHovered)
        {
            img.color = darkenedColor;
            isHovered = true;
        }
        else if (!currentlyHovered && isHovered)
        {
            img.color = originalColor;
            isHovered = false;
        }
    }

    private bool IsPointerOverThisUI()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (var result in results)
        {
            if (result.gameObject == this.gameObject)
                return true;
        }

        return false;
    }
}
