using System.Collections.Generic;
using RPGDemo.Inventories;
using UnityEngine;

namespace RPGDemo.Shops
{
    [CreateAssetMenu(fileName = "New ShopConfig", menuName = "RPGDem/Shop/New Shop Config")]
    public class ShopConfig : ScriptableObject
    {
        [SerializeField] ShopItemConfig[] configs;

        public IEnumerable<ShopItemConfig> GetShopItemConfigs()
        {
            foreach (var itemConfig in configs)
            {
                yield return itemConfig;
            }
        }


        [System.Serializable]
        public class ShopItemConfig
        {
            public InventoryItem item;
            public int stockAmount;
        }

    }
}
