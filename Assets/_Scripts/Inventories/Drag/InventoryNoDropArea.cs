using RPGDemo.Core.DraggingFrame;
using UnityEngine;

namespace RPGDemo.Inventories
{
    public class InventoryNoDropArea : MonoBehaviour, IDragDestination<InventoryItem>
    {
        public int GetMaxAcceptable(InventoryItem item)
        {
            return 0;
        }

        public void AddItems(InventoryItem item, int amount)
        {
            Debug.Log("在非丢弃区域，不执行丢弃");
        }
    }
}
