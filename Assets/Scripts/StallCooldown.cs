using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class StallCooldown : MonoBehaviour
{
    [Header("Cooldown Options")]
    [SerializeField] private Image stallCooldown;
    [SerializeField] private Image stallUpperHalf;

    [Tooltip("Color applied during cooldown (e.g., almost black)")]
    [SerializeField] private Color cooldownColor = new Color(0.1f, 0.1f, 0.1f, 1f);

    [Tooltip("Original color to revert to after cooldown")]
    [SerializeField] private Color normalColor = Color.white;

    [Tooltip("How long the cooldown lasts in seconds")]
    [SerializeField] private float cooldownDuration = 3f;

    public bool isCoolingDown = false;

    public void TriggerCooldown()
    {
        if (!isCoolingDown)
            StartCoroutine(CooldownRoutine());
    }

    private IEnumerator CooldownRoutine()
    {
        isCoolingDown = true;

        StallUI stallUI = GetComponent<StallUI>();
        if (stallUI != null)
        {
            stallUI.SetBlinking(false);
        }

        // Set visuals for cooldown
        if (stallUpperHalf != null)
            stallUpperHalf.gameObject.SetActive(false);

        if (stallCooldown != null)
            stallCooldown.color = cooldownColor;

        float elapsed = 0f;

        // Gradually transition color from cooldownColor to normalColor
        while (elapsed < cooldownDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / cooldownDuration);

            if (stallCooldown != null)
                stallCooldown.color = Color.Lerp(cooldownColor, normalColor, t);

            yield return null;
        }

        // Restore visuals
        if (stallUpperHalf != null)
            stallUpperHalf.gameObject.SetActive(true);

        isCoolingDown = false;

        if (stallUI != null && stallUI.isPlayerNearby)
        {
            stallUI.SetBlinking(true);
        }
    }
}
