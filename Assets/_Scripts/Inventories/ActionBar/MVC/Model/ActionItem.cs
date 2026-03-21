using UnityEngine;

namespace RPGDemo.Inventories.ActionBar
{
    // [CreateAssetMenu(fileName = "New ActionItem", menuName = "RPGDemo/Inventory/ActionItem")]
    public abstract class ActionItem : InventoryItem
    {
        public float cooldown = 5f;
        [Tooltip("是否是消耗品")]
        [SerializeField] private bool _isConsumable = true;

        public bool IsConsumable() => _isConsumable;

        public abstract bool Use(GameObject user);
        // {
        //     Debug.Log($"{user.name}使用物品{name}");
        // }


    }
}
