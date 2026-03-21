using System;
using UnityEngine;
using RPGDemo.Attributes;

namespace RPGDemo.Combat
{

    /// <summary>
    ///定义战斗对象的脚本,有些物品有血量但不是战斗对象：如可破坏的箱子，木栏等
    /// </summary>
    [RequireComponent(typeof(Health))]
    public class CombatTarget : MonoBehaviour
    {
        Health health;
        Animator animator;

        private void Awake()
        {
            health = GetComponent<Health>();
            animator = GetComponent<Animator>();
        }

        private void OnEnable()
        {
            health.OnDeath += OnDeath;
        }

        private void OnDeath()
        {
            //死亡处理
            animator?.SetBool("Death", true);
        }

        public void TakeDamage(float damageToTake)
        {
            health.TakeDamage(damageToTake);
        }
    }
}
