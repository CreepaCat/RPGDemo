using UnityEngine;

namespace RPGDemo.Core.Strategies
{
    [System.Serializable]
    public abstract class EffectStrategy : IEffectStrategy
    {
        public abstract void Apply(Character caster, Character target, string skillId);
    }
}
