using System.Collections.Generic;
using UnityEngine;

namespace RPGDemo.Core
{

    public enum Predicate
    {
        None,
        HasItem,
        Kill,
        Collection,
        CanAcceptQuest,
        QuestFinished,
        QuestCompleted,
        ObjectiveCompleted,
        QuestInprogress,
    }

    // 使用复杂条件判断时，此SO只作为容器使用，因此其parameters失去意义，置空
    public enum ConditionLogicType
    {
        Single,     // 普通条件（使用 predicate）
        And,        // 所有子条件必须成立
        Or,         // 任意一个子条件成立
    }
    [CreateAssetMenu(fileName = "ConditionSO", menuName = "RPGDemo/Conditions/ConditionSO")]
    public class ConditionSO : ScriptableObject
    {
        [SerializeField] string description;
        [SerializeField] Predicate predicateType = Predicate.None;

        [SerializeField] List<Parameter> parameters = null;

        [Header("复合逻辑")]
        [SerializeField] ConditionLogicType logicType = ConditionLogicType.Single;
        [Header("取反")]
        [Tooltip("对AND取反不等价于原条件的OR，这符合德摩根定律AND + 取反 → 等价于 (!A) OR (!B)\nOR + 取反 → 等价于 (!A) AND (!B)\nSingle + 取反 → 等价于 !(原条件)")]
        [SerializeField] private bool isNegative = false; //是否取反值，取反后AND等价于OR

        [Header("子条件（And / Or  使用）")]
        [SerializeField] List<ConditionSO> subConditions = null;

        /// <summary>
        /// 单个参数（以ScriptableObject作为条件目标容器）
        /// </summary>
        [System.Serializable]
        public class Parameter
        {
            public ScriptableObject scriptableObject;
            public int number = 1;

            public string description;
        }
        public IEnumerable<Parameter> GetParameters() => parameters;
        public bool GetIsNegative() => isNegative;


        public Predicate GetPredicateType() => predicateType;


        public bool Check()
        {
            bool result = ComputeInnerResult();

            return isNegative ? !result : result;


        }

        /// <summary>
        /// 内部计算，不考虑取反
        /// </summary>
        /// <returns></returns>
        private bool ComputeInnerResult()
        {
            if (logicType == ConditionLogicType.Single)
            {
                if (GetPredicateType() == Predicate.None) return true;
                return ConditionHandler.GetInstance().Check(GetPredicateType(), parameters);
            }

            if (subConditions == null || subConditions.Count == 0)
                return true;

            switch (logicType)
            {
                case ConditionLogicType.And:
                    foreach (var sub in subConditions)
                        if (sub != null && !sub.Check())
                            return false;
                    return true;

                case ConditionLogicType.Or:
                    foreach (var sub in subConditions)
                        if (sub != null && sub.Check())
                            return true;
                    return false;

                default:
                    return true;
            }
        }
    }
}
