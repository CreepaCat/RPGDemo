using System;
using RPGDemo.Core.DraggingFrame;
using RPGDemo.Inventories.Utils;
using TMPro;
using UnityEngine;

namespace RPGDemo.Inventories
{
    /// <summary>
    ///MVC  View
    /// </summary>
    public class EquipmentSlotUI : MonoBehaviour, IItemGetter, IDragContainer<InventoryItem>
    {
        //config
        [SerializeField] EquipLocation _allowedEquipLocation;
        [SerializeField] TextMeshProUGUI tmp_slotName;
        [SerializeField] InventoryItemIcon _itemIcon = null;

        //Cache
        Equipment _playerEquipment;

        private void Awake()
        {
            _playerEquipment = Player.GetInstance().GetComponent<Equipment>();
            _playerEquipment.SetSlot(_allowedEquipLocation);
            SetSlotName();
        }



        private void OnEnable()
        {
            _playerEquipment.OnEquipmentUpdated += RedrawUI;
        }

        private void OnDisable()
        {
            _playerEquipment.OnEquipmentUpdated -= RedrawUI;
        }

        private void Start()
        {
            RedrawUI();
        }

        void RedrawUI()
        {
            if (GetItem() == null)
            {
                _itemIcon.gameObject.SetActive(false);
                return;
            }

            _itemIcon.gameObject.SetActive(true);
            InventoryItem equippedItem = (InventoryItem)_playerEquipment.GetEquipableItemInSlot(_allowedEquipLocation);
            _itemIcon.SetItem(equippedItem);
            Debug.Log($"装备槽{_allowedEquipLocation}有物品{equippedItem.GetDisplayName()}");
        }

        public InventoryItem GetItem()
        {
            return _playerEquipment.GetEquipableItemInSlot(_allowedEquipLocation);
        }

        public int GetAmount()
        {
            if (GetItem() == null)
                return 0;
            return 1;
        }

        public void RemoveItems(int amount)
        {
            _playerEquipment.RemoveItem(_allowedEquipLocation);
        }


        public int GetMaxAcceptable(InventoryItem item)
        {
            EquipableItem equipableItem = item as EquipableItem;
            if (equipableItem == null)
            {
                print("非可装备物品");
                return 0;
            }
            if (equipableItem.GetAllowedEquipLocation() != _allowedEquipLocation)
            {
                print("非可装备位置");
                return 0;
            }

            if (GetItem() != null)
            {
                print("装备槽不为空");
                return 0;
            }
            return 1;
        }

        public void AddItems(InventoryItem item, int amount)
        {
            _playerEquipment.AddItem(_allowedEquipLocation, item as EquipableItem);
        }
        private void SetSlotName()
        {
            string slotName = "";
            switch (_allowedEquipLocation)
            {
                case EquipLocation.Gloves:
                    slotName = "手套";
                    break;
                case EquipLocation.Body:
                    slotName = "胸甲";
                    break;
                case EquipLocation.Trousers:
                    slotName = "腿甲";
                    break;
                case EquipLocation.Boots:
                    slotName = "鞋子";
                    break;
                case EquipLocation.Weapon:
                    slotName = "武器";
                    break;
                case EquipLocation.Helmet:
                    slotName = "头盔";
                    break;
            }

            tmp_slotName.text = slotName;
        }
    }
}
