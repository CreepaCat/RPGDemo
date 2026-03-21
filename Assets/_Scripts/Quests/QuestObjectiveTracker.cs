using System;
using System.Collections.Generic;
using RPGDemo.Core;
using UnityEngine;

namespace RPGDemo.Quests
{

    /// <summary>
    /// 跟踪任务目标的完成状态
    ///</summary>
    public class QuestObjectiveTracker : MonoBehaviour
    {
        [SerializeField] private QuestSO quest;
        [SerializeField] private ObjectiveSO objective;

        private bool isCompleted = false;
        private PlayerQuestHandler playerQuestHandler;


        public void Setup(QuestSO questSo, ObjectiveSO objectiveSo)
        {
            quest = questSo;
            objective = objectiveSo;
            gameObject.name = questSo.description + objectiveSo.Description;
            ConditionHandler.OnAnyConditionChanged += CheckAndUpdate;
            playerQuestHandler = PlayerQuestHandler.GetInstance();
            if (!playerQuestHandler.HasQuest(quest)) return;
            // 立即检查一次
            CheckAndUpdate();
        }
        private void OnDestroy()
        {
            ConditionHandler.OnAnyConditionChanged -= CheckAndUpdate;
        }
        private void CheckAndUpdate()
        {
            if (!playerQuestHandler.HasQuest(quest)) return;

            // bool nowCompleted = false;

            var objStatus = playerQuestHandler.GetQuestStatus(quest).GetObjectiveStatus(objective);
            bool nowCompleted = objStatus.IsCompleted();

            if (isCompleted && !nowCompleted)
            {
                // 可逆！（收集物品丢弃）
                playerQuestHandler.CancelObjective(quest, objective);
                isCompleted = false;
            }
            else if (!isCompleted && nowCompleted)
            {
                playerQuestHandler.CompleteObjective(quest, objective);
                isCompleted = true;
            }
        }
    }
}
