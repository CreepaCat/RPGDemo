using System;
using RPGDemo.Inventories.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RPGDemo.Inventories
{
    public class SlotHighlighter:MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
    {
        [SerializeField] Image Image_hoverBackground = null;
        [SerializeField] Image Image_selectedFrame =  null;
        
        //CACHE
        SlotItemSelector _slotItemSelector = null;

        private void Awake()
        {
            _slotItemSelector = GetComponentInChildren<SlotItemSelector>();
            HideSelectedUI();
        }

        private void OnEnable()
        {
            _slotItemSelector.OnSelected += ShowSelectedUI;
            _slotItemSelector.OnDeselect += HideSelectedUI;
        }

        private void ShowSelectedUI()
        {
            Image_selectedFrame.gameObject.SetActive(true);
            Image_hoverBackground.gameObject.SetActive(true);
        }

        private void HideSelectedUI()
        {
            Image_selectedFrame.gameObject.SetActive(false);
            Image_hoverBackground.gameObject.SetActive(false);
        }

        private void ShowHoveringUI()
        {
            Image_hoverBackground.gameObject.SetActive(true);
        }

        private void HideHoveringUI()
        {
            Image_hoverBackground.gameObject.SetActive(false);
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            if (GetComponentInParent<IItemGetter>().GetItem() == null)
            {
                return;
            }
            ShowHoveringUI();
         
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            var currentSelected = GetComponentInParent<SelectorsRoot>().GetCurrentSelected();
            if (currentSelected != null
                && ReferenceEquals(currentSelected, _slotItemSelector))
            {
                return;
            }
            
            HideHoveringUI();
        }
    }
}