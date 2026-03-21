using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPGDemo.Core.Strategies
{
    /// <summary>
    /// 目标过滤策略
    /// </summary>
    public interface IFilterStrategy
    {
        bool IsValidTarget(Character caster, Character target);
    }
}
