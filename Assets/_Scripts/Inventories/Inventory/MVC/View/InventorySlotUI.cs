using RPGDemo.Core.DraggingFrame;
using UnityEngine;

namespace RPGDemo.Inventories
{
    public class InventorySlotUI : SlotUI, IDragContainer<InventoryItem>
    {
        [SerializeField] private InventoryItemIcon icon = null;

        //CACHE
        private Inventory _playerInventory;
        private int _index;

        public void Setup(Inventory inventory, int index)
        {
            _playerInventory = inventory;
            _index = index;
            icon.SetItem(_playerInventory.GetItemInSlot(index));
        }

        public override InventoryItem GetItem()
        {
            return _playerInventory.GetItemInSlot(_index);
        }

        public override int GetAmount()
        {
            return _playerInventory.GetItemAmountInSlot(_index);
        }

        public void RemoveItems(int amount)
        {

            _playerInventory.RemoveFromSlot(_index, amount);
        }


        public int GetMaxAcceptable(InventoryItem item)
        {
            return item.GetMaxStackableAmount() - _playerInventory.GetItemAmountInSlot(_index);
        }

        public void AddItems(InventoryItem item, int amount)
        {
            Debug.LogFormat("AddItems,index{0}, item:{1}, number:{2}", _index, item, amount);
            _playerInventory.AddToSlot(_index, item, amount);
        }
    }
}
