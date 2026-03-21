using RPGDemo.Inventories;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPGDemo.Quests
{
    public class RewardSlotUI:MonoBehaviour
    {
        [SerializeField] private Sprite goldImage;
        [SerializeField] private Sprite expImage;
        
        [SerializeField] Image itemIcon;
        [SerializeField] TextMeshProUGUI textQuantity;
        [SerializeField] TextMeshProUGUI textItemName;
        public void Setup(QuestSO.RewardItem itemData)
        {
            itemIcon.sprite = itemData.item.GetIcon();
            textQuantity.text = "X" + itemData.quantity.ToString();
            textItemName.text = itemData.item.GetDisplayName();
        }

        public void SetupGold(int quantity)
        {
            itemIcon.sprite = goldImage;
            textQuantity.text = "X" + quantity.ToString();
            textItemName.text = "金币";
        }
        public void SetupExp(int quantity)
        {
            itemIcon.sprite = expImage;
            textQuantity.text ="X" + quantity.ToString();
            textItemName.text = "经验";
        }
    }
}