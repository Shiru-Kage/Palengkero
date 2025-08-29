using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HighlightEffect : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] private Color highlightColor = Color.yellow;
    [SerializeField] private float blinkSpeed = 2f;

    [Header("UI Outline Settings")]
    [SerializeField] private float uiOutlineThickness = 2f;

    [Header("Sprite Highlight Settings")]
    [SerializeField] private float spriteScaleIncrease = 0.05f; // % scale increase for sprite copy

    private Outline uiOutline;
    private SpriteRenderer spriteRenderer;
    private GameObject spriteHighlightCopy;

    private Color originalUIColor;
    private Coroutine blinkRoutine;

    private void Awake()
    {
        uiOutline = GetComponent<Outline>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (uiOutline != null)
        {
            originalUIColor = uiOutline.effectColor;
            uiOutline.enabled = false;
            uiOutline.effectDistance = new Vector2(uiOutlineThickness, uiOutlineThickness);
        }

        if (spriteRenderer != null)
        {
            CreateSpriteHighlightCopy();
        }
    }

    private void CreateSpriteHighlightCopy()
    {
        spriteHighlightCopy = new GameObject("HighlightCopy");
        spriteHighlightCopy.transform.SetParent(transform);
        spriteHighlightCopy.transform.localPosition = Vector3.zero;
        spriteHighlightCopy.transform.localRotation = Quaternion.identity;
        spriteHighlightCopy.transform.localScale = Vector3.one * (1f + spriteScaleIncrease);

        var copyRenderer = spriteHighlightCopy.AddComponent<SpriteRenderer>();
        copyRenderer.sprite = spriteRenderer.sprite;
        copyRenderer.color = highlightColor;
        copyRenderer.sortingLayerID = spriteRenderer.sortingLayerID;
        copyRenderer.sortingOrder = spriteRenderer.sortingOrder + 1;

        spriteHighlightCopy.SetActive(false);
    }

    public void SetBlinking(bool shouldBlink)
    {
        if (uiOutline == null && spriteHighlightCopy == null) return;

        if (shouldBlink)
            StartBlinking();
        else
            StopBlinking();
    }

    public void SetHighlight(bool enabled)
    {
        StopBlinking();

        if (uiOutline != null)
        {
            uiOutline.enabled = enabled;
            if (enabled)
            {
                uiOutline.effectColor = highlightColor;
                uiOutline.effectDistance = new Vector2(uiOutlineThickness, uiOutlineThickness);
            }
        }

        if (spriteHighlightCopy != null)
        {
            spriteHighlightCopy.SetActive(enabled);
            if (enabled)
                spriteHighlightCopy.GetComponent<SpriteRenderer>().color = highlightColor;
        }
    }

    private void StartBlinking()
    {
        if (blinkRoutine == null)
            blinkRoutine = StartCoroutine(BlinkRoutine());
    }

    private void StopBlinking()
    {
        if (blinkRoutine != null)
        {
            StopCoroutine(blinkRoutine);
            blinkRoutine = null;
        }

        if (uiOutline != null)
        {
            uiOutline.enabled = false;
            uiOutline.effectColor = originalUIColor;
        }

        if (spriteHighlightCopy != null)
        {
            spriteHighlightCopy.SetActive(false);
        }
    }

    private IEnumerator BlinkRoutine()
    {
        float t = 0f;
        Color baseColor = highlightColor;

        if (uiOutline != null) uiOutline.enabled = true;
        if (spriteHighlightCopy != null) spriteHighlightCopy.SetActive(true);

        var sr = spriteHighlightCopy != null ? spriteHighlightCopy.GetComponent<SpriteRenderer>() : null;

        while (true)
        {
            t += Time.unscaledDeltaTime * blinkSpeed;
            float alpha = Mathf.PingPong(t, 1f);
            baseColor.a = alpha;

            if (uiOutline != null)
                uiOutline.effectColor = baseColor;

            if (sr != null)
                sr.color = baseColor;

            yield return null;
        }
    }
}
