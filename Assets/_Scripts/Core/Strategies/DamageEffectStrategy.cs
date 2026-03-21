using RPGDemo.Attributes;
using RPGDemo.Skills;
using RPGDemo.Stats;
using UnityEngine;

namespace RPGDemo.Core.Strategies
{
    /// <summary>
    /// 伤害效果策略
    /// </summary>
    [CreateAssetMenu(menuName = "RPGDemo/Strategy/Effect/DamageEffect")]
    public class DamageEffectStrategy : EffectStrategy
    {
        public float damageToAplus; //固定伤害
        [Range(0, 9999)] public float damageMultiplier; //百分比伤害,可用来定义技能倍率
        public override void Apply(Character caster, Character target, string skillId)
        {
            //先判断是不是技能，如果是技能，使用技能的设置
            //否则应用固定设置
            SkillItem skill = SkillItem.GetItemFromID(skillId) as SkillItem;
            if (skill != null)
            {
                damageToAplus = 0;
                damageMultiplier = skill.damageMultiplier;
            }

            var baseAtk = caster.GetComponent<BaseStats>().GetStats(StatsType.Attack);

            var finalDamage = (baseAtk) * (1 + damageMultiplier * 0.01f) + damageToAplus;
            // - target.GetComponent<BaseStats>().GetStats(StatsType.Defense);

            Debug.Log($"{caster}对{target}造成最终伤害{finalDamage}");
            target.GetComponent<Health>().TakeDamage(finalDamage);

        }
    }
}
