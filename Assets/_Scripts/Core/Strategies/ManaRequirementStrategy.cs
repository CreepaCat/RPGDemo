using UnityEngine;

namespace RPGDemo.Core.Strategies
{

    /// <summary>
    /// 魔力要求策略
    /// </summary>
    [CreateAssetMenu(menuName = "RPGDemo/Strategy/CastRequirement/HealthRequirement")]

    public class ManaRequirementStrategy : CastRequirementStrategy
    {
        public override bool CanCast(Character caster)
        {
            throw new System.NotImplementedException();
        }

        public override void Consume(Character caster)
        {
            throw new System.NotImplementedException();
        }
    }
}
