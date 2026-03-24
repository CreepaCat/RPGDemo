using UnityEngine;

namespace RPGDemo.Core.Strategies
{
    [System.Serializable]
    public abstract class FilterStrategy : IFilterStrategy
    {
        public abstract bool IsValidTarget(Character caster, Character target);
    }
}
