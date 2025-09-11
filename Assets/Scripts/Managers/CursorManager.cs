using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance { get; private set; }
    [Header("Cursor Textures")]
    [SerializeField] private Texture2D defaultCursor;
    [SerializeField] private Texture2D uiCursor;
    [Header("Audio")]
    [SerializeField] private AudioClip clickSound;

    private Vector2 defaultHotspot;
    private Vector2 uiHotspot;
    private Texture2D currentCursor;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this; 
        DontDestroyOnLoad(gameObject); 
    }

    void Update()
    {
        Texture2D newCursor = defaultCursor;
        Vector2 hotspot = defaultHotspot;

        bool overUI = false;

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
                    overUI = true;
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

        if (overUI && Input.GetMouseButtonDown(0))
        {
            PlayClickSound();
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

    private void PlayClickSound()
    {
        if (clickSound != null)
        {
            AudioManager.Instance.PlaySFX(clickSound);
        }
    }
}
