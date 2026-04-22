using UnityEngine;
using RPGDemo.Inventories;
using RPGDemo.InteractionSystem;
using System.Collections.Generic;
using System;
using RPGDemo.Saving;
using Newtonsoft.Json.Linq;

namespace RPGDemo.Shops
{
    public class Shop : MonoBehaviour, ISaveable
    {
        [SerializeField] ShopConfig shopConfig = null;
        private List<ShopItem> shopItems = new();

        protected void Awake()
        {
            //原配置的shopItems，以及玩家出售的不在shopConfig里的item list
            if (shopConfig != null && shopItems.Count < 1) //确保在读档之前使用初始化配置
            {
                InitShopItems();
            }

        }

        private void InitShopItems()
        {
            foreach (var config in shopConfig.GetShopItemConfigs())
            {
                ShopItem shopItem = new ShopItem()
                {
                    item = config.item,
                    availableAmount = config.stockAmount
                };
                shopItems.Add(shopItem);
            }
        }


        [System.Serializable]
        public class ShopItem
        {
            public InventoryItem item;
            public int availableAmount;

        }
        public List<ShopItem> GetShopItems() => shopItems;

        public IEnumerable<ShopItem> GetFilterItems(ItemCategory selectedItemCategory)
        {
            foreach (var shopItem in shopItems)
            {
                if (shopItem.item.GetCategory() == selectedItemCategory)
                {
                    yield return shopItem;
                }
            }
        }

        public bool SellToShop(InventoryItem item, int amount)
        {
            if (item == null)
            {
                Debug.LogWarning("SellToShop失败：item为空。");
                return false;
            }

            if (amount <= 0)
            {
                Debug.LogWarning($"SellToShop失败：amount无效，当前值 {amount}。");
                return false;
            }

            bool addedToExistingStock = false;
            for (int i = 0; i < shopItems.Count; i++)
            {
                if (shopItems[i] == null || shopItems[i].item != item) continue;

                shopItems[i].availableAmount += amount;
                addedToExistingStock = true;
                break;
            }

            if (!addedToExistingStock)
            {
                shopItems.Add(new ShopItem
                {
                    item = item,
                    availableAmount = amount
                });
            }

            //从inventory移除相应数量物品
            Inventory.GetPlayerInventory().RemoveItem(item, amount);

            Purse.GetPlayerPurse()?.AddMoney(item.GetPrice() * amount);

            return true;
            // OnShopItemsUpdated?.Invoke();

        }

        public bool BuyFromShop(ShopItem shopItem, int amountToBuy)
        {
            if (shopItem == null)
            {
                Debug.LogWarning("SellToShop失败：item为空。");
                return false;
            }

            if (amountToBuy <= 0)
            {
                Debug.LogWarning($"SellToShop失败：amount无效，当前值 {amountToBuy}。");
                return false;
            }

            Debug.Log($"从商店购买物品{shopItem.item.GetDisplayName()}数量{amountToBuy}");
            //从商店购物检查
            //1、库存检查
            if (shopItem.availableAmount < amountToBuy) return false;
            //2、资金检查
            var totalPrice = shopItem.item.GetPrice() * amountToBuy;
            var purse = Purse.GetPlayerPurse();
            if (!purse.CanAfford(totalPrice))
            {
                Debug.Log($"没有足够资金");
                return false;
            }

            //3、背包空间检查
            //购买商品不能用单slot检查，要用批量物品格检查，因为购买的数量分割后可能超过了背包剩余格子空间
            var playerInventory = Inventory.GetPlayerInventory();
            Dictionary<InventoryItem, int> itemDict = new();
            itemDict[shopItem.item] = amountToBuy;
            if (!playerInventory.AddItemDict(itemDict))
            {
                Debug.Log($"背包没有足够空间");
                return false;
            }

            //扣钱并加入背包
            shopItem.availableAmount -= amountToBuy;
            if (shopItem.availableAmount <= 0)
            {
                //todo:显示售罄，或移除
                shopItems.Remove(shopItem);

            }
            purse.SpendMoney(totalPrice);

            return true;

        }
        //通过对话打开商店
        public void Interact()
        {
            Debug.Log("Interacting with shop...");
            Player.GetInstance().GetComponent<Shopper>()?.InteractWithShop(this);
        }

        public int GetQuantity(InventoryItem item)
        {
            foreach (var shopItem in shopItems)
            {
                if (shopItem.item == item)
                {
                    return shopItem.availableAmount;
                }
            }
            return 0;
        }

        [System.Serializable]
        private struct ShopItemSaveData
        {
            public string itemID;
            public int availableAmount;
        }

        private enum SaveData
        {
            ShopItems
        }

        JToken ISaveable.CapatureStateAsJToken()
        {
            JObject state = new JObject();
            IDictionary<string, JToken> stateDict = state;

            List<ShopItemSaveData> itemsToSave = new List<ShopItemSaveData>();
            foreach (var shopItem in shopItems)
            {
                if (shopItem == null || shopItem.item == null) continue;
                if (shopItem.availableAmount <= 0) continue;

                itemsToSave.Add(new ShopItemSaveData
                {
                    itemID = shopItem.item.GetItemID(),
                    availableAmount = shopItem.availableAmount
                });
            }

            stateDict[SaveData.ShopItems.ToString()] = JToken.FromObject(itemsToSave);
            return state;
        }

        void ISaveable.RestoreStateFromJToken(JToken s)
        {
            JObject state = s as JObject;
            if (state == null) return;

            IDictionary<string, JToken> stateDict = state;
            if (!stateDict.ContainsKey(SaveData.ShopItems.ToString())) return;

            List<ShopItemSaveData> savedItems = stateDict[SaveData.ShopItems.ToString()]
                .ToObject<List<ShopItemSaveData>>();
            if (savedItems == null) return;

            shopItems.Clear();

            foreach (var savedItem in savedItems)
            {
                if (string.IsNullOrWhiteSpace(savedItem.itemID)) continue;
                if (savedItem.availableAmount <= 0) continue;

                InventoryItem item = InventoryItem.GetItemFromID(savedItem.itemID);
                if (item == null) continue;

                shopItems.Add(new ShopItem
                {
                    item = item,
                    availableAmount = savedItem.availableAmount
                });
            }
            //刷新UI
        }

    }
}
