using RPGDemo.Attributes;
using RPGDemo.Core.Strategies;
using RPGDemo.Stats;
using UnityEngine;
/// <summary>
/// 基于攻击倍率的伤害效果
/// </summary>
[System.Serializable]
public class AttackMultilplierDamageEffectStrategy : EffectStrategy
{
    [Range(0f, 999f)] public float attackDamageMultiplier;
    public override void Apply(Character caster, Character target, string skillId)
    {
        float attack = caster.GetComponent<BaseStats>().GetStats(StatsType.Attack);

        float finalDamage = attack * (1 + attackDamageMultiplier * 0.01f);
        Debug.Log($"{caster}对{target}造成基于攻击倍率的伤害{finalDamage}");
        target.GetComponent<Health>()?.TakeDamage(finalDamage);
    }
}
