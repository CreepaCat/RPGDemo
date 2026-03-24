
using System.Collections.Generic;
using UnityEngine;

namespace RPGDemo.Core.Strategies
{
    [System.Serializable]
    public abstract class VisualStrategy : IVisualStrategy
    {
        public abstract void Play(Character caster, List<Character> targets, string skillId, Transform castPoint = null);

    }
}
