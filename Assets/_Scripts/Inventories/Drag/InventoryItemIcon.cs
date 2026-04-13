using RPGDemo.Inventories.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPGDemo.Inventories
{
    public class InventoryItemIcon : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI itemAmountText = null;
        // [SerializeField] bool isActionItem = false;

        //CACHE
        private InventoryItem _item = null;

        public InventoryItem GetItem() => _item;

        private void Awake()
        {
            itemAmountText?.gameObject.SetActive(false);
        }

        public void SetItem(InventoryItem item)
        {
            _item = item;

            var iconImage = GetComponent<Image>();

            if (_item == null)
            {
                iconImage.enabled = false;
                itemAmountText?.gameObject.SetActive(false);
            }
            else
            {
                iconImage.enabled = true;
                iconImage.sprite = _item.GetIcon();
                iconImage.preserveAspect = true;

                var itemGetter = GetComponentInParent<IItemGetter>();


                if (itemGetter.GetAmount() > 1)
                {
                    if (itemAmountText != null)
                    {
                        itemAmountText.gameObject.SetActive(true);
                        itemAmountText.text = itemGetter.GetAmount().ToString();
                    }

                }
            }
        }
    }
}
