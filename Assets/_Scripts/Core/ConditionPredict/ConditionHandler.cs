using System.Collections.Generic;
using UnityEngine;

namespace RPGDemo.Core
{
    /// <summary>
    /// 条件检测器
    /// </summary>
    public class ConditionHandler : MonoBehaviour
    {
        public static event System.Action OnAnyConditionChanged;

        private IEnumerable<IPredicateEvaluator> evaluators = null;

        private void Awake()
        {
            evaluators = GetComponentsInChildren<IPredicateEvaluator>();
        }

        public static ConditionHandler GetInstance()
        {
            return GameObject.FindGameObjectWithTag("Player").GetComponent<ConditionHandler>();
        }

        public IEnumerable<IPredicateEvaluator> GetEvaluators() => evaluators;

        public void AnyConditionChanged()
        {
            OnAnyConditionChanged?.Invoke();
        }

        public bool Check(Predicate predicate, IEnumerable<ConditionSO.Parameter> parameters)
        {
            foreach (var evaluator in evaluators)
            {
                bool? result = evaluator.Evaluate(predicate, parameters);
                if (result.HasValue)
                {
                    return result.Value;

                }
            }
            return false;
        }
    }
}
