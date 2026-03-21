using UnityEngine;

namespace RPGDemo.Items
{
    public abstract class EquipmentConfig : ScriptableObject
    {
        [Header("Equipment Information")]
        [SerializeField] private string itemName;
        [SerializeField] private Sprite itemIcon;
        
        public string ItemName => itemName;

        public Sprite ItemIcon => itemIcon;
        
        
    }
}
