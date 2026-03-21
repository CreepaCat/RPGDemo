using UnityEngine;

namespace RPGDemo.Core.Strategies
{
    public abstract class FilterStrategy : ScriptableObject, IFilterStrategy
    {
        public abstract bool IsValidTarget(Character caster, Character target);
    }
}
