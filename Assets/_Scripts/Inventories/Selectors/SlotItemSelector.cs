using System;
using RPGDemo.Inventories.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RPGDemo.Inventories
{
    public class SlotItemSelector : MonoBehaviour,IPointerDownHandler
    {
        //CACHE
        private SelectorsRoot _selectorsRoot = null;
        
        
        public event Action OnSelected;
        public event Action OnDeselect;
        private void Awake()
        {
            _selectorsRoot = GetComponentInParent<SelectorsRoot>();
     
        }
        
        public void Selected()
        {
            Debug.Log("OnSelected");
            OnSelected?.Invoke();
            
        }

        public void Deselected()
        {
            Debug.Log("Deselected");
            OnDeselect?.Invoke();
        }

        public InventoryItem GetSelectedItem()
        {
            return GetComponentInParent<IItemGetter>().GetItem();
        }
        

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            //实时获取，避免缓存无效BUG
            var newSelectorsList = _selectorsRoot.GetComponentsInChildren<SlotItemSelector>();
            //Debug.Log("选中"+transform.name);
            _selectorsRoot.UpdateSelectors(this,newSelectorsList);
        }
        
      
    }
}
