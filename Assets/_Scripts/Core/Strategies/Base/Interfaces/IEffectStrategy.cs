using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPGDemo.Core.Strategies
{
    /// <summary>
    /// 效果应用策略
    /// </summary>
    public interface IEffectStrategy
    {
        void Apply(Character caster, Character target, string skillId);
    }
}
