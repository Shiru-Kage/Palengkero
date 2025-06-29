using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class StallUI : MonoBehaviour
{
    [SerializeField] private StallCooldown stallCooldown;
    [SerializeField] private StallData stallData;

    [Header("Stall UI")]
    [SerializeField] private Image upperStallHalf;
    [SerializeField] private Image lowerStallHalf;
    [SerializeField] private Image backgroundImage;

    private Transform stallInnerUIContainer;
    private GameObject stallInnerUICanvasObject;
    private GameObject informationPanel;

    private GameObject itemDisplay;
    private Button purchaseButton;
    private Button haggleButton;

    private TextMeshProUGUI itemName;
    private Image itemIcon;
    private TextMeshProUGUI stockInfo;
    private TextMeshProUGUI priceInfo;
    private TextMeshProUGUI descriptionInfo;

    private Button[] itemButtons;
    private Outline outline;
    private Coroutine blinkRoutine;
    private Color originalColor;

    private Stall currentStall; 

    [Header("Outline Settings")]
    [SerializeField] private Color outlineColor = Color.yellow;
    [SerializeField] private float blinkSpeed = 2f;
    [SerializeField] private float outlineThickness = 2f;

    [HideInInspector] public bool isPlayerNearby = false;

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

    private void Start()
    {
        if (upperStallHalf != null) upperStallHalf.sprite = stallData.upperStallIcon;
        if (lowerStallHalf != null) lowerStallHalf.sprite = stallData.lowerStallIcon;
        if (backgroundImage != null) backgroundImage.sprite = stallData.stallBackground;
    }

    public void AssignUIContainer(Transform container) => stallInnerUIContainer = container;
    public void SetUICanvasObject(GameObject canvasObject) => stallInnerUICanvasObject = canvasObject;
    public Button[] GetItemButtons() => itemButtons;

    public void SetupUIReferences()
    {
        if (stallInnerUIContainer == null)
        {
            Debug.LogError("StallInnerUIContainer not assigned!");
            return;
        }

        Transform infoPanel = stallInnerUIContainer.Find("Information Panel");
        if (infoPanel != null)
        {
            itemName = infoPanel.Find("ItemName")?.GetComponent<TextMeshProUGUI>();
            itemIcon = infoPanel.Find("ItemIcon")?.GetComponent<Image>();
            stockInfo = infoPanel.Find("Texts/Stock")?.GetComponent<TextMeshProUGUI>();
            priceInfo = infoPanel.Find("Texts/Price")?.GetComponent<TextMeshProUGUI>();
            descriptionInfo = infoPanel.Find("Texts/Flavor text")?.GetComponent<TextMeshProUGUI>();

            informationPanel = infoPanel.gameObject;
        }

        Transform boxes = stallInnerUIContainer.Find("Boxes");

        if (boxes != null)
        {
            int childCount = boxes.childCount;
            itemButtons = new Button[childCount];

            for (int i = 0; i < childCount; i++)
            {
                Transform child = boxes.GetChild(i);
                Button button = child.GetComponent<Button>();
                itemButtons[i] = button;

                if (button == null)
                {
                    Debug.LogWarning($"Child '{child.name}' of Boxes does not have a Button component.");
                }
            }
        }

        Transform haggleTransform = stallInnerUIContainer.Find("Haggle");
        if (haggleTransform != null)
            haggleButton = haggleTransform.GetComponent<Button>();

        purchaseButton = stallInnerUIContainer.parent.Find("Purchase")?.GetComponent<Button>();

        ValidateUI();
    }

    private void ValidateUI()
    {
        if (itemName == null) Debug.LogWarning("ItemName not found.");
        if (itemIcon == null) Debug.LogWarning("ItemIcon not found.");
        if (stockInfo == null) Debug.LogWarning("StockInfo not found.");
        if (priceInfo == null) Debug.LogWarning("PriceInfo not found.");
        if (descriptionInfo == null) Debug.LogWarning("DescriptionInfo not found.");
        if (haggleButton == null) Debug.LogWarning("haggleButton not found.");
        if (purchaseButton == null) Debug.LogWarning("PurchaseButton not found.");
        if (itemButtons == null || itemButtons.Length == 0) Debug.LogWarning("ItemButtons not found.");
    }

    public void SetStallReference(Stall stall)
    {
        currentStall = stall;

        if (purchaseButton != null)
        {
            purchaseButton.onClick.RemoveAllListeners();
            purchaseButton.onClick.AddListener(() => currentStall.OnPurchaseButtonPressed());
        }

        if (haggleButton != null)
        {
            haggleButton.onClick.RemoveAllListeners();
            haggleButton.onClick.AddListener(() =>
            {
                currentStall.TryStartHaggling();
            });
        }
    }

    public void HideDetailsAfterPurchase()
    {
        if (purchaseButton != null) purchaseButton.gameObject.SetActive(false);
        if (haggleButton != null) haggleButton.gameObject.SetActive(false);
        if (informationPanel != null) informationPanel.SetActive(false);
        if (stallInnerUICanvasObject != null) stallInnerUICanvasObject.SetActive(false);
    }

    public void HideDetailsAfterHaggle()
    {
        if (purchaseButton != null) purchaseButton.gameObject.SetActive(false);
        if (haggleButton != null) haggleButton.gameObject.SetActive(false);
        if (informationPanel != null) informationPanel.SetActive(false);
        if (stallInnerUIContainer != null) stallInnerUIContainer.gameObject.SetActive(false);
    }

    public void DisplayDetailsAfterHaggle()
    {
        if (purchaseButton != null) purchaseButton.gameObject.SetActive(true);
        if (haggleButton != null) haggleButton.gameObject.SetActive(true);
        if (informationPanel != null) informationPanel.SetActive(true);
        if (stallInnerUIContainer != null) stallInnerUIContainer.gameObject.SetActive(true);
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

            // ✅ Find and set the icon from the child named "ItemIcon"
            Image iconImage = button.transform.Find("Icon")?.GetComponent<Image>();
            if (iconImage != null)
            {
                iconImage.sprite = items[i].icon;
            }
            else
            {
                Debug.LogWarning($"Icon not found on {button.name}");
            }

            int capturedIndex = i;
            ItemData capturedItem = items[capturedIndex];
            int capturedStock = stocks[capturedIndex];

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                currentStall.SetSelectedItem(capturedIndex);
                DisplayItemDetails(capturedItem, capturedStock);
                if (purchaseButton != null) purchaseButton.gameObject.SetActive(true);
            });
        }
    }


    private void DisplayItemDetails(ItemData item, int stock)
    {
        itemName.text = item.itemName;
        itemIcon.sprite = item.icon;
        stockInfo.text = $"Stock amount: {stock}x";

        float price = item.price;
        float finalPrice = price;

        // ✅ Check this stall's local discount, not HaggleSystem global
        if (currentStall != null && item.id == currentStall.GetDiscountedItemId())
        {
            finalPrice *= 0.5f;
            finalPrice = Mathf.Round(finalPrice);
        }

        priceInfo.text = finalPrice < price
            ? $"Price: <color=red><s>₱{price}</s></color> ₱{finalPrice}"
            : $"Price: ₱{price}";

        descriptionInfo.text = item.flavorText;
    }

    public void UpdateSelectedItemPrice(float discountedPrice)
    {
        if (currentStall == null || priceInfo == null) return;

        var (item, _) = currentStall.GetItemAndStock(currentStall.SelectedItemIndex);
        if (item == null) return;

        float originalPrice = item.price;

        priceInfo.text = discountedPrice < originalPrice
            ? $"Price: <color=red><s>₱{originalPrice}</s></color> ₱{discountedPrice}"
            : $"Price: ₱{originalPrice}";
    }

    public void SetBlinking(bool shouldBlink)
    {
        if (outline == null) return;
        if (stallCooldown != null && stallCooldown.isCoolingDown)
        {
            StopBlinkingOutline();
            return;
        }

        if (shouldBlink) StartBlinkingOutline();
        else StopBlinkingOutline();
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
