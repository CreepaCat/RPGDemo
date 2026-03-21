using UnityEngine;
using TMPro;
using UnityEngine.UI;
using RPGDemo.Inventories;
using Unity.VisualScripting;

namespace RPGDemo.Shops
{
    public class BuyItemPanel : MonoBehaviour
    {
        [SerializeField] SellItemSlotUI itemSlotUI = null;
        [SerializeField] TextMeshProUGUI txt_itemDisplayName = null;
        [SerializeField] TextMeshProUGUI txt_totalPrice = null;
        [SerializeField] TextMeshProUGUI txt_maxQuantity = null;
        [SerializeField] TextMeshProUGUI txt_currentQuantity = null;
        [SerializeField] Slider quantitySlider = null;
        [SerializeField] Button btn_buy = null;
        private int _currentQuantity = 0;

        Shop.ShopItem currentShopItem = null;

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
            btn_buy.onClick.AddListener(OnClickBuyButton);
        }

        private void OnDisable()
        {
            quantitySlider.onValueChanged.RemoveListener(HandleQuantityChanged);
            btn_buy.onClick.RemoveListener(OnClickBuyButton);
        }
        public void SetBuyItemQuantity(Shop.ShopItem shopItem)
        {
            currentShopItem = shopItem;
            if (shopItem == null) return;
            //设置显示图标
            itemSlotUI.SetItem(shopItem.item);
            //商店中该物品的数量
            quantitySlider.maxValue = shopItem.availableAmount;
            txt_itemDisplayName.text = shopItem.item.GetDisplayName();
            txt_maxQuantity.text = quantitySlider.maxValue.ToString();


            quantitySlider.value = 1;
            UpdateCurrentQuantity(1, shopItem.item);
        }

        public int GetQuantity()
        {
            return _currentQuantity;
        }

        private void OnClickBuyButton()
        {
            //var item = itemSlotUI.GetItem();
            if (currentShopItem == null)
                return;
            var shopUI = GetComponentInParent<ShopUI>();
            if (!shopUI.Buy(currentShopItem, _currentQuantity))
                return;

            gameObject.SetActive(false); //交易后关闭面板
        }

        private void HandleQuantityChanged(float amount)
        {
            var item = itemSlotUI.GetItem();
            if (item == null) return;
            UpdateCurrentQuantity(amount, item);

        }

        private void UpdateCurrentQuantity(float amount, InventoryItem item)
        {
            txt_currentQuantity.text = quantitySlider.value.ToString();
            _currentQuantity = (int)amount;

            var totalPrice = item.GetPrice() * _currentQuantity;

            if (!Purse.GetPlayerPurse().CanAfford(totalPrice))
            {
                txt_totalPrice.color = Color.red;
                btn_buy.interactable = false;
            }
            else
            {
                txt_totalPrice.color = Color.white;
                btn_buy.interactable = true;
            }
            txt_totalPrice.text = "总价:" + totalPrice.ToString();
        }
    }

}
