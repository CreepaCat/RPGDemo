using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using RPGDemo.Core;
using RPGDemo.Saving;
using RPGDemo.Stats;
using UnityEngine;

namespace RPGDemo.Quests
{
    public class PlayerQuestHandler : MonoBehaviour, IPredicateEvaluator, ISaveable
    {
        [SerializeField] List<QuestSO> quests;
        [SerializeField] QuestObjectiveTracker objectiveTrackerPrefab;

        private List<QuestStatus> activeQuests = new();

        public event System.Action<QuestSO> OnQuestCompleted;
        public event System.Action<QuestSO> OnQuestProgressChanged;


        private void Start()
        {
            // 确保持久化容器存在

            // quests = QuestSO.
            foreach (QuestSO quest in quests)
            {

                AcceptQuest(quest);

            }

        }


        public static PlayerQuestHandler GetInstance()
        {
            return GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerQuestHandler>();
        }

        public List<QuestStatus> GetActiveQuests() => activeQuests;


        public void AcceptQuest(QuestSO quest)
        {
            if (HasQuest(quest)) return;

            InstantiateQuestTracker(quest);

            activeQuests.Add(new QuestStatus(quest));
            OnQuestProgressChanged?.Invoke(quest);
        }
        /// <summary>
        /// 对每一个任务都添加一个任务追踪器
        /// </summary>
        /// <param name="quest"></param>
        private void InstantiateQuestTracker(QuestSO quest)
        {
            var trackerParent = new GameObject("QuestTracker_" + quest.GetQuestName());
            DontDestroyOnLoad(trackerParent);

            foreach (var objective in quest.GetQuestObjectives())
            {
                var tracker = Instantiate(objectiveTrackerPrefab, trackerParent.transform);
                tracker.Setup(quest, objective);

            }
        }

        public bool HasQuest(QuestSO quest) => activeQuests.Exists(q => q.quest == quest);
        public bool HasAnyQuest() => activeQuests.Any();

        public QuestStatus GetQuestStatus(QuestSO quest) => activeQuests.Find(q => q.quest == quest);
        public bool IsQuestCompleted(QuestSO quest)
        {
            var qs = GetQuestStatus(quest);
            return qs != null && qs.IsCompleted();
        }

        public bool IsQuestFinished(QuestSO quest)
        {
            var qs = GetQuestStatus(quest);
            return qs != null && qs.IsFinished();
        }

        /// <summary>
        /// 获得任务奖励时调用
        /// </summary>
        /// <param name="quest"></param>
        public void FinishQuest(QuestSO quest)
        {
            var qs = GetQuestStatus(quest);
            if (qs == null || !qs.IsCompleted()) return;

            qs.Finish();
            //finish后需要移除quest tracker
            GameObject tracker = GameObject.Find("QuestTracker_" + quest.GetQuestName());
            if (tracker != null)
            {
                Destroy(tracker);
            }
            OnQuestProgressChanged?.Invoke(quest);
            ConditionHandler.GetInstance().AnyConditionChanged();
        }

        #region ConditionHandler AnyConditionChanged事件回调方法

        public void CompleteObjective(QuestSO quest, ObjectiveSO objective, int number = 1)
        {
            if (!quest.HasObjective(objective))
            {
                Debug.LogWarning($"完成任务时失败，任务 {quest.GetQuestName()} 不包含目标 {objective.Description}");
                return;
            }
            var qs = GetQuestStatus(quest);
            if (qs == null) return;

            qs.CompleteObjective(objective, number);
            OnQuestProgressChanged?.Invoke(quest);
            Debug.Log("Objective Completed");

            if (qs.IsCompleted())
            {
                qs.CompleteQuest();
                OnQuestCompleted?.Invoke(quest);
            }


        }
        public void CancelObjective(QuestSO quest, ObjectiveSO objective, int number = 1)
        {
            var qs = GetQuestStatus(quest);
            if (qs == null) return;

            qs.CancelObjective(objective, number);
            OnQuestProgressChanged?.Invoke(quest);   // UI刷新

            Debug.Log("Objective Cancelled");
        }
        #endregion

        public bool CanAcceptQuest(QuestSO questSo)
        {
            if (questSo == null) return false;
            //是否已完成
            if (GetQuestStatus(questSo)?.progress == QuestProgress.Finished)
            {
                return false;
            }
            //等级限制
            if (GetComponent<BaseStats>().CurrentLevel < questSo.questLevel)
            {
                return false;
            }
            //前置任务限制
            foreach (var preQuest in questSo.preQuests)
            {
                if (!HasQuest(preQuest) || GetQuestStatus(preQuest)?.progress != QuestProgress.Finished)
                    return false;
            }
            return true;
        }
        /// <summary>
        /// 用于计算角色身上与任务相关的条件判断
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public bool? Evaluate(Predicate predicate, IEnumerable<ConditionSO.Parameter> parameters)
        {
            switch (predicate)
            {
                case Predicate.QuestCompleted:
                    QuestSO quest1 = (QuestSO)parameters.ToArray()[0].scriptableObject;
                    if (quest1 == null) return false;
                    return HasQuest(quest1) && IsQuestCompleted(quest1);

                case Predicate.QuestFinished:
                    QuestSO quest2 = (QuestSO)parameters.ToArray()[0].scriptableObject;
                    if (quest2 == null) return false;
                    return HasQuest(quest2) && IsQuestFinished(quest2);

                case Predicate.CanAcceptQuest:
                    QuestSO quest3 = (QuestSO)parameters.ToArray()[0].scriptableObject;
                    return CanAcceptQuest(quest3);
                default:
                    return null;
            }
        }


        //玩家身上的任务状态存档

        enum QuestSaveKeys
        {
            QuestStatuses,
        }

        JToken ISaveable.CapatureStateAsJToken()
        {
            JObject state = new JObject();
            IDictionary<string, JToken> statusDict = state;

            string saveKey = QuestSaveKeys.QuestStatuses.ToString();
            List<object> questStatusRecords = new();

            foreach (var questStatus in activeQuests)
            {
                object saveData = questStatus.CaptureState();
                questStatusRecords.Add(saveData);
            }
            //JToken表示可以嵌套在JObject内
            statusDict[saveKey] = JToken.FromObject(questStatusRecords);

            return state;
        }

        void ISaveable.RestoreStateFromJToken(JToken s)
        {
            JObject state = s as JObject;
            IDictionary<string, JToken> stateDict = state;
            string saveKey = QuestSaveKeys.QuestStatuses.ToString();
            if (!stateDict.ContainsKey(saveKey)) return;

            List<object> questStatusRecords = stateDict[saveKey].ToObject<List<object>>();

            //使用Newtonsoft.Json读取出来的objet对象为JObject类型
            foreach (JObject record in questStatusRecords)
            {
                QuestStatus questStatus = new QuestStatus(null);
                questStatus.RestoreState(record);
                activeQuests.Add(questStatus);
                if (questStatus.progress == QuestProgress.Active
                || questStatus.progress == QuestProgress.Completed)
                {
                    InstantiateQuestTracker(questStatus.quest);
                }
            }

        }


    }
}
