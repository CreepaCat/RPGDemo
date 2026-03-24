using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace RPGDemo.Core.Strategies
{
    /// <summary>
    /// 技能施放要求策略
    /// </summary>
    public abstract class CastRequirementStrategy : ICastRequirementStrategy
    {
        public abstract bool CanCast(Character caster);
        public abstract void Consume(Character caster);

    }
}
