using System.Collections.Generic;
using UnityEngine;
using RPGDemo.Inventories;

namespace RPGDemo.Quests
{

    public enum AIQuestListStatus
    {
        NoQuest, //没有任务
        CanGiveQuest, //可以发布任务
        CanCompleteQuest, //可以完成任务
        InProgress //任务进行中
    }

    /// <summary>
    /// 定义NPC身上的任务列表
    /// 根据自己发布给玩家的任务状态,来显示头顶的提示符号
    /// </summary>
    public class AIQuestHandler : MonoBehaviour
    {
        [SerializeField] private List<QuestSO> quests;

        PlayerQuestHandler playerQuestHandler;
        private void Awake()
        {
            playerQuestHandler = PlayerQuestHandler.GetInstance();
        }

        public AIQuestListStatus GetAIQuestListStatus()
        {
            var playerQuestList = PlayerQuestHandler.GetInstance();

            //按信息重要程度提前返回
            foreach (var quest in quests)
            {
                var questStatus = playerQuestList.GetQuestStatus(quest);
                if (questStatus == null)
                {
                    Debug.Log("有没有接取的任务" + quest.GetQuestName());
                    // hasCanGiveQuest = playerQuestHandler.CanAcceptQuest(quest);
                    if (playerQuestHandler.CanAcceptQuest(quest))
                        return AIQuestListStatus.CanGiveQuest;
                }
                if (questStatus != null && !questStatus.IsFinished())
                {
                    if (questStatus.IsCompleted())
                    {
                        return AIQuestListStatus.CanCompleteQuest;
                    }
                    else
                    {
                        return AIQuestListStatus.InProgress;
                    }
                }

            }

            return AIQuestListStatus.NoQuest;
        }
        public void AddQuest(QuestSO questToAdd)
        {
            if (questToAdd == null || quests.Contains(questToAdd)) return;
            quests.Add(questToAdd);
        }

        public void GiveQuest(QuestSO questToGive)
        {
            if (playerQuestHandler.HasQuest(questToGive))
            {
                Debug.Log($"玩家已接取任务{questToGive.GetQuestName()},无法重复接取");
                return;
            }
            playerQuestHandler.AcceptQuest(questToGive);
        }

        public void GiveReward(QuestSO questToReward)
        {
            if (!playerQuestHandler.HasQuest(questToReward))
            {
                Debug.Log($"玩家没有任务{questToReward.GetQuestName()},无法获得任务奖励");

                return;
            }
            if (playerQuestHandler.GetQuestStatus(questToReward).IsFinished())
            {
                Debug.Log($"任务{questToReward.GetQuestName()}已完结,无法重复获得任务奖励");

                return;
            }
            var rewardItems = questToReward.GetQuestRewardItems();
            if (rewardItems != null && rewardItems.Count > 0)
            {
                if (!TryRewardItemsToPlayer(rewardItems))
                {
                    BottomMessageBox.ShowCustom("无法获得任务奖励");
                    return;
                }
            }

            //奖励金币
            if (questToReward.GetQuestRewardCoins() > 0)
            {
                Purse.GetPlayerPurse().AddMoney(questToReward.GetQuestRewardCoins());
                SideMessageBox.ShowReward("金币", questToReward.GetQuestRewardCoins());
            }


            //奖励经验
            if (questToReward.GetQuestRewardExp() > 0)
            {
                Player.GetInstance().Experience.GainExp(questToReward.GetQuestRewardExp());
                SideMessageBox.ShowReward("经验", questToReward.GetQuestRewardExp());
            }



            //修改任务状态
            playerQuestHandler.FinishQuest(questToReward);

        }

        private bool TryRewardItemsToPlayer(List<QuestSO.RewardItem> rewardItems)
        {
            Dictionary<InventoryItem, int> rewardItemDict = new();
            var playerInventory = Inventory.GetPlayerInventory();
            foreach (var rewardItemData in rewardItems)
            {
                if (rewardItemData == null) continue;
                if (rewardItemDict.ContainsKey(rewardItemData.item))
                {
                    rewardItemDict[rewardItemData.item] += rewardItemData.quantity;
                }
                else
                {
                    rewardItemDict[rewardItemData.item] = rewardItemData.quantity;
                }
            }

            if (!playerInventory.AddItemDict(rewardItemDict))
            {
                Debug.Log("玩家背包没有空位，无法获得任务奖励，");
                BottomMessageBox.ShowInventorySpaceNotEnough();
                return false;
            }

            foreach (var rewardData in rewardItems)
            {
                SideMessageBox.ShowReward(rewardData.item.GetDisplayName(), rewardData.quantity);
            }

            return true;
        }
    }
}
