using RPGDemo.Inventories;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPGDemo.Shops
{
    public class SellItemPanel : MonoBehaviour
    {
        [SerializeField] SellItemSlotUI sellItemSlotUI = null;
        [SerializeField] TextMeshProUGUI txt_itemDisplayName = null;
        [SerializeField] TextMeshProUGUI txt_totalPrice = null;
        [SerializeField] TextMeshProUGUI txt_maxQuantity = null;
        [SerializeField] TextMeshProUGUI txt_currentQuantity = null;
        [SerializeField] Slider quantitySlider = null;

        [SerializeField] Button btn_sell = null;
        // [SerializeField] Button btn_cancel = null;

        private int _currentQuantity = 0;

        private void Awake()
        {
            quantitySlider.wholeNumbers = true;
            quantitySlider.minValue = 1;
        }
        private void Start()
        {
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            quantitySlider.onValueChanged.AddListener(HandleQuantityChanged);
            btn_sell.onClick.AddListener(OnClickSellButton);
        }

        private void OnDisable()
        {
            quantitySlider.onValueChanged.RemoveListener(HandleQuantityChanged);
            btn_sell.onClick.RemoveListener(OnClickSellButton);
        }
        public void SetSellItemQuantity(InventoryItem item)
        {
            if (item == null)
            {
                txt_itemDisplayName.text = "无物品";
                quantitySlider.maxValue = 0;
            }
            else
            {
                //*此处的slot仅用作物品展示，接受数为1，因此每次加进来都需要放回背包
                Inventory.GetPlayerInventory().AddItemToFirstFoundSlot(item, 1);
                quantitySlider.maxValue = Inventory.GetPlayerInventory().GetTotalAmount(item);
                txt_itemDisplayName.text = item.GetDisplayName();

            }
            // txt_maxQuantity.text = quantitySlider.maxValue.ToString();
            // quantitySlider.value = 1;
            UpdateSliderQuantity(1, item);
        }

        public int GetQuantity()
        {
            return _currentQuantity;
        }

        private void OnClickSellButton()
        {
            var item = sellItemSlotUI.GetItem();
            GetComponentInParent<ShopUI>().Sell(item, _currentQuantity);

            //检查背包是否有足够物品，若没有，将物品置空
            if (Inventory.GetPlayerInventory().GetTotalAmount(item) < 1)
            {
                sellItemSlotUI.RemoveItems(1);
                quantitySlider.maxValue = 0;
            }
            else
            {
                quantitySlider.maxValue = Inventory.GetPlayerInventory().GetTotalAmount(item);

            }


            UpdateSliderQuantity(1, item);
        }

        private void HandleQuantityChanged(float amount)
        {
            var item = sellItemSlotUI.GetItem();
            if (item == null) return;
            // UpdateSliderQuantity(amount, item);
            txt_currentQuantity.text = quantitySlider.value.ToString();
            _currentQuantity = (int)amount;
            txt_totalPrice.text = "总价:" + (item?.GetPrice() * _currentQuantity).ToString();

        }

        private void UpdateSliderQuantity(float currentQuantity, InventoryItem item)
        {
            txt_maxQuantity.text = quantitySlider.maxValue.ToString();
            quantitySlider.value = currentQuantity;

            txt_currentQuantity.text = quantitySlider.value.ToString();
            _currentQuantity = (int)currentQuantity;
            txt_totalPrice.text = "总价:" + (item?.GetPrice() * _currentQuantity).ToString();
        }
    }
}
