using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class StallUI : MonoBehaviour
{
    [Header("Stall Data")]
    [SerializeField] private StallData stallData;

    [Header("Stall Outer UI")]
    [SerializeField] private SpriteRenderer upperIconRenderer;
    [SerializeField] private SpriteRenderer lowerIconRenderer;
    [SerializeField] private Image backgroundRenderer;

    [Header("Stall Inner UI")]
    [SerializeField] private GameObject itemDisplay;
    [SerializeField] private Button purchaseButton;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI nutritionInfo;
    [SerializeField] private TextMeshProUGUI satisfactionInfo;

    [Header("Outline Settings")]
    [SerializeField] private Color outlineColor = Color.yellow;
    [SerializeField] private float blinkSpeed = 2f;
    [SerializeField] private float outlineThickness = 2f;

    [Header("Item Display")]
    [SerializeField] private Button[] itemButtons;

    private Outline outline;
    private Coroutine blinkRoutine;
    private Color originalColor;
    private Stall currentStall;

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

    public void SetStallReference(Stall stall)
    {
        currentStall = stall;
    }

    public void DisplayItems(ItemData[] items, int[] stocks)
    {
        for (int i = 0; i < itemButtons.Length; i++)
        {
            Button button = itemButtons[i];

            if (i >= items.Length || items[i] == null)
            {
                button.gameObject.SetActive(false);
                continue;
            }

            button.gameObject.SetActive(true);

            Image iconImage = button.GetComponentInChildren<Image>();
            if (iconImage != null)
            {
                iconImage.sprite = items[i].icon;
            }

            // Capture values locally for the lambda
            int capturedIndex = i;
            ItemData capturedItem = items[capturedIndex];
            int capturedStock = stocks[capturedIndex];

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                currentStall.SetSelectedItem(capturedIndex);
                DisplayItemDetails(capturedItem, capturedStock);
            });
        }

        // Optionally show the first item by default
        if (items.Length > 0 && items[0] != null)
        {
            DisplayItemDetails(items[0], stocks[0]);
        }
    }

    private void DisplayItemDetails(ItemData item, int stock)
    {
        if (itemName != null)
            itemName.text = item.itemName;

        if (nutritionInfo != null)
            nutritionInfo.text = $"Nutrition: {item.nutrition}";

        if (satisfactionInfo != null)
            satisfactionInfo.text = $"Satisfaction: {item.satisfaction}";

        // You could also show stock or enable the purchase button here
        // purchaseButton.interactable = stock > 0;
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
