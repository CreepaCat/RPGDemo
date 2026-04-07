using System.Collections.Generic;
using System.Linq;
using RPGDemo.Weapons;
using UnityEngine;
using RPGDemo.Attributes;

#if UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace RPGDemo.Combat
{
    public class Fighter : MonoBehaviour
    {
        [SerializeField] float autoTargetRadius = 20f;
        [SerializeField] LayerMask targetLayer;
        [Required][SerializeField] WeaponConfig weaponConfig;

        //[SerializeField] bool isPlayer = false;

        Weapon weapon;

        CombatTarget currentTarget => GetCurrentTarget()?.GetComponent<CombatTarget>();


        public bool IsAttacking;

        public bool HasTarget() => currentTarget != null;


        RaycastHit[] targetBuffer = new RaycastHit[32];

        public object TargetingSystem { get; internal set; }

        private void Awake()
        {
            //将player Weapon和 enmey weapon统一
            weapon = GetComponent<Weapon>();
            weapon.targetLayer = targetLayer;
        }



        private void Start()
        {
            weapon.UpdateWeaponConfig(weaponConfig);
        }

        public void HandleRandomAttack()
        {
            weapon.HandleRandomAttack();
        }


        public void HandleComboAttack()
        {
            weapon.HandleWeaponCombo();
        }

        public void OnAttackStart(int handIndex)
        {


            IsAttacking = true;
            weapon.OpenDamageCollider(handIndex);
            weapon.PlaySlashVfx(handIndex);

        }

        public void OnAttackOver()
        {
            IsAttacking = false;
            weapon.CloseDamageColliders();
            //_player.Animator.SetBool("DoCombo", false);
        }

        public Character GetCurrentTarget()
        {
            //自动获取距离最近的敌人
            return GetAllTarget().FirstOrDefault();


        }

        // public void EnableWeaponDamage()
        // {
        //     weapon.OpenDamageCollider();
        // }

        public void DisableWeaponDamage()
        {
            weapon.CloseDamageColliders();
        }

        public List<Character> GetAllTarget()
        {

            List<Character> results = new();
            //targetBuffer = new RaycastHit[32];

            int count = Physics.SphereCastNonAlloc(transform.position, autoTargetRadius, Vector3.up, targetBuffer, 3f, targetLayer);
            for (int i = 0; i < count; i++)
            {
                RaycastHit other = targetBuffer[i];
                if (other.collider == null) continue;
                CombatTarget combatTarget = other.collider.GetComponent<CombatTarget>();
                if (combatTarget == null) continue;

                var target = combatTarget.GetComponent<Character>();
                results.Add(target);

            }

            //按距离排序
            results.Sort((a, b) =>
            {
                float disA = Vector3.Distance(transform.position, a.transform.position);
                float disB = Vector3.Distance(transform.position, b.transform.position);
                int dis = disA.CompareTo(disB);
                return dis;
            });

            Debug.Log($"获取到附近敌人个数{results.Count}");

            return results;
        }



        #region 动画事件

        public void Hit()
        {
            Debug.Log("Hit");
            //打开武器伤害范围检测，对所有符合条件的对象施加伤害
            //_weapon.GetComponent<Collider>().enabled = true;
        }

        #endregion
    }
}
