using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RPGDemo.Quests
{
    public class QuestSlotUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI tmp_questName;
        [SerializeField] TextMeshProUGUI tmp_questLevel;
        [SerializeField] Image background;
        [SerializeField] Image trackingStatus;
        [SerializeField] Image finishedStatus;
        [SerializeField] Color selectedColor;

        private int index = 0;
        private QuestStatus cachedQuestStatus;

        public void Setup(QuestStatus questStatus, int index, bool isSelected, bool isTracking)
        {
            tmp_questName.text = questStatus.GetQuest().GetQuestName();
            tmp_questLevel.text = "Lv." + questStatus.GetQuest().GetQuestLevel();
            this.index = index;
            this.cachedQuestStatus = questStatus;
            background.color = isSelected ? selectedColor : Color.white;


            finishedStatus.gameObject.SetActive(questStatus.IsFinished());
            trackingStatus.gameObject.SetActive(isTracking && !questStatus.IsFinished());
        }

        public void OnClickSlot()
        {
            //Debug.Log("OnClickSlot" + index);
            // var playerQuest = PlayerQuestHandler.GetPlayerQuestHandler();
            // foreach (var objectiveStatus in cachedQuestStatus.GetAllObjectiveStatus())
            // {
            //     Debug.Log($"Objective: {objectiveStatus.GetObjective().Description} " +
            //               $"==> {objectiveStatus.GetObjectiveStateText()}");
            // }
            GetComponentInParent<QuestPanelUI>().SetCurrentSelectedSlot(index);


        }

    }
}
