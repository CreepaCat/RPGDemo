using System.Collections.Generic;
using RPGDemo.Combat;
using UnityEngine;

namespace RPGDemo.Weapons
{
    public class WeaponHolder : MonoBehaviour
    {
        [SerializeField] bool isLeftHand = false;
        private void Awake()
        {
            var weapon = GetComponentInParent<Weapon>();
            if (isLeftHand)
            {
                weapon?.SetLeftWeaponHolder(this);
            }
            else
            {
                weapon?.SetRightWeaponHolder(this);
            }

        }

        public IEnumerable<DamageCollider> GetDamageColliders()
        {
            return GetComponentsInChildren<DamageCollider>();
        }

    }
}
