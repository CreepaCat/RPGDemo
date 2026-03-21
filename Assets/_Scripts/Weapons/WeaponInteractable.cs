using RPGDemo.InteractionSystem;
using UnityEngine;

namespace RPGDemo.Weapons
{
    public class WeaponInteractable : Interactable
    {

        [SerializeField] private WeaponConfig weaponConfig;
        
        public override void Interact(Interactor interactor)
        {
            base.Interact(interactor);

            Weapon weapon = interactor.GetComponentInParent<Weapon>();
           // weapon.UpdateWeaponConfig(weaponConfig);
            weapon.EquipWeapon(weaponConfig);
            
            // WeaponEquipptor equipptor = interactor.GetComponentInParent<WeaponEquipptor>();
            // equipptor.EquipWeapon();
            Destroy(gameObject);
            
            
            
        }

 
    }
}
