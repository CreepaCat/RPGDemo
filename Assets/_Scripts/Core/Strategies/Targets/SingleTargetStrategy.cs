using UnityEngine;
using System.Collections.Generic;
using RPGDemo.Combat;

namespace RPGDemo.Core.Strategies
{
    /// <summary>
    /// 单目标选择策略
    /// </summary>
    [CreateAssetMenu(menuName = "RPGDemo/Strategy/Target/SingleTarget")]
    public class SingleTargetStrategy : TargetStrategy
    {
        public bool isSelfTarget = false; //是否以自己为目标
        public override List<Character> GetValidTargets(Character caster, IRangeStrategy range, IFilterStrategy filter)
        {
            if (isSelfTarget) return new List<Character>() { caster };
            // 示例：鼠标指向或自动锁定最近敌人（可改成 Raycast）
            var target = caster.GetComponent<Fighter>().GetCurrentTarget();


            if (target == null)
            {
                return new List<Character>();
            }

            if (range != null && !range.IsPositionInRange(caster.transform.position, target.transform.position))
            {

                return new List<Character>();
            }

            if (filter != null && !filter.IsValidTarget(caster, target))
            {
                return new List<Character>();
            }
            return new List<Character>() { target };
        }
    }
}
