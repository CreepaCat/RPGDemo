
using MyNodeEditor.Extension.Dialogue;
using NewDialogueFrame;
using RPGDemo.Inventories;
using UnityEngine;

namespace RPGDemo.Quests
{
    public class QuestGiver : MonoBehaviour
    {
        [SerializeField] QuestSO questConfig;
        [SerializeField] DialogueTree dialogue;

        [Header("提交任务道具")]
        [SerializeField] InventoryItem questItem = null;
        [SerializeField] int amount = 1;


        AIQuestHandler aiQuestHandler = null;
        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            if (questConfig == null)
                Debug.LogError($"{gameObject.name}的任务没有配置,请先配置再使用");
            aiQuestHandler = GetComponentInParent<AIQuestHandler>();
            aiQuestHandler.AddQuest(questConfig);

            GetComponentInParent<CanversantTargetInteractable>()?.AddDialogue(dialogue);


        }

        public void GiveQuest()
        {
            aiQuestHandler.GiveQuest(questConfig);
        }


        public void GiveReward()
        {
            aiQuestHandler.GiveReward(questConfig);

        }

        public void GainQuestItemFromPlayer()
        {
            if (questItem == null) return;
            Inventory.GetPlayerInventory().RemoveItem(questItem, amount);
            SideMessageBox.Show(SideMessageBox.MessageType.Normal, $"提交任务道具{questItem.GetDisplayName()} x{amount}");
        }

    }
}
