using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ArchiveItems : InventoryUI
{
    private void Awake()
    {
        ArchiveManager.Instance.SetArchiveItemsReference(this);
    }

    public void UnlockItemInArchive(InventoryItem purchasedItem)
    {
        foreach (Transform child in GetSlotContainer())
        {
            Image itemImage = child.transform.Find("InfoPanel/InfoPanelContainer/Upper Part/Item Frame/Item Icon")?.GetComponent<Image>();
            TextMeshProUGUI itemName = child.transform.Find("InfoPanel/InfoPanelContainer/Upper Part/Item Frame/Item Icon/Item Name")?.GetComponent<TextMeshProUGUI>();

            if (itemImage != null && itemName != null && itemName.text == purchasedItem.itemData.itemName)
            {
                itemImage.color = Color.white;
            }
        }

        Debug.Log($"{purchasedItem.itemData.itemName} has been unlocked in the archive UI.");
    }

    public override void DisplayInventory()
    {
        base.DisplayInventory();

        foreach (Transform child in GetSlotContainer())
        {
            Destroy(child.gameObject);
        }

        foreach (var item in ItemDatabaseManager.Instance.itemDatabase.items)
        {
            CreateArchiveSlot(item);
        }
        foreach (Transform child in GetSlotContainer())
        {
            TextMeshProUGUI itemNameText = child.transform.Find("InfoPanel/InfoPanelContainer/Upper Part/Item Frame/Item Icon/Item Name")?.GetComponent<TextMeshProUGUI>();
            
            if (itemNameText != null)
            {
                string itemName = itemNameText.text;

                bool isUnlocked = ArchiveManager.Instance.IsItemUnlocked(itemName);

                Image itemImage = child.transform.Find("InfoPanel/InfoPanelContainer/Upper Part/Item Frame/Item Icon")?.GetComponent<Image>();
                if (itemImage != null)
                {
                    itemImage.color = isUnlocked ? Color.white : Color.black;
                }
            }
        }
    }

    private void CreateArchiveSlot(ItemData itemData)
    {
        GameObject slot = Instantiate(GetItemSlotPrefab(), GetSlotContainer());

        Transform itemIconTransform = slot.transform.Find("ItemIcon"); 
        Image itemImage = itemIconTransform?.GetComponent<Image>(); 

        if (itemImage != null)
        {
            itemImage.sprite = itemData.icon;

            bool isUnlocked = ArchiveManager.Instance.IsItemUnlocked(itemData.id); 

            if (isUnlocked)
            {
                itemImage.color = Color.white;
            }
            else
            {
                itemImage.color = Color.black;
            }
        }

        Button button = slot.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(() => OnItemSelected(itemData));
        }
    }


    private void OnItemSelected(ItemData itemData)
    {
        GetInformationPanel().SetActive(true);
        Debug.Log($"Selected Item: {itemData.itemName}");

        bool isUnlocked = ArchiveManager.Instance.IsItemUnlocked(itemData.id);  

        if (isUnlocked)
        {
            if (GetSelectedImage() != null && itemData.icon != null)
            {
                GetSelectedImage().sprite = itemData.icon;
                GetSelectedImage().color = Color.white;  
            }

            if (GetItemNameText() != null)
            {
                GetItemNameText().text = itemData.itemName;
            }

            if (GetPriceText() != null)
            {
                GetPriceText().text = $"Price: {itemData.price.ToString("F2")}";
            }

            if (GetNutritionText() != null)
            {
                int nutritionValue = itemData.nutrition;
                GetNutritionText().text = $"Nutrition: {nutritionValue}";
                GetNutritionText().color = Color.white;

                if (nutritionValue < 0)
                {
                    GetNutritionText().color = Color.red; 
                }
                else
                {
                    GetNutritionText().color = Color.green; 
                }
            }

            if (GetSatisfactionText() != null)
            {
                int satisfactionValue = itemData.satisfaction;
                GetSatisfactionText().text = $"Satisfaction: {satisfactionValue}";
                GetSatisfactionText().color = Color.white;

                if (satisfactionValue < 0)
                {
                    GetSatisfactionText().color = Color.red;  
                }
                else
                {
                    GetSatisfactionText().color = Color.green;  
                }
            }

            if (GetflavorText() != null)
            {
                GetflavorText().text = $"Flavor: {itemData.flavorText}";
            }
        }
        else
        {
            if (GetSelectedImage() != null)
            {
                GetSelectedImage().sprite = itemData.icon;
                GetSelectedImage().color = Color.black;
            }

            if (GetItemNameText() != null)
            {
                GetItemNameText().text = "???";
            }

            if (GetPriceText() != null)
            {
                GetPriceText().text = "Price: ???";
            }

            if (GetNutritionText() != null)
            {
                GetNutritionText().color = Color.white;
                GetNutritionText().text = "Nutrition: ???";
            }

            if (GetSatisfactionText() != null)
            {
                GetSatisfactionText().color = Color.white;
                GetSatisfactionText().text = "Satisfaction: ???";
            }

            if (GetflavorText() != null)
            {
                GetflavorText().text = "Flavor: ???";
            }
        }
    }
}
