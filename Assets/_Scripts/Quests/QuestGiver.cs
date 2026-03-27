
using MyNodeEditor.Extension.Dialogue;
using NewDialogueFrame;
using UnityEngine;

namespace RPGDemo.Quests
{
    public class QuestGiver : MonoBehaviour
    {
        [SerializeField] QuestSO questConfig;
        [SerializeField] DialogueTree dialogue;

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

    }
}
