using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace RPGDemo.Inventories.Pickups
{
    public class Pickup : MonoBehaviour
    {
        InventoryItem _item;
        int _amount;
        Inventory _inventory;

        [Header("悠悠球参数")]
        public float amplitude = 2f;        // 上下移动的幅度
        public float duration = 1.2f;       // 单程时间
        public Ease easeType = Ease.InOutSine;   // 缓动类型

        private Vector3 startPos;

        private void Start()
        {
            _inventory = Inventory.GetPlayerInventory();
            startPos = transform.position;
            PlayYoYo();
        }

        private void OnDestroy()
        {
            transform.DOKill();
        }

        public void PlayYoYo()
        {
            // 计算目标位置（在初始位置的正上方或下方）
            Vector3 targetPos = startPos + new Vector3(0, amplitude, 0);

            // 使用 DOMoveY 实现上下移动 + 无限循环
            transform.DOMoveY(targetPos.y, duration)
                .SetEase(easeType)                    // 平滑缓动
                .SetLoops(-1, LoopType.Yoyo);         // -1 = 无限循环，Yoyo = 往返
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

            PickupSpawner spawner = GetComponentInParent<PickupSpawner>();
            spawner?.PickupCallback(_amount - quantityRemained);

            SideMessageBox.ShowPickup(_item, _amount - quantityRemained);

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
