using System;
using UnityEngine;

namespace RPGDemo.Quests
{
    [RequireComponent(typeof(CanvasGroup))]
    public class RewardPanelUI:MonoBehaviour
    {
        [SerializeField] Transform contentRoot;
        [SerializeField] RewardSlotUI rewardSlotPrefab;
        
        CanvasGroup canvasGroup;
        QuestPanelUI questPanelUI;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
            questPanelUI = GetComponentInParent<QuestPanelUI>();
           
        }

        private void OnEnable()
        {
            questPanelUI.OnRefreshPanel += UpdateUI;
        }

        private void OnDisable()
        {
            questPanelUI.OnRefreshPanel -= UpdateUI;
        }


        public void UpdateUI()
        {
            foreach (Transform child in contentRoot)
            {
                Destroy(child.gameObject);
            }

            var quest  = GetComponentInParent<QuestPanelUI>().CurrentSelectedQuest?.GetQuest();
            canvasGroup.alpha = 0;
            if(quest == null) return;
            canvasGroup.alpha = 1;
       
            foreach (var itemData in quest.GetQuestRewardItems())
            {
                RewardSlotUI rewardSlot = Instantiate(rewardSlotPrefab, contentRoot);
                rewardSlot.Setup(itemData);
            }
            
            if (quest.GetQuestRewardCoins() > 0)
            {
                RewardSlotUI rewardSlot = Instantiate(rewardSlotPrefab, contentRoot);
                rewardSlot.SetupGold(quest.GetQuestRewardCoins());
            }
            
            if (quest.GetQuestRewardExp() > 0)
            {
                RewardSlotUI rewardSlot = Instantiate(rewardSlotPrefab, contentRoot);
                rewardSlot.SetupExp(quest.GetQuestRewardExp());
            }
        }
    }
}