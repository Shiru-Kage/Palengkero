using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Transform inventoryPanel;
    [SerializeField] private GameObject itemSlotPrefab;

    [Header("TextMeshPro References")]
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private TextMeshProUGUI nutritionText;
    [SerializeField] private TextMeshProUGUI satisfactionText;
    [SerializeField] private TextMeshProUGUI flavorText;
    [SerializeField] private TextMeshProUGUI itemNameText;

    [Header("Selected Item Display")]
    [SerializeField] private Image selectedItemImage;
    [SerializeField] private GameObject informationPanel;

    private void OnEnable()
    {
        Inventory.Instance.OnInventoryUpdated += DisplayInventory;
    }

    private void OnDisable()
    {
        Inventory.Instance.OnInventoryUpdated -= DisplayInventory;
    }
    void Awake()
    {
        informationPanel.SetActive(false);
    }
    private void Start()
    {
        DisplayInventory();
    }

    public void DisplayInventory()
    {
        foreach (Transform child in inventoryPanel)
        {
            Destroy(child.gameObject);
        }

        foreach (var inventoryItem in Inventory.Instance.GetAllItems())
        {
            GameObject itemSlot = Instantiate(itemSlotPrefab, inventoryPanel);

            Button button = itemSlot.GetComponent<Button>();

            Transform buttonChild = button.transform.GetChild(0);
            Image itemImage = buttonChild.GetComponent<Image>();

            if (itemImage != null && inventoryItem.itemData.icon != null)
            {
                itemImage.sprite = inventoryItem.itemData.icon;
            }

            if (button != null)
            {
                button.onClick.AddListener(() => OnItemSelected(inventoryItem));
            }
        }
    }

    private void OnItemSelected(InventoryItem inventoryItem)
    {
        ShowInformationPanel();
        Debug.Log("Item Selected: " + inventoryItem.itemData.itemName);

        if (selectedItemImage != null && inventoryItem.itemData.icon != null)
        {
            selectedItemImage.sprite = inventoryItem.itemData.icon;
        }

        if (priceText != null)
        {
            priceText.text = "Price: " + inventoryItem.itemData.price.ToString("F2");
        }
        if (nutritionText != null)
        {
            nutritionText.text = "Nutrition: " + inventoryItem.itemData.nutrition.ToString();
        }
        if (satisfactionText != null)
        {
            satisfactionText.text = "Satisfaction: " + inventoryItem.itemData.satisfaction.ToString();
        }
        if (flavorText != null)
        {
            flavorText.text = "Flavor: " + inventoryItem.itemData.flavorText;
        }
        if (itemNameText != null)
        {
            itemNameText.text = inventoryItem.itemData.itemName;
        }
    }

    public void ShowInformationPanel()
    {
        if (informationPanel != null)
        {
            informationPanel.SetActive(!informationPanel.activeSelf);
        }
        else
        {
            Debug.LogWarning("Information panel is not assigned.");
        }
    }
}
