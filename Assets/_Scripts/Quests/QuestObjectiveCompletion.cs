using System;
using UnityEngine;
using RPGDemo.Attributes;
using NewDialogueFrame;

namespace RPGDemo.Quests
{


    /// <summary>
    /// 用来完成击杀，收集，交谈类任务目标进度计数的组件
    /// </summary>
    public class QuestObjectiveCompletion : MonoBehaviour
    {
        [SerializeField] QuestSO quest;
        [SerializeField] ObjectiveSO objective;
        [SerializeField] int completeNumber = 1;
        private void Start()
        {
            switch (objective.GetObjectiveType())
            {
                case ObjectiveType.Kill:
                    GetComponentInParent<Health>().OnDeath += CompleteObjective;
                    break;
                case ObjectiveType.Collect:
                    //Inventory.OnItemCollected += HandleItemCollected;
                    break;
                case ObjectiveType.Talk:
                    //NPCDialogue.OnNPCTalked += HandleNPCTalked;
                    //GetComponentInParent<CanversantTarget>().OnDialogueStart += CompleteObjective;
                    break;
            }
        }

        public void CompleteObjective()
        {
            var playerQuestHandler = PlayerQuestHandler.GetInstance();
            if (!playerQuestHandler.HasQuest(quest)) return;
            Debug.Log($"任务目标进度增加: {objective.Description} +{completeNumber}");

            playerQuestHandler.CompleteObjective(quest, objective, completeNumber);
        }
    }
}
