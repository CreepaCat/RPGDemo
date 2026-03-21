
using System.Collections.Generic;

namespace RPGDemo.Core.Strategies
{
    /// <summary>
    /// 目标选择策略
    /// </summary>
    public interface ITargetSelectionStrategy
    {
        List<Character> GetValidTargets(Character caster, IRangeStrategy range, IFilterStrategy filter);
    }
}
