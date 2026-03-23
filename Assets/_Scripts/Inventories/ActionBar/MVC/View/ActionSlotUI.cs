using RPGDemo.Core.DraggingFrame;
using RPGDemo.Inventories.Utils;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace RPGDemo.Inventories.ActionBar
{
    public class ActionSlotUI : MonoBehaviour, IItemGetter, IDragContainer<InventoryItem>
    {
        //CONFIG
        [SerializeField] private int _index;
        [SerializeField] InventoryItemIcon _itemIcon;

        [SerializeField] Image coolDownFill; //冷却图片

        float cooldownTimer = float.MaxValue;

        //CACHE
        ActionStore _playerActionStore = null;

        private void Awake()
        {
            _playerActionStore = GameObject.FindGameObjectWithTag("Player").GetComponent<ActionStore>();
            coolDownFill.fillAmount = 0;
        }

        private void OnEnable()
        {
            _playerActionStore.OnActionStoreUpdated += Redraw;
        }

        private void OnDisable()
        {
            _playerActionStore.OnActionStoreUpdated -= Redraw;

        }

        private void Start()
        {
            Redraw();

        }

        private void Update()
        {
            if (GetItem() != null)
                coolDownFill.fillAmount = _playerActionStore.GetCooldownRatio((ActionItem)GetItem());
        }

        private void Redraw()
        {
            _itemIcon.SetItem(_playerActionStore.GetItemInSlot(_index));
        }

        public InventoryItem GetItem()
        {
            return (InventoryItem)_playerActionStore.GetItemInSlot(_index);
        }

        public int GetAmount()
        {
            return _playerActionStore.GetAmountInSlot(_index);
        }

        public void RemoveItems(int amount)
        {
            _playerActionStore.RemoveItems(_index, amount);
        }

        public int GetMaxAcceptable(InventoryItem item)
        {
            return _playerActionStore.GetMaxAcceptable(_index, item);
        }

        public void AddItems(InventoryItem item, int amount)
        {
            _playerActionStore.AddItems(_index, (ActionItem)item, amount);
        }
    }
}
