using System.Collections.Generic;
using UnityEngine;
using RPGDemo.Inventories.Utils;

namespace RPGDemo.Inventories
{
    public class SelectorsRoot : MonoBehaviour
    {
        
        SlotItemSelector _currentSelected = null;
        
        
        public SlotItemSelector GetCurrentSelected() => _currentSelected;
        
        public void UpdateSelectors(SlotItemSelector newSelector,SlotItemSelector[] selectors)
        {
            if(ReferenceEquals(_currentSelected, newSelector)) return;
           // Debug.Log("UpdateSelectors,Count"+selectors.Length );
            
            //新选择
            foreach (var selector in selectors)
            {
                if (ReferenceEquals(selector, newSelector))
                {
                    if (_currentSelected != null)
                    {
                        _currentSelected.Deselected();
                    }
                    if (newSelector.GetSelectedItem() == null)
                    {
                        _currentSelected = null;
                        return;
                    }
                    _currentSelected = newSelector;
                    _currentSelected.Selected();
                    
                    break;
                    
                }
            }
        }
        

    }
}