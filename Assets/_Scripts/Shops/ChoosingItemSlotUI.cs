using RPGDemo.Inventories;
using RPGDemo.Core.DraggingFrame;
using UnityEngine;

namespace RPGDemo.Shops
{

    public class SellItemSlotUI : SlotUI, IDragDestination<InventoryItem>
    {
        [SerializeField] InventoryItemIcon itemIcon = null;
        [SerializeField] bool isBuying;

        SellItemPanel sellItemPanel = null;

        private void Awake()
        {
            sellItemPanel = GetComponentInParent<SellItemPanel>();
        }

        public void SetItem(InventoryItem item)
        {
            itemIcon.SetItem(item);
        }

        public void AddItems(InventoryItem item, int amount)
        {
            itemIcon.SetItem(item);
            sellItemPanel.SetSellItemQuantity(item);

        }

        public override int GetAmount()
        {
            return itemIcon?.GetItem() != null ? 1 : 0;
        }

        public override InventoryItem GetItem()
        {
            return itemIcon?.GetItem();
        }

        public int GetMaxAcceptable(InventoryItem item)
        {
            if (isBuying)
            {
                return 0;
            }
            if (itemIcon.GetItem() != null
            && itemIcon.GetItem() == item)
            {
                return 0;
            }
            return 1; //此处接受1作为可放置icon的容器
        }

        public void RemoveItems(int amount)
        {
            itemIcon.SetItem(null);
            //sellItemPanel.SetSellItemQuantity(null);
        }
    }
}
