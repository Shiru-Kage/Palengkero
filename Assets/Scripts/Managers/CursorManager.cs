using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CursorManager : MonoBehaviour
{
    [Header("Cursor Textures")]
    [SerializeField] private Texture2D defaultCursor;
    [SerializeField] private Texture2D uiCursor;

    private Vector2 defaultHotspot;
    private Vector2 uiHotspot;
    private Texture2D currentCursor;

    void Update()
    {
        Texture2D newCursor = defaultCursor;
        Vector2 hotspot = defaultHotspot;

        if (EventSystem.current != null)
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            foreach (var hit in results)
            {
                GameObject obj = hit.gameObject;

                if (IsInteractiveUI(obj))
                {
                    newCursor = uiCursor;
                    hotspot = uiHotspot;
                    break;
                }

                var graphic = obj.GetComponent<Graphic>();
                if (graphic != null && graphic.raycastTarget)
                {
                    break;
                }
            }
        }

        if (newCursor != currentCursor)
        {
            SetCursor(newCursor, hotspot);
        }
    }

    private bool IsInteractiveUI(GameObject obj)
    {
        return obj.GetComponent<Button>() != null ||
            obj.GetComponent<InputField>() != null ||
            obj.GetComponent<TMP_InputField>() != null ||
            obj.GetComponentInParent<Button>() != null ||
            obj.GetComponentInParent<InputField>() != null ||
            obj.GetComponentInParent<TMP_InputField>() != null;
    }

    private void SetCursor(Texture2D texture, Vector2 hotspot)
    {
        Cursor.SetCursor(texture, hotspot, CursorMode.Auto);
        currentCursor = texture;
    }
}
