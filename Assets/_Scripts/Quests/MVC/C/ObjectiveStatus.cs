using System.Linq;
using RPGDemo.Core;
using RPGDemo.Inventories;
using UnityEngine;

namespace RPGDemo.Quests
{
    [System.Serializable]
    public class ObjectiveStatus
    {
        public ObjectiveSO objective;

        public int currentProgress;  // 用于显示 3/5
        public bool everCompleted;   // 是否曾经达成（用于永久完成类目标）
        public ObjectiveStatus(ObjectiveSO objectiveSo)
        {
            if (objectiveSo == null)
            {
                Debug.LogError($"任务目标为空Objective is null");
            }
            objective = objectiveSo;
            Debug.LogFormat("New  ObjectiveStatus _objective:{0}  completedNumber:{1}", objectiveSo, this.currentProgress);

        }

        public ObjectiveSO GetObjective()
        {
            return objective;
        }



        /// <summary>
        /// 实时判断是否完成（支持收集物品丢弃后回滚）
        /// </summary>
        public bool IsCompleted()
        {
            if (objective == null) return false;


            bool isCompleted = false;
            if (objective.IsUseCondition())
            {
                isCompleted = objective.GetCondition()?.Check() ?? false;

            }
            else
            {
                //对于不使用Condition的任务目标，直接根据当前进度和要求的数量来判断是否完成
                isCompleted = currentProgress >= objective.GetRequiredAmount();
            }
            return isCompleted;

        }




        public string GetObjectiveStateText()
        {
            if (currentProgress >= objective.GetRequiredAmount())
            {
                return "已完成";
            }
            return $"{currentProgress}/{objective.GetRequiredAmount()}";


        }



    }
}
