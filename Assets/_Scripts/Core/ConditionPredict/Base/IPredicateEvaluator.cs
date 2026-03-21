using System.Collections.Generic;

namespace RPGDemo.Core
{
    public interface IPredicateEvaluator
    {
        bool? Evaluate(Predicate predicate,IEnumerable<ConditionSO.Parameter> parameters);
    }
}