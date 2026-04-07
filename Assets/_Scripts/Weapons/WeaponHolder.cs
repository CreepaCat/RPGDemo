using System.Collections.Generic;
using System.Linq;
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

        public List<DamageCollider> GetDamageColliders()
        {

            if (GetComponentsInChildren<DamageCollider>()?.ToList() is { } colliders)
            {
                return colliders;
            }
            return new List<DamageCollider>();
        }

    }
}
