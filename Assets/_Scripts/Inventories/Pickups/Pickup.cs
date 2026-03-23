using System.Collections.Generic;
using UnityEngine;

namespace RPGDemo.Inventories.Pickups
{
    public class Pickup : MonoBehaviour
    {
        InventoryItem _item;
        int _amount;
        Inventory _inventory;

        private void Start()
        {
            _inventory = Inventory.GetPlayerInventory();
        }

        public void Setup(InventoryItem item, int amount = 1)
        {
            _item = item;
            _amount = amount;
            //如果有世界UI，刷新UI显示
            //如果有特效，启动特效
        }

        public InventoryItem GetItem() => _item;
        public int GetItemAmount() => _amount;

        public void PickupItem()
        {
            //对于一组多个的物品，逐个向背包添加，当背包满时停止

            if (CanBePickedUp())
            {
                Dictionary<InventoryItem, int> itemDict = new()
                {
                    { _item,_amount}
                };
                _inventory.AddItemDict(itemDict);

            }
            else
            {
                //todo:计算当前物品最多能捡多少个
                return;
            }

            int quantityRemained = 0;
            // for (int i = 0; i < GetItemAmount(); i++)
            // {
            //     bool success = _inventory.AddItemToFirstFoundSlot(_item, 1);
            //     if (!success)
            //     {
            //         quantityRemained = _amount - i;
            //         break;
            //     }
            //     Debug.Log("Picked up " + _item.GetDisplayName() + "x" + (_amount - quantityRemained));
            // }

            PickupSpawner spawner = GetComponentInParent<PickupSpawner>();
            spawner?.PickupCallback(_amount - quantityRemained);

            if (quantityRemained != 0)
            {
                _amount = quantityRemained;
                //todo:刷新UI显示
                return;
            }
            DestroyImmediate(gameObject);
        }

        public bool CanBePickedUp()
        {
            if (_inventory.HasSingleSlotSpaceFor(_item, _amount))
                return true;
            Dictionary<InventoryItem, int> itemDict = new()
            {
                { _item,_amount}
            };
            return _inventory.HasSpaceFor(itemDict);
        }
    }
}
