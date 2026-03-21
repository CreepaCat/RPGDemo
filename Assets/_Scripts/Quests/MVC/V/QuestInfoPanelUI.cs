using System;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

namespace RPGDemo.Quests
{
    [RequireComponent(typeof(CanvasGroup))]
    public class QuestInfoPanelUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI tmp_questName;
        [SerializeField] TextMeshProUGUI tmp_questLevel;
        [SerializeField] TextMeshProUGUI tmp_questDescription;
        [SerializeField] private Transform objectivesRoot;
        [SerializeField] QuestObjectiveSlotUI questObjectivePrefab;

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
            //Debug.Log("QuestInfoPanelUI UpdateUI");
            var currentSelectedQuest = GetComponentInParent<QuestPanelUI>().CurrentSelectedQuest;
            //gameObject.SetActive(false);
            canvasGroup.alpha = 0;

            if (currentSelectedQuest == null) return;
            canvasGroup.alpha = 1;

            tmp_questName.text = currentSelectedQuest.GetQuest().GetQuestName();
            tmp_questLevel.text = "Lv." + currentSelectedQuest.GetQuest().GetQuestLevel().ToString();
            tmp_questDescription.text = currentSelectedQuest.GetQuest().GetQuestDescription();

            foreach (Transform child in objectivesRoot)
            {
                Destroy(child.gameObject);
            }

            foreach (var objective in currentSelectedQuest.GetAllObjectiveStatus())
            {
                QuestObjectiveSlotUI objectiveSlot = Instantiate(questObjectivePrefab, objectivesRoot);
                objectiveSlot.Setup(objective);
            }
        }

    }
}
