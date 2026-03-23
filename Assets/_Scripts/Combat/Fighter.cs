using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPGDemo.Combat
{
    public class Fighter : MonoBehaviour
    {
        [SerializeField] float autoTargetRadius = 20f;
        [SerializeField] LayerMask targetLayer;
        private Player _player;

        CombatTarget currentTarget = null;

        RaycastHit[] targetBuffer = new RaycastHit[32];

        public object TargetingSystem { get; internal set; }

        private void Awake()
        {
            _player = GetComponent<Player>();
        }



        public void HandleLightAttack()
        {
            _player.Weapon.LightAttack();
        }

        public void HandleHeaveyAttack()
        {
            _player.Weapon.HeaveAttack();
        }


        public void OpenCombatTargetDetector()
        {
            _player.Weapon.OpenDamageCollider();
        }

        public void CloseCombatTargetDetector()
        {
            _player.Weapon.CloseDamageCollider();
        }

        public Character GetCurrentTarget()
        {
            //自动获取距离最近的敌人
            return GetAllTarget().FirstOrDefault();


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
