using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class StallUI : MonoBehaviour
{
    [Header("Stall Data")]
    [SerializeField] private StallData stallData;
    [Header("Stall UI")]
    [SerializeField] private SpriteRenderer upperIconRenderer;
    [SerializeField] private SpriteRenderer lowerIconRenderer;
    [SerializeField] private Image backgroundRenderer;

    [Header("Outline Settings")]
    [SerializeField] private Color outlineColor = Color.yellow;
    [SerializeField] private float blinkSpeed = 2f;
    [SerializeField] private float outlineThickness = 2f;

    private Outline outline;
    private Coroutine blinkRoutine;
    private Color originalColor;

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
        else
        {
            Debug.LogWarning($"Outline component missing on {gameObject.name}");
        }
    }

    private void Start()
    {

        if (upperIconRenderer != null)
            upperIconRenderer.sprite = stallData.upperStallIcon;

        if (lowerIconRenderer != null)
            lowerIconRenderer.sprite = stallData.lowerStallIcon;

        if (backgroundRenderer != null)
            backgroundRenderer.sprite = stallData.stallBackground;
    }

    public void SetBlinking(bool shouldBlink)
    {
        if (outline == null) return;

        if (shouldBlink)
            StartBlinkingOutline();
        else
            StopBlinkingOutline();
    }

    private void StartBlinkingOutline()
    {
        if (blinkRoutine == null)
        {
            outline.enabled = true;
            SetOutlineThickness(outlineThickness);
            blinkRoutine = StartCoroutine(BlinkOutline());
        }
    }

    private void StopBlinkingOutline()
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
