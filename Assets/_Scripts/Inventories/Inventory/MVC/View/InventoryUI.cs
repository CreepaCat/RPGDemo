
using System;
using System.Linq;
using TMPro;
using UnityEngine;


namespace RPGDemo.Inventories
{
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] InventorySlotUI inventorySlotUIPrefab = null;
        [SerializeField] InventoryDropTarget inventoryDropTarget = null;
        [SerializeField] private Transform slotsRoot = null;
        // [SerializeField] private Key inventoryKey = Key.I;
        //CACHE
        Inventory _playerInventory;
        ItemCategory currentItemCategory = ItemCategory.None;

        // Lifecircle
        private void Awake()
        {
            _playerInventory = Inventory.GetPlayerInventory();
            // _playerPurse = Purse.GetPlayerPurse();
        }

        private void OnEnable()
        {
            _playerInventory.OnInventoryUpdated += Redraw;
        }

        private void OnDisable()
        {
            _playerInventory.OnInventoryUpdated -= Redraw;
        }

        private void Start()
        {
            Redraw();
        }

        private void Update()
        {
            if (InventoryDragItem.IsDragging() && !inventoryDropTarget.gameObject.activeSelf)
            {
                inventoryDropTarget.gameObject.SetActive(true);
            }
            else if (!InventoryDragItem.IsDragging() && inventoryDropTarget.gameObject.activeSelf)
            {
                inventoryDropTarget.gameObject.SetActive(false);
            }
        }

        public void SetCurrentCategory(ItemCategory newCategory)
        {
            if (newCategory != currentItemCategory)
            {
                currentItemCategory = newCategory;
                Redraw();
            }
        }
        public void SetCurrentCategory(int categoryIndex)
        {
            ItemCategory category = (ItemCategory)categoryIndex;
            if (category != currentItemCategory)
            {
                currentItemCategory = category;
                Redraw();
            }
        }

        /// <summary>
        /// 整理背包
        /// </summary>
        public void OrgnizeInventory()
        {
            currentItemCategory = ItemCategory.None;
            _playerInventory.OrganizeInventory();

        }


        private void Redraw()
        {
            //todo:对象池优化

            foreach (Transform child in slotsRoot)
            {
                Destroy(child.gameObject);
            }

            if (currentItemCategory == ItemCategory.None)
            {
                for (int i = 0; i < _playerInventory.GetSize(); i++)
                {
                    InventorySlotUI slotUI = Instantiate(inventorySlotUIPrefab, slotsRoot);
                    slotUI.Setup(_playerInventory, i);
                }
            }
            else
            {
                var filterSlots = _playerInventory.GetFilterSlots(currentItemCategory).ToList();
                for (int i = 0; i < filterSlots.Count; i++)
                {
                    InventorySlotUI slotUI = Instantiate(inventorySlotUIPrefab, slotsRoot);
                    slotUI.Setup(_playerInventory, filterSlots[i].index);
                }
            }



            //  txt_money.text = Mathf.RoundToInt(_playerPurse.GetBalance()).ToString();
        }
    }
}
