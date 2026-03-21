
using System.Collections.Generic;
using UnityEngine;

namespace RPGDemo.Core.Strategies
{
    public abstract class VisualStrategy : ScriptableObject, IVisualStrategy
    {
        public abstract void Play(Character caster, List<Character> targets, string skillId, Transform castPoint = null);

    }
}
