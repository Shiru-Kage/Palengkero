using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class StallUI : MonoBehaviour
{
    [SerializeField] private StallCooldown stallCooldown;
    [SerializeField] private StallData stallData;

    public StallData GetStallData() => stallData;

    [Header("Stall UI")]
    [SerializeField] private Image upperStallHalf;
    [SerializeField] private Image lowerStallHalf;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private HighlightEffect highlightEffect;
    [SerializeField] private AudioClip purchaseSound;

    [Header("Purcahsed Item Settings")]
    [SerializeField] private GameObject purchasedItem;
    [SerializeField] private float purchaseEffectScale = 1.2f;
    [SerializeField] private float purchaseEffectMoveY = 50f;
    [SerializeField] private float purchaseEffectDuration = 0.5f;

    private Transform stallInnerUIContainer;
    private GameObject stallInnerUICanvasObject;
    private GameObject unableToPurchase;
    private GameObject informationPanel;

    private Button purchaseButton;
    private Button haggleButton;

    private TextMeshProUGUI itemName;
    private Image itemIcon;
    private TextMeshProUGUI stockInfo;
    private TextMeshProUGUI priceInfo;
    private TextMeshProUGUI descriptionInfo;

    private Button[] itemButtons;
    private Stall currentStall;

    [HideInInspector] public bool isPlayerNearby = false;


    private void Start()
    {
        if (upperStallHalf != null) upperStallHalf.sprite = stallData.upperStallIcon;
        if (lowerStallHalf != null) lowerStallHalf.sprite = stallData.lowerStallIcon;
        if (backgroundImage != null) backgroundImage.sprite = stallData.stallBackground;
    }

    public void AssignUIContainer(Transform container) => stallInnerUIContainer = container;
    public void SetUICanvasObject(GameObject canvasObject) => stallInnerUICanvasObject = canvasObject;
    public Button[] GetItemButtons() => itemButtons;

    private void CheckAndTogglePlayerMovement()
    {
        var player = Object.FindFirstObjectByType<PlayerController>();
        if (player != null)
        {
            bool uiActive = stallInnerUICanvasObject != null && stallInnerUICanvasObject.activeSelf;
            player.ToggleMovement(!uiActive);
        }
    }

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

        Transform confirmationPanel = stallInnerUICanvasObject.transform.Find("Purchase Confirmation");
        if (confirmationPanel != null)
        {
            unableToPurchase = confirmationPanel.gameObject;
        }
        else
        {
            Debug.LogWarning("confirmationPanel is null");
        }

        Transform boxes = stallInnerUIContainer.Find("Stall Tables");

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
        CheckAndTogglePlayerMovement();
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
            purchaseButton.onClick.AddListener(() =>
            {
                var (item, _) = currentStall.GetItemAndStock(currentStall.SelectedItemIndex);

                if (item != null)
                {
                    currentStall.OnPurchaseButtonPressed(BuyerType.Player);
                    if (TutorialManager.Instance != null)
                    {
                        TutorialManager.Instance.NotifyAction("Purchase");
                    }
                    PlayPurchaseEffect(item);
                }
            });
        }

        if (haggleButton != null)
        {
            haggleButton.onClick.RemoveAllListeners();
            haggleButton.onClick.AddListener(() =>
            {
                currentStall.TryStartHaggling();
                HideDetailsAfterHaggle();
            });
        }
    }

    private void PlayPurchaseEffect(ItemData purchasedData)
    {
        if (purchasedItem == null || purchasedData == null) return;

        purchasedItem.SetActive(true);

        Image purchasedImage = purchasedItem.GetComponent<Image>();
        if (purchasedImage != null)
            purchasedImage.sprite = purchasedData.icon;

        RectTransform rect = purchasedItem.GetComponent<RectTransform>();
        CanvasGroup canvasGroup = purchasedItem.GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = purchasedItem.AddComponent<CanvasGroup>();

        rect.localScale = Vector3.one;
        rect.anchoredPosition = Vector2.zero;
        canvasGroup.alpha = 1f;

        LeanTween.scale(rect, Vector3.one * purchaseEffectScale, purchaseEffectDuration * 0.6f).setEaseOutBack();

        LeanTween.moveY(rect, rect.anchoredPosition.y + purchaseEffectMoveY, purchaseEffectDuration).setEaseOutQuad();

        LeanTween.alphaCanvas(canvasGroup, 0f, purchaseEffectDuration).setOnComplete(() =>
        {
            purchasedItem.SetActive(false);
        });
    }

    public void HideDetailsAfterPurchase()
    {
        if (purchaseButton != null) purchaseButton.gameObject.SetActive(false);
        if (haggleButton != null) haggleButton.gameObject.SetActive(false);
        if (informationPanel != null) informationPanel.SetActive(false);
        if (stallInnerUICanvasObject != null) stallInnerUICanvasObject.SetActive(false);
        CheckAndTogglePlayerMovement();
    }

    public void HideDetailsAfterHaggle()
    {
        if (purchaseButton != null) purchaseButton.gameObject.SetActive(false);
        if (haggleButton != null) haggleButton.gameObject.SetActive(false);
        if (informationPanel != null) informationPanel.SetActive(false);
    }

    private void DisplayDetailsAfterHaggle()
    {
        if (purchaseButton != null) purchaseButton.gameObject.SetActive(true);
        if (haggleButton != null) haggleButton.gameObject.SetActive(true);
        if (informationPanel != null) informationPanel.SetActive(true);
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

            Image iconImage = button.transform.Find("Icon")?.GetComponent<Image>();
            if (iconImage != null)
            {
                iconImage.sprite = items[i].icon;
                if (stocks[i] <= 0)
                {
                    iconImage.color = Color.black;
                }
                else
                {
                    iconImage.color = Color.white;
                }
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
    public void UpdateDisplayItemsForPlayer(ItemData[] items, int[] stocks, Stall stall)
    {
        var player = Object.FindFirstObjectByType<PlayerController>();
        if (player != null && player.CurrentInteractable == stall)
        {
            DisplayItems(items, stocks);
        }
    }
    public void RefreshItemDetails(ItemData item, int stock)
    {
        DisplayItemDetails(item, stock);
    }

    private void DisplayItemDetails(ItemData item, int stock)
    {
        if (stock <= 0)
        {
            itemIcon.sprite = item.icon;
            itemIcon.color = Color.black;

            if (itemName != null) itemName.text = item.itemName;
            if (stockInfo != null)
            {
                stockInfo.text = "Item out of stock";
                stockInfo.alignment = TMPro.TextAlignmentOptions.Center;
                stockInfo.fontSize = 50;
                stockInfo.color = Color.red;
            }
            if (priceInfo != null) priceInfo.text = "";
            if (descriptionInfo != null) descriptionInfo.text = "";
            if (haggleButton != null) haggleButton.gameObject.SetActive(false);
            return;
        }

        itemIcon.sprite = item.icon;
        itemIcon.color = Color.white;

        itemName.text = item.itemName;
        stockInfo.text = $"Stock amount: {stock}x";
        stockInfo.alignment = TMPro.TextAlignmentOptions.Left;
        stockInfo.fontSize = 25;
        stockInfo.color = Color.black;

        float price = item.price;
        float finalPrice = price;

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
        DisplayDetailsAfterHaggle();
    }

    public void ShowUnableToPurchasePanel()
    {
        if (unableToPurchase != null)
        {
            unableToPurchase.SetActive(true);
            StartCoroutine(HideUnableToPurchaseAfterDelay(2f));
        }
    }

    private IEnumerator HideUnableToPurchaseAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (unableToPurchase != null)
            unableToPurchase.SetActive(false);
    }

    public void SetHighlight(bool active)
    {
        if (highlightEffect != null)
        {
            highlightEffect.SetBlinking(active);
        }
    }
    
    public void PlayPurchaseSound()
    {
        if (purchaseSound != null)
        {
            AudioManager.Instance.PlaySFX(purchaseSound);
        }
    }
}
