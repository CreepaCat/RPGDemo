using RPGDemo.Attributes;
using UnityEngine;
using RPGDemo.Skills;

namespace RPGDemo.Core.Strategies
{
    /// <summary>
    /// 治疗效果策略
    /// </summary>
    //[CreateAssetMenu(menuName = "RPGDemo/Strategy/Effect/HealEffect")]
    [System.Serializable]

    public class HealEffectStrategy : EffectStrategy
    {
        public float baseHeal = 50;
        public override void Apply(Character caster, Character target, string skillId)
        {
            // 先判断是不是技能，如果是技能，使用技能的设置
            //否则应用固定设置
            // SkillItem skill = SkillItem.GetItemFromID(skillId) as SkillItem;
            // if (skill != null && skill.isHealingSkill)
            // {

            //     baseHeal = skill.baseHeal;
            // }


            target.GetComponent<Health>().Heal(baseHeal);

        }
    }
}
