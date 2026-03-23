using RPGDemo.Core.Strategies;
using UnityEngine;

/// <summary>
/// 目标过滤策略
/// </summary>
[CreateAssetMenu(menuName = "RPGDemo/Strategy/Filter/EnemyFilter")]
public class EnemyFilterStrategy : FilterStrategy
{
    public bool isCaontainBoss = false;
    public override bool IsValidTarget(Character caster, Character target)
    {
        if (target.gameObject.CompareTag("Enemy"))
        {
            return true;
        }

        if (isCaontainBoss && target.gameObject.CompareTag("Boss"))
        {
            return true;
        }
        return false;
    }
}
