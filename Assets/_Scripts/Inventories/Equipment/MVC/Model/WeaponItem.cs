using RPGDemo.Weapons;
using UnityEngine;

namespace RPGDemo.Inventories
{
    [CreateAssetMenu(menuName = ("RPGDemo/Inventory/EquipableItem/Weapon"), fileName = ("New Weapon"))]
    public class WeaponItem : EquipableItem
    {
        [SerializeField] private WeaponConfig _weaponConfig = null;
        // [SerializeField] private
        public WeaponConfig GetWeaponConfig() => _weaponConfig;

    }
}
