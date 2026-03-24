using System.Collections.Generic;
using UnityEngine;

namespace RPGDemo.Core.Strategies
{
    public abstract class TargetStrategy : ITargetSelectionStrategy
    {
        public abstract List<Character> GetValidTargets(Character caster, IRangeStrategy range, IFilterStrategy filter);
    }
}
