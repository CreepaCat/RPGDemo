using System.Linq;
using RPGDemo.Core;
using RPGDemo.Inventories;
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

            UpdateCollectTypeQuestTrackingUI(objStatus);

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

        private void UpdateCollectTypeQuestTrackingUI(ObjectiveStatus objStatus)
        {

            if (objective.GetObjectiveType() == ObjectiveType.Collect)
            {
                //如果任务已提交，则将目标状态定格
                if (playerQuestHandler.GetQuestStatus(quest).IsFinished())
                {
                    objStatus.currentProgress = objective.GetRequiredAmount();
                    return;
                }
                //显示背包物品个数
                var item = objective.GetCondition().GetParameters().ToList()[0].scriptableObject as InventoryItem;
                if (item == null) return;

                int itemNum = Inventory.GetPlayerInventory().GetTotalAmount(item);
                if (objStatus.currentProgress != itemNum)
                {
                    objStatus.currentProgress = itemNum;
                    Mathf.Clamp(objStatus.currentProgress, 0, objective.GetRequiredAmount());
                    //发布任务进程改变事件,用来更新UI
                    playerQuestHandler.CompleteObjective(quest, objective, 0);
                }

            }
        }
    }
}
