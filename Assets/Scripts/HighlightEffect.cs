using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Outline))]
public class HighlightEffect : MonoBehaviour
{
    [Header("Outline Settings")]
    [SerializeField] private Color outlineColor = Color.yellow;
    [SerializeField] private float blinkSpeed = 2f;
    [SerializeField] private float outlineThickness = 2f;

    private Outline outline;
    private Color originalColor;
    private Coroutine blinkRoutine;

    private void Awake()
    {
        outline = GetComponent<Outline>();
        if (outline != null)
        {
            originalColor = outline.effectColor;
            outline.effectColor = outlineColor;
            SetOutlineThickness(outlineThickness);
            outline.enabled = false;
        }
    }

    public void SetBlinking(bool shouldBlink)
    {
        if (outline == null) return;

        if (shouldBlink)
            StartBlinking();
        else
            StopBlinking();
    }
    public void SetHighlight(bool enabled)
    {
        if (outline == null) return;

        StopBlinking();
        outline.enabled = enabled;
        if (enabled)
        {
            outline.effectColor = outlineColor;
            SetOutlineThickness(outlineThickness);
        }
    }

    private void StartBlinking()
    {
        if (blinkRoutine == null)
        {
            outline.enabled = true;
            SetOutlineThickness(outlineThickness);
            blinkRoutine = StartCoroutine(BlinkOutline());
        }
    }

    private void StopBlinking()
    {
        if (blinkRoutine != null)
        {
            StopCoroutine(blinkRoutine);
            blinkRoutine = null;
        }

        outline.effectColor = originalColor;
        outline.enabled = false;
    }

    private IEnumerator BlinkOutline()
    {
        float t = 0f;
        Color baseColor = outlineColor;

        while (true)
        {
            t += Time.unscaledDeltaTime * blinkSpeed;
            baseColor.a = Mathf.PingPong(t, 1f);
            outline.effectColor = baseColor;
            yield return null;
        }
    }

    private void SetOutlineThickness(float thickness)
    {
        outline.effectDistance = new Vector2(thickness, thickness);
    }
}
