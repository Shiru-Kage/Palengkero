using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class StallManager : MonoBehaviour
{
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

        originalColor = outline.effectColor;
        outline.effectColor = outlineColor;
        SetOutlineThickness(outlineThickness);

        outline.enabled = false;
    }

    public void StartBlinkingOutline()
    {
        if (blinkRoutine == null)
        {
            outline.enabled = true;
            SetOutlineThickness(outlineThickness);
            blinkRoutine = StartCoroutine(BlinkOutline());
        }
    }

    public void StopBlinkingOutline()
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
        // Uniform X and Y for symmetrical outline
        outline.effectDistance = new Vector2(thickness, thickness);
    }
}