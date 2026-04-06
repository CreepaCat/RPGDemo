using System;
using System.Collections.Generic;
using Core.AudioSystem;
using RPGDemo.Stats;
using RPGDemo.Weapons;
using Unity.VisualScripting;
using UnityEngine;

namespace RPGDemo.Combat
{
    public class DamageCollider : MonoBehaviour
    {
        // [SerializeField] private float damage;

        [SerializeField] LayerMask damageLayer;

        public Transform owner;

        HashSet<CombatTarget> _targets = new HashSet<CombatTarget>();

        Collider damageCollider;
        Weapon weapon;



        private void Awake()
        {
            damageCollider = GetComponent<Collider>();
            damageCollider.isTrigger = true;
            DisableCollider();

            // GetComponentInParent<Weapon>().AddDamageCollider(this);
        }

        private void OnDestroy()
        {
            // GetComponentInParent<Weapon>()?.RemoveDamageCollider(this);
        }

        public void Setup(Transform owner, LayerMask damageLayer, Weapon weapon)
        {

            this.owner = owner;
            this.damageLayer = damageLayer;
            this.weapon = weapon;

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
            if (((1 << other.gameObject.layer) & damageLayer.value) == 0)
            {
                return;
            }
            if (other.TryGetComponent(out CombatTarget target) && !_targets.Contains(target))
            {
                _targets.Add(target);

                target.TakeDamage(owner.GetComponent<BaseStats>().GetStats(StatsType.Attack));


                var sound = weapon.CurrentWeaponConfig.HitSound;
                if (sound == null) return;
                SoundManager.Instance.CreateSound()
                .WithSound(sound)
                .WithPlayPosition(target.transform.position)
                .WithRandomPitch()
                .Play();


            }
        }
    }
}
