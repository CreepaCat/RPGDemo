using System;
using RPGDemo.Inventories;
using UnityEngine;

namespace RPGDemo.Shops
{
    public class Shopper : MonoBehaviour
    {
        [SerializeField] Shop currentShop = null;

        public event Action OnShopChanged;

        public void InteractWithShop(Shop newShop)
        {
            if (newShop == currentShop) return;
            currentShop = newShop;
            OnShopChanged?.Invoke();
        }

        public Shop GetCurrentShop()
        {
            return currentShop;
        }

        public bool BuyFromShop(Shop.ShopItem item, int amount)
        {
            if (currentShop == null) return false;

            if (currentShop.BuyFromShop(item, amount))
            {
                OnShopChanged?.Invoke();
                return true;
            }
            return false;

        }

        public void SellToShop(InventoryItem item, int amount)
        {
            if (currentShop == null) return;

            if (currentShop.SellToShop(item, amount))
            {

                OnShopChanged?.Invoke();
                Debug.Log($"成功卖出物品{item.GetDisplayName()}数量{amount}");
            }


        }
    }
}
