using System;
using RPGDemo.Inventories;
using RPGDemo.Inventories.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPGDemo.Shops
{
    public class ShopSlotUI : SlotUI
    {
        [SerializeField] TextMeshProUGUI txt_itemName = null;
        [SerializeField] TextMeshProUGUI txt_itemPrice = null;
        [SerializeField] TextMeshProUGUI txt_itemStockAmount = null;
        [SerializeField] Button btn_buy;


        [SerializeField] private int slotIndex = 0;
        [SerializeField] InventoryItemIcon itemIcon = null;

        Shop.ShopItem shopItem = null;
        ShopUI shopUI = null;

        public void Setup(Shop.ShopItem shopItem, int slotIndex, ShopUI shopUI)
        {
            this.shopItem = shopItem;
            this.slotIndex = slotIndex;
            this.shopUI = shopUI;

            btn_buy.onClick.AddListener(() =>
            {
                shopUI.ShowBuyPanel(shopItem);
            });

            if (itemIcon != null)
            {
                itemIcon.SetItem(shopItem.item);
            }
            Refresh();
        }

        public void Refresh()
        {
            if (shopItem != null)
            {
                txt_itemName.text = shopItem.item.GetDisplayName();
                txt_itemPrice.text = "单价:" + shopItem.item.GetPrice().ToString();
                txt_itemStockAmount.text = "库存:" + shopItem.availableAmount.ToString();
            }
        }

        public override int GetAmount()
        {
            return shopItem.availableAmount;
        }

        public override InventoryItem GetItem()
        {
            return shopItem.item;

        }

        public Shop.ShopItem GetShopItem()
        {
            return shopItem;
        }
    }
}
