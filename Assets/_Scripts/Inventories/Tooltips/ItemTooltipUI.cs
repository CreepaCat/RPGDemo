using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPGDemo.Inventories.Tooltips
{
    public class ItemTooltipUI : MonoBehaviour
    {
        [SerializeField] private Image itemImage;
        [SerializeField] private TextMeshProUGUI itemNameText;
        [SerializeField] private TextMeshProUGUI categoryText;
        [SerializeField] private TextMeshProUGUI priceText;
        [SerializeField] private TextMeshProUGUI itemDescriptionText;

        public void Setup(InventoryItem item)
        {
            itemImage.sprite = item.GetIcon();
            itemNameText.text = item.GetDisplayName();
            categoryText.text = item.GetCategoryString();
            priceText.text = item.GetPrice().ToString();
            itemDescriptionText.text = item.GetDescription();
        }
    }
}
