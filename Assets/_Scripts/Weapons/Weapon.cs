using System;
using System.Collections.Generic;
using System.Linq;
using RPGDemo.Combat;
using UnityEngine;


namespace RPGDemo.Weapons
{
    /// <summary>
    /// 武器管理父类
    /// </summary>
    public class Weapon : MonoBehaviour
    {
        [SerializeField] protected WeaponConfig _currentWeaponConfig;
        [SerializeField] protected WeaponHolder _rightWeaponHolder;
        [SerializeField] protected WeaponHolder _leftWeaponHolder;

        public LayerMask targetLayer;
        protected Character character;
        public WeaponConfig CurrentWeaponConfig => _currentWeaponConfig;


        protected virtual void Awake()
        {
            character = GetComponent<Character>();
        }

        public virtual void HandleWeaponCombo()
        {
            if (_currentWeaponConfig == null)
                return;

            if (character.AnimationHandler.IsInteracting)
                return;
            character.AnimationHandler.Animator.SetBool("DoCombo", true);
            character.AnimationHandler.PlayTargetAnimation(Animator.StringToHash(_currentWeaponConfig.GetNextCombo(-1)), true, true, 0.2f);

        }

        internal void HandleRandomAttack()
        {

            if (_currentWeaponConfig == null)
                return;

            if (character.AnimationHandler.IsInteracting)
                return;

            var randomAnima = _currentWeaponConfig.GetRandomAttack();
            character.AnimationHandler.PlayTargetAnimation(Animator.StringToHash(randomAnima), true, false);


        }


        public void OpenDamageCollider(int handIndex)
        {
            List<DamageCollider> dcs = new();
            if (handIndex > 0)
            {
                dcs.AddRange(_rightWeaponHolder.GetDamageColliders().ToList());
            }
            else if (handIndex < 0)
            {
                dcs.AddRange(_leftWeaponHolder.GetDamageColliders().ToList());
            }
            else
            {
                dcs.AddRange(_rightWeaponHolder?.GetDamageColliders()?.ToList());
                dcs.AddRange(_leftWeaponHolder?.GetDamageColliders()?.ToList());
            }

            foreach (var collider in dcs)
            {
                if (collider == null) continue;
                collider.Setup(transform, targetLayer, this);
                collider.EnableCollider();
            }

        }

        public void CloseDamageColliders()
        {
            List<DamageCollider> dcs = new();
            dcs.AddRange(_rightWeaponHolder?.GetDamageColliders()?.ToList());
            dcs.AddRange(_leftWeaponHolder?.GetDamageColliders()?.ToList());


            foreach (var collider in dcs)
            {
                if (collider == null) continue;
                collider.DisableCollider();
            }
        }


        public void UpdateWeaponConfig(WeaponConfig newConfig)
        {
            _currentWeaponConfig = newConfig;
        }
        internal void SetRightWeaponHolder(WeaponHolder weaponHolder)
        {
            _rightWeaponHolder = weaponHolder;
        }
        internal void SetLeftWeaponHolder(WeaponHolder weaponHolder)
        {
            _leftWeaponHolder = weaponHolder;
        }


    }
}
