using UnityEngine;

namespace RPGDemo.Core.Strategies
{

    /// <summary>
    /// 魔力要求策略
    /// </summary>
    //[CreateAssetMenu(menuName = "RPGDemo/Strategy/CastRequirement/HealthRequirement")]

    public class ManaRequirementStrategy : CastRequirementStrategy
    {
        [SerializeField,]
        [Range(0f, 100f)] public float ManaPercentage;
        [SerializeField] public bool isLessThan = true;
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
