using UnityEngine;
using UnityEngine.Events;
public class CustomButton : MonoBehaviour
{
    private PolygonCollider2D poly;
    public UnityEvent onClick;

    void Start()
    {
        poly = GetComponent<PolygonCollider2D>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (poly.OverlapPoint(mouseWorld))
            {
                onClick.Invoke();
            }
        }
    }
}
