using System.Collections.Generic;
using RPGDemo.Combat;
using RPGDemo.Core.Strategies;
using UnityEngine;

/// <summary>
/// 多目标选择策略
/// </summary>
//[CreateAssetMenu(menuName = "RPGDemo/Strategy/Target/MultiTargetStrategy")]
[System.Serializable]
public class MultiTargetStrategy : TargetStrategy
{
    public int maxTargetNum = 3;
    public override List<Character> GetValidTargets(Character caster, IRangeStrategy range, IFilterStrategy filter)
    {
        //多目标选择
        List<Character> result = new();
        //使用角色身上的索敌器获取附近的敌人
        var targets = caster.GetComponent<Fighter>()?.GetAllTarget();
        if (targets == null) return result;

        for (int i = 0; i < targets.Count; i++)
        {
            if (i >= maxTargetNum)
            {
                break;
            }
            if (targets[i] != null && range.IsPositionInRange(caster.transform.position, targets[i].transform.position)
              && filter.IsValidTarget(caster, targets[i]))
            {
                result.Add(targets[i]);
            }
        }

        return result;
    }
}
