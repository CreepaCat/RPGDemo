using System;
using System.Collections.Generic;
using System.Linq;
using RPGDemo.Combat;
using RPGDemo.Inventories;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RPGDemo.Weapons
{
    /// <summary>
    ///用于管理角色武器，如果Player和Enemy共用，可将 EquipWeapon，Sheath、Unsheath方法抽象为接口使用
    /// </summary>
    public class PlayerWeapon : Weapon
    {
        // [SerializeField] private WeaponConfig _currentWeaponConfig;
        // [SerializeField] private WeaponHolder _weaponHolder;
        [SerializeField] private SheathHolder _sheathHolder;

        [SerializeField] private Key drawOrSheathKey = Key.Q;


        public bool HasSheathedWeapon => _sheathHolder.transform.childCount > 0;
        public bool HasEquippedWeapon => _rightWeaponHolder.transform.childCount > 0;

        // private int clipIDLightAttack => Animator.StringToHash(_currentWeaponConfig.LightAttack_01);
        // private int clipIDHeaveyAttack => Animator.StringToHash(_currentWeaponConfig.HeaveyAttack);

        private Player player;
        Equipment playerEquipment;

        protected override void Awake()
        {
            base.Awake();
            player = character as Player;
            playerEquipment = player.GetComponent<Equipment>();
            _rightWeaponHolder = GetComponentInChildren<WeaponHolder>();
            _sheathHolder = GetComponentInChildren<SheathHolder>();
        }


        void OnEnable()
        {
            playerEquipment.OnWeaponUpdated += OnEquippedWeaponUpdated;
        }

        void OnDisable()
        {
            playerEquipment.OnWeaponUpdated -= OnEquippedWeaponUpdated;
        }

        void Start()
        {
            OnEquippedWeaponUpdated();
        }



        //PUBLIC

        #region 轻重攻击

        // public void LightAttack()
        // {
        //     if (_currentWeaponConfig == null)
        //         return;
        //     if (HasSheathedWeapon)
        //     {
        //         UnsheathSword();
        //         return;
        //     }
        //     Debug.Log("轻攻击");
        //     Player player = GetComponent<Player>();
        //     player.AnimationHandler.PlayTargetAnimation(clipIDLightAttack, true);
        //     //currentCombo = 0;
        //     player.AnimationHandler.UpdateCanDoCombo(true);
        // }

        // public void HeaveAttack()
        // {
        //     if (_currentWeaponConfig == null)
        //         return;
        //     if (HasSheathedWeapon)
        //     {
        //         UnsheathSword();
        //         return;
        //     }
        //     Debug.Log("重攻击");
        //     GetComponent<Player>().AnimationHandler.PlayTargetAnimation(clipIDHeaveyAttack, true);
        //     lastAttack = _currentWeaponConfig.HeaveyAttack;
        // }
        #endregion

        public override void HandleWeaponCombo()
        {
            if (_currentWeaponConfig == null)
                return;
            if (HasSheathedWeapon)
            {
                UnsheathSword();
                return;
            }

            Player player = GetComponent<Player>();
            if (player.AnimationHandler.CanDoCombo)
            {
                player.Animator.SetBool("DoCombo", true);
                player.AnimationHandler.PlayTargetAnimation(Animator.StringToHash(_currentWeaponConfig.GetNextCombo(-1)), true, true, 0.2f);

            }
        }


        public void EquipWeapon(WeaponConfig newConfig)
        {
            UpdateWeaponConfig(newConfig);
            //  if(newConfig == null) return;
            SpawnWeaponInSheath(_sheathHolder.transform);
        }

        public void UnsheathSword()
        {

            if (!HasSheathedWeapon)
                return;
            Debug.Log("UnsheathSword");
            player.AnimationHandler.PlayTargetHandMaskedAnimation(PlayerAnimatorParamConfig.clipIDUnsheath, true);
            //player.Animator.SetBool(PlayerAnimatorParamConfig.animaIDIsCombat, true);

        }

        public void SheathSword()
        {
            if (!HasEquippedWeapon)
                return;
            Debug.Log("SheathSword");
            player.AnimationHandler.PlayTargetHandMaskedAnimation(PlayerAnimatorParamConfig.clipIDSheath, true);
        }




        #region 动画事件回调

        public void OnUnsheath()
        {
            DestroyHolderChildren();
            UpdateWeapon(_rightWeaponHolder.transform);
        }

        public void OnSheath()
        {
            DestroyHolderChildren();
            UpdateWeapon(_sheathHolder.transform);
        }



        #endregion

        //PRIVATE

        //从装备栏装备武器时事件回调
        private void OnEquippedWeaponUpdated()
        {
            if (playerEquipment.HasEquippedWeapon())
            {
                WeaponItem weaponItem = playerEquipment.GetEquipableItemInSlot(EquipLocation.Weapon) as WeaponItem;
                EquipWeapon(weaponItem.GetWeaponConfig());
                UnsheathSword();
            }
            else
            {
                EquipWeapon(null);
            }
        }

        private void DestroyHolderChildren()
        {
            foreach (Transform child in _sheathHolder.transform)
            {
                Destroy(child.gameObject);
            }

            foreach (Transform child in _rightWeaponHolder.transform)
            {
                Destroy(child.gameObject);
            }
        }


        private void SpawnWeaponInSheath(Transform sheathHolder)
        {
            DestroyHolderChildren();
            UpdateWeapon(sheathHolder);
        }

        /// <summary>
        /// 每次生成武器，都需要更新伤害碰撞检测器
        /// </summary>
        /// <param name="sheathHolder"></param>
        private void UpdateWeapon(Transform sheathHolder)
        {
            GameObject weaponGO = _currentWeaponConfig?.SpawnWeapon(sheathHolder);
            // if (weaponGO == null) return;
            // List<DamageCollider> damageColliders = weaponGO?.GetComponents<DamageCollider>().ToList();

            // foreach (var collider in damageColliders)
            // {
            //     collider?.Setup(player.transform, targetLayer, this);
            // }

        }
    }
}
