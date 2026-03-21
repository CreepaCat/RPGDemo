using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPGDemo.Core.Strategies
{
    public interface ICastRequirementStrategy
    {
        bool CanCast(Character caster);
        abstract void Consume(Character caster);
    }
}
