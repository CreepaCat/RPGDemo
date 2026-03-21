using System.Collections.Generic;
using UnityEngine;

namespace RPGDemo.Combat
{
    public class DamageCollider : MonoBehaviour
    {
        [SerializeField] private float damage;
        public Transform owner;

        HashSet<CombatTarget> _targets = new HashSet<CombatTarget>();

        Collider damageCollider;

        private void Awake()
        {
            damageCollider = GetComponent<Collider>();
            DisableCollider();
        }

        public void Setup(float damage,Transform owner)
        {
            this.damage = damage;
            this.owner = owner;
        }

        public void EnableCollider()
        {
            _targets.Clear();
            damageCollider.enabled = true;
        }

        public void DisableCollider()
        {
            damageCollider.enabled = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.transform == owner) return;
            if (other.TryGetComponent(out CombatTarget target) && !_targets.Contains(target))
            {
                _targets.Add(target);
                target.TakeDamage(damage);
            }
        }
    }
}
