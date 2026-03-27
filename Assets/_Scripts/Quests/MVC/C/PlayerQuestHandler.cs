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
        [SerializeField] List<QuestSO> congfigQuests;
        [SerializeField] QuestObjectiveTracker objectiveTrackerPrefab;

        private List<QuestStatus> activeQuests = new();

        public event System.Action<QuestSO> OnQuestCompleted;
        public event System.Action<QuestSO> OnQuestProgressChanged;


        private void Start()
        {
            foreach (QuestSO quest in congfigQuests)
            {

                AcceptQuest(quest);

            }
            // OnQuestProgressChanged?.Invoke(null);

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
            SideMessageBox.ShowQuestAccepted(quest.GetQuestName());

            //接取任务后立马进行一次条件检测
            ConditionHandler.GetInstance().AnyConditionChanged();

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

        public bool IsQuestInProgress(QuestSO quest)
        {
            var qs = GetQuestStatus(quest);
            return qs != null && qs.IsInProgress();
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
            SideMessageBox.ShowQuestCompleted(quest.GetQuestName());
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
            //防止重复完成已结束的任务
            if (qs == null || qs.IsCompleted() || qs.IsFinished()) return;

            qs.CompleteObjective(objective, number);
            OnQuestProgressChanged?.Invoke(quest);
            Debug.Log("Objective Completed");

            if (qs.IsAllObjectivesCompleted())
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

            var questStatus = GetQuestStatus(questSo);
            // if (questStatus == null) return true;
            //是否已提交
            //todo:严格判断，只要接取了就不能再接取（除非中途放其了）
            //todo:如此一来对话端就要做更细化的分支,可接取、进行中、可提交、已完成，用四个分支进行细化对话
            if (HasQuest(questSo))
            {
                // Debug.Log("已任务" + questSo.GetQuestName());
                return false;
            }
            // if (GetQuestStatus(questSo)?.progress == QuestProgress.Finished)
            // {
            //     // Debug.Log("已任务" + questSo.GetQuestName());
            //     return false;
            // }
            //等级限制
            if (GetComponent<BaseStats>().CurrentLevel < questSo.questLevel)
            {
                // Debug.Log("等级不足" + questSo.GetQuestName());
                return false;
            }
            //前置任务限制
            foreach (var preQuest in questSo.preQuests)
            {
                if (!HasQuest(preQuest) || GetQuestStatus(preQuest).progress != QuestProgress.Finished)
                {
                    // Debug.Log("前置任务未完成" + questSo.GetQuestName());
                    return false;
                }
            }
            return true;
        }

        #region 条件检测
        /// <summary>
        /// 用于计算角色身上与任务相关的条件判断
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public bool? Evaluate(Predicate predicate, IEnumerable<ConditionSO.Parameter> parameters)
        {

            QuestSO quest = (QuestSO)parameters.ToArray()[0].scriptableObject;
            switch (predicate)
            {
                case Predicate.QuestCompleted:
                    if (quest == null) return false;
                    return HasQuest(quest) && IsQuestCompleted(quest);

                case Predicate.QuestFinished:

                    if (quest == null) return false;
                    return HasQuest(quest) && IsQuestFinished(quest);

                case Predicate.CanAcceptQuest:

                    return CanAcceptQuest(quest);
                case Predicate.QuestInprogress:
                    if (quest == null) return false;
                    return HasQuest(quest) && IsQuestInProgress(quest);
                default:
                    return null;
            }
        }
        #endregion

        //玩家身上的任务状态存档
        #region 存档相关
        enum SaveData
        {
            QuestStatuses,
        }

        JToken ISaveable.CapatureStateAsJToken()
        {
            JObject state = new JObject();
            IDictionary<string, JToken> statusDict = state;

            string saveKey = SaveData.QuestStatuses.ToString();
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
            string saveKey = SaveData.QuestStatuses.ToString();
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
        #endregion

    }

}
