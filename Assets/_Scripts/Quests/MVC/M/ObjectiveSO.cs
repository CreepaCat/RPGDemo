using System;
using System.Collections.Generic;
using System.Linq;
using RPGDemo.Core;
using RPGDemo.Inventories;
using UnityEngine;

namespace RPGDemo.Quests
{
    public enum ObjectiveType { Boolean, Collect, Kill, Talk }
    [CreateAssetMenu(fileName = "New QuestObjective", menuName = "RPGDemo/Quest/New QuestObjective", order = 2)]
    public class ObjectiveSO : ScriptableObject
    {
        [field: SerializeField] public string Description { get; private set; }
        [SerializeField] private int _completedRequireAmount = 1;


        [Header("目标类型")]
        [SerializeField] ObjectiveType objectiveType = ObjectiveType.Boolean;
        [SerializeField] private bool useCandition = true;
        [SerializeField] ConditionSO condition;

        private static Dictionary<string, ObjectiveSO> _lookup;

        public static ObjectiveSO GetObjectiveByDescription(string description)
        {
            if (_lookup == null)
            {
                BuildLookup();
            }
            if (string.IsNullOrEmpty(description))
            {
                return null;
            }

            return _lookup[description];
        }

        private static void BuildLookup()
        {
            _lookup = new Dictionary<string, ObjectiveSO>();
            var allObjectives = Resources.LoadAll<ObjectiveSO>(""); // 加载所有ObjectiveSO资源
            foreach (var obj in allObjectives)
            {
                if (!_lookup.ContainsKey(obj.Description))
                {
                    _lookup[obj.Description] = obj;
                }
                else
                {

                }
            }
        }

        public ConditionSO GetCondition() => condition;
        public bool IsUseCondition() => useCandition;
        public bool HasCondition() => condition != null;

        public int GetRequiredAmount() => _completedRequireAmount;
        public ObjectiveType GetObjectiveType() => objectiveType;
    }

}
