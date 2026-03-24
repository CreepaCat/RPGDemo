using RPGDemo.Attributes;
using RPGDemo.Skills;
using RPGDemo.Stats;
using UnityEngine;

namespace RPGDemo.Core.Strategies
{
    /// <summary>
    /// 伤害效果策略
    /// </summary>
    //[CreateAssetMenu(menuName = "RPGDemo/Strategy/Effect/DamageEffect")]
    public class DamageEffectStrategy : EffectStrategy
    {
        public float baseDamage; //固定伤害
                                 // [Range(0, 9999)] public float damageMultiplier; //百分比伤害,可用来定义技能倍率
        public override void Apply(Character caster, Character target, string skillId)
        {

            Debug.Log($"{caster}对{target}造成伤害{baseDamage}");
            target?.GetComponent<Health>()?.TakeDamage(baseDamage);

        }
    }
}
