using UnityEngine;

namespace RPGDemo.Combat
{
    public class Fighter : MonoBehaviour
    {
        private Player _player;

        CombatTarget currentTarget = null;

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
            //获取当前的锁定的单个战斗对象

            return currentTarget.GetComponent<Character>();


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
