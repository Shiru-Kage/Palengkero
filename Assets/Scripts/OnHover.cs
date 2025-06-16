using UnityEngine;
using UnityEngine.UI;
public class OnHover : MonoBehaviour
{
    private Image img;
    private PolygonCollider2D poly;
    private Color originalColor;
    private Color darkenedColor;
    private bool isHovered = false;

    void Start()
    {
        img = GetComponent<Image>();
        poly = GetComponent<PolygonCollider2D>();

        originalColor = img.color;
        darkenedColor = originalColor * 0.7f;
    }

    void Update()
    {
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        bool currentlyHovered = poly.OverlapPoint(mouseWorldPos);

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
}
