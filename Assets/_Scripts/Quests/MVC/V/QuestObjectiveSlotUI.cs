using TMPro;
using UnityEngine;

namespace RPGDemo.Quests
{
    public class QuestObjectiveSlotUI:MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI tmp_objectiveDescription;
        [SerializeField] TextMeshProUGUI tmp_objectiveState;

        public void Setup(ObjectiveStatus objectiveStatus)
        {
            tmp_objectiveDescription.text = objectiveStatus.GetObjective()?.Description;
            tmp_objectiveState.text = objectiveStatus.GetObjectiveStateText();

        }
    }
}