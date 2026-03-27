using System;
using RPGDemo.Combat;
using RPGDemo.Inventories;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RPGDemo.Weapons
{
    /// <summary>
    ///用于管理角色武器，如果Player和Enemy共用，可将 EquipWeapon，Sheath、Unsheath方法抽象为接口使用
    /// </summary>
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private WeaponConfig _currentWeaponConfig;
        [SerializeField] private WeaponHolder _weaponHolder;
        [SerializeField] private SheathHolder _sheathHolder;

        [SerializeField] private Key drawOrSheathKey = Key.Q;

        public WeaponConfig CurrentWeaponConfig => _currentWeaponConfig;

        //CACHE
        public string lastAttack;
        public int currentCombo = -1;

        //伤害目标检测
        public DamageCollider damageCollider = null;

        // public event Action OnEnterCombat;
        // public event Action OnExitCombat;


        public bool HasSheathedWeapon => _sheathHolder.transform.childCount > 0;
        public bool HasEquippedWeapon => _weaponHolder.transform.childCount > 0;

        private int clipIDLightAttack => Animator.StringToHash(_currentWeaponConfig.LightAttack_01);
        private int clipIDHeaveyAttack => Animator.StringToHash(_currentWeaponConfig.HeaveyAttack);

        private Player player;
        Equipment playerEquipment;

        private void Awake()
        {
            player = GetComponent<Player>();
            playerEquipment = player.GetComponent<Equipment>();
            _weaponHolder = GetComponentInChildren<WeaponHolder>();
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



        private void Update()
        {

            // if (!player.AnimatorHandler.IsCombat && HasEquippedWeapon)
            // {
            //     OnEnterCombat?.Invoke();
            //     player.Animator.SetBool(PlayerAnimatorParamConfig.animaIDIsCombat, HasEquippedWeapon);
            // }
            // else if (player.AnimatorHandler.IsCombat && !HasEquippedWeapon)
            // {
            //     OnExitCombat?.Invoke();
            //     player.Animator.SetBool(PlayerAnimatorParamConfig.animaIDIsCombat, HasEquippedWeapon);
            // }


        }

        //PUBLIC

        public void LightAttack()
        {
            if (_currentWeaponConfig == null)
                return;
            if (HasSheathedWeapon)
            {
                UnsheathSword();
                return;
            }
            Debug.Log("轻攻击");
            Player player = GetComponent<Player>();
            player.AnimatorHandler.PlayTargetAnimation(clipIDLightAttack, true);
            currentCombo = 0;
            player.AnimatorHandler.UpdateCanDoCombo(true);
        }

        public void HeaveAttack()
        {
            if (_currentWeaponConfig == null)
                return;
            if (HasSheathedWeapon)
            {
                UnsheathSword();
                return;
            }
            Debug.Log("重攻击");
            GetComponent<Player>().AnimatorHandler.PlayTargetAnimation(clipIDHeaveyAttack, true);
            lastAttack = _currentWeaponConfig.HeaveyAttack;
        }

        public void HandleWeaponCombo()
        {
            if (_currentWeaponConfig == null)
                return;
            if (HasSheathedWeapon)
            {
                UnsheathSword();
                return;
            }
            Debug.Log("HandleWeaponCombo");
            Player player = GetComponent<Player>();
            if (player.AnimatorHandler.CanDoCombo)
            {
                player.Animator.SetBool("DoCombo", true);

                string nextCombo = _currentWeaponConfig.GetNextCombo(currentCombo);
                //  player.AnimatorHandler.PlayTargetAnimation(Animator.StringToHash(nextCombo), true, 0.2f, true);
                player.AnimatorHandler.PlayTargetAnimation(Animator.StringToHash(_currentWeaponConfig.GetNextCombo(-1)), true, 0.2f, true);
                lastAttack = nextCombo;
                currentCombo++;
            }
        }

        public void OpenDamageCollider()
        {
            damageCollider.EnableCollider();
        }

        public void CloseDamageCollider()
        {
            damageCollider.DisableCollider();
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
            player.AnimatorHandler.PlayTargetHandMaskedAnimation(PlayerAnimatorParamConfig.clipIDUnsheath, true);
            //player.Animator.SetBool(PlayerAnimatorParamConfig.animaIDIsCombat, true);

        }

        public void SheathSword()
        {
            if (!HasEquippedWeapon)
                return;
            Debug.Log("SheathSword");
            player.AnimatorHandler.PlayTargetHandMaskedAnimation(PlayerAnimatorParamConfig.clipIDSheath, true);
        }




        #region 动画事件回调

        public void OnUnsheath()
        {
            DestroyHolderChildren();
            UpdateWeapon(_weaponHolder.transform);
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

            foreach (Transform child in _weaponHolder.transform)
            {
                Destroy(child.gameObject);
            }
        }

        private void UpdateWeaponConfig(WeaponConfig newConfig)
        {
            _currentWeaponConfig = newConfig;
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
            damageCollider = weaponGO?.GetComponent<DamageCollider>();
            float damage = _currentWeaponConfig == null ? 0f : _currentWeaponConfig.WeaponDamage;
            damageCollider?.Setup(damage, player.transform);
        }
    }
}
