using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using RPGDemo.Inventories;
using UnityEngine;

namespace RPGDemo.Quests
{
    [System.Serializable]
    public class QuestStatus
    {
        public QuestSO quest;
        public QuestProgress progress { get; private set; } = QuestProgress.Active;

        [SerializeField] private List<ObjectiveStatus> objectiveStatuses = new();

        public QuestStatus(QuestSO q)
        {
            if (q == null) return;
            quest = q;
            foreach (var obj in q.objectives)
            {
                objectiveStatuses.Add(new ObjectiveStatus(obj));
            }
            if (objectiveStatuses == null || objectiveStatuses.Count < 1)
            {
                CompleteQuest();
            }
        }

        public QuestSO GetQuest() => quest;
        public List<ObjectiveStatus> GetAllObjectiveStatus() => objectiveStatuses;

        public void CompleteObjective(ObjectiveSO objective, int number = 1)
        {
            var status = objectiveStatuses.Find(s => s.objective == objective);
            if (status == null) return;

            status.currentProgress += number;
            status.everCompleted = true;
        }

        public void CancelObjective(ObjectiveSO objective, int number = 1)
        {
            var status = objectiveStatuses.Find(s => s.objective == objective);
            if (status == null) return;

            status.currentProgress = Mathf.Max(0, status.currentProgress - number);
        }

        public bool IsAllObjectivesCompleted()
        {
            return objectiveStatuses.Count < 1 || objectiveStatuses.TrueForAll(s => s.IsCompleted());
        }

        public bool IsCompleted()
        {
            return progress == QuestProgress.Completed;
        }
        public void CompleteQuest()
        {
            progress = QuestProgress.Completed;
        }


        public bool IsFinished()
        {
            return progress == QuestProgress.Finished;
        }

        public bool IsInProgress()
        {
            return progress == QuestProgress.Active;
        }


        public void Finish()
        {
            if (!IsCompleted()) return;
            progress = QuestProgress.Finished;
            Debug.Log($"任务 {quest.GetQuestName()} 已完成并结算奖励");
        }


        public ObjectiveStatus GetObjectiveStatus(ObjectiveSO objective)
        {
            var s = objectiveStatuses.Find(x => x.objective == objective);
            return s;
        }

        public int GetProgress(ObjectiveSO objective)
        {
            var s = GetObjectiveStatus(objective);
            return s?.currentProgress ?? 0;
        }


        #region 存档相关

        /// <summary>
        /// 自己记录自己的状态，用于存档
        /// </summary>
        [System.Serializable]
        class QuestStatusRecord
        {
            public string questName;
            public QuestProgress progress;
            public List<ObjectiveStatusRecord> objectiveStatuses;
        }

        [System.Serializable]
        class ObjectiveStatusRecord
        {
            public string objectiveDescription;
            public int currentProgress;

        }

        public object CaptureState()
        {
            QuestStatusRecord record = new QuestStatusRecord
            {
                questName = quest.name,
                progress = progress,
                objectiveStatuses = new List<ObjectiveStatusRecord>()
            };

            foreach (var status in objectiveStatuses)
            {
                record.objectiveStatuses.Add(new ObjectiveStatusRecord
                {
                    objectiveDescription = status.objective.Description,
                    currentProgress = status.currentProgress,

                });
            }
            return record;
        }

        public void RestoreState(JObject state)
        {
            QuestStatusRecord record = state.ToObject<QuestStatusRecord>();
            if (record == null) return;
            QuestSO questSO = QuestSO.GetByName(record.questName);
            if (questSO == null)
            {
                Debug.LogError($"无法找到任务SO，任务名：{record.questName}");
                return;
            }
            quest = questSO;
            progress = record.progress;
            objectiveStatuses.Clear();
            foreach (var objRecord in record.objectiveStatuses)
            {
                ObjectiveSO objSO = ObjectiveSO.GetObjectiveByDescription(objRecord.objectiveDescription);
                if (objSO == null)
                {
                    Debug.LogError($"无法找到任务目标SO，描述：{objRecord.objectiveDescription}");
                    continue;
                }
                ObjectiveStatus objStatus = new ObjectiveStatus(objSO)
                {
                    currentProgress = objRecord.currentProgress,
                };
                objectiveStatuses.Add(objStatus);
            }
        }





        #endregion
    }

    public enum QuestProgress { None, Active, Completed, Finished }
}
