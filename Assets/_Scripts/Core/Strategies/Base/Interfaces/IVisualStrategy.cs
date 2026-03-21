
using System.Collections.Generic;
using UnityEngine;

namespace RPGDemo.Core.Strategies
{
    /// <summary>
    /// 特效策略
    /// </summary>
    public interface IVisualStrategy
    {
        void Play(Character caster, List<Character> targets, string skillId, Transform castPoint = null);
    }
}
