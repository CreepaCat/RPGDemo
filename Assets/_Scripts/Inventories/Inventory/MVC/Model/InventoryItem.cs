using System.Collections.Generic;
using RPGDemo.Core;
using RPGDemo.Inventories.Pickups;
using UnityEngine;

namespace RPGDemo.Inventories
{


    //作为泛型类型传递
    // [CreateAssetMenu(fileName = "New InventoryItem", menuName = "InventoryItem/New Item", order = 0)]
    public abstract class InventoryItem : Item
    {
        [SerializeField] Pickup _pickupPrefab = null;

        [SerializeField] private bool _isStackable = false;
        [SerializeField] private int _maxStackableAmount;

        [Header("种类和价格")]
        [SerializeField] private ItemCategory _itemCategory;
        [SerializeField] private int _price;



        //CACHE
        private static Dictionary<string, InventoryItem> _lookup;

        /// <summary>
        ///使用静态字典和方法来避免单例
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns></returns>
        public static InventoryItem GetItemFromID(string itemID)
        {
            //先构建词典
            if (_lookup == null)
            {
                _lookup = new Dictionary<string, InventoryItem>();
                //从资源文件夹加载所有item
                InventoryItem[] inventoryItems = Resources.LoadAll<InventoryItem>("");
                for (int i = 0; i < inventoryItems.Length; i++)
                {
                    if (_lookup.ContainsKey(inventoryItems[i].GetItemID()))
                    {
                        Debug.LogErrorFormat("有重复id的inventoryItem,id{0},name{1}", inventoryItems[i].GetItemID(), inventoryItems[i].GetDisplayName());
                        continue;
                    }
                    _lookup.Add(inventoryItems[i].GetItemID(), inventoryItems[i]);
                }
            }
            //返回结果
            if (string.IsNullOrWhiteSpace(itemID) || !_lookup.ContainsKey(itemID)) return null;
            return _lookup[itemID];
        }

        //  public int GetMaxStackableNumber()

        public string GetItemID() => _itemID;
        public string GetDisplayName() => _displayName;
        public string GetDescription() => _description;
        public Sprite GetIcon() => _icon;
        public ItemCategory GetItemCategory() => _itemCategory;
        public bool IsStackable() => _isStackable;
        public int GetMaxStackableAmount() => _isStackable ? _maxStackableAmount : 1;

        public ItemCategory GetCategory() => _itemCategory;
        public int GetPrice() => _price;

        public Pickup SpawnPickup(Vector3 position, int amount)
        {
            //todo:物品生成在大世界的粒子特效
            if (_pickupPrefab == null)
            {
                Debug.LogError($"InventoryItem {_displayName} 的PickupPrefab为空");
                return null;
            }
            Pickup pickup = Instantiate(_pickupPrefab);
            pickup.transform.position = position;
            pickup.Setup(this, amount);
            return pickup;
        }

        public string GetCategoryString()
        {
            switch (_itemCategory)
            {
                case ItemCategory.Weapon:
                    return "武器";
                case ItemCategory.Armour:
                    return "护甲";
                case ItemCategory.Potion:
                    return "消耗品";
                case ItemCategory.SkillRoll:
                    return "技能";
                case ItemCategory.QuestItem:
                    return "任务物品";

                default:
                    return "无";
            }
        }
    }
}
