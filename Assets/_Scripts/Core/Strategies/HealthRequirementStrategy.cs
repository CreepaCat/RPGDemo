using RPGDemo.Attributes;
using UnityEngine;

namespace RPGDemo.Core.Strategies
{
    /// <summary>
    /// 血量百分比要求策略
    /// </summary>
    [CreateAssetMenu(menuName = "RPGDemo/Strategy/CastRequirement/HealthRequirement")]
    public class HealthRequirementStrategy : CastRequirementStrategy
    {
        [SerializeField,]
        [Range(0f, 100f)] public float healthPercentage;
        [SerializeField] public bool isLessThan = true;
        public override bool CanCast(Character caster)
        {
            var health = caster.GetComponent<Health>();
            if (health.IsDead()) return false;
            if (isLessThan && health.GetHealthRatio() * 100 < healthPercentage)
            {
                return true;
            }
            else if (!isLessThan && health.GetHealthRatio() >= healthPercentage)
            {
                return true;
            }
            string log = isLessThan ? "小于" : "大于或等于";
            Debug.Log($"{caster.name} 的血量百分比没达到要求:{log}{healthPercentage}%");
            return false;
        }

        public override void Consume(Character caster)
        {

        }
    }
}
