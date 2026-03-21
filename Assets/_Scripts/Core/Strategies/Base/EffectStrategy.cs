using UnityEngine;

namespace RPGDemo.Core.Strategies
{
    public abstract class EffectStrategy : ScriptableObject, IEffectStrategy
    {
        public abstract void Apply(Character caster, Character target, string skillId);
    }
}
