using System.Collections.Generic;
using System.Linq;
using RPGDemo.Inventories;
using UnityEngine;

namespace RPGDemo.Quests
{

    [CreateAssetMenu(fileName = "New Quest", menuName = "RPGDemo/Quest/New Quest", order = 1)]
    public class QuestSO : ScriptableObject
    {
        //CONFIG
        //  [field: SerializeField] public string questID { private set; get; }
        [field: SerializeField] public string questName { private set; get; }
        [field: SerializeField] public int questLevel { private set; get; }

        [TextArea]
        [field: SerializeField] public string description;
        [field: SerializeField] public List<ObjectiveSO> objectives { private set; get; } = new List<ObjectiveSO>();

        [field: SerializeField] public List<RewardItem> rewardItems { private set; get; } = new List<RewardItem>();
        [field: SerializeField] public int rewardCoins { private set; get; } = -1;
        [field: SerializeField] public int rewardExp { private set; get; } = -1;
        [field: SerializeField] public List<QuestSO> preQuests { private set; get; } = new List<QuestSO>();
        [SerializeField] public QuestSO postQuest;  //后续任务，完成此任务后自动接取

        //CACHE
        private static Dictionary<string, QuestSO> questsDict;


        [System.Serializable]
        public class RewardItem
        {
            public int quantity = 1;
            public InventoryItem item; //奖励物品

        }


        //全局查找
        public static QuestSO GetByName(string questName)
        {
            if (questsDict == null)
            {
                BuildLookup();
            }

            if (string.IsNullOrEmpty(questName) || !questsDict.ContainsKey(questName)) return null;

            return questsDict[questName];
        }

        private static void BuildLookup()
        {
            questsDict = new Dictionary<string, QuestSO>();
            QuestSO[] questSOs = Resources.LoadAll<QuestSO>(""); //从资源文件夹加载
            foreach (var questSO in questSOs)
            {

                if (string.IsNullOrEmpty(questSO.name))
                {
                    Debug.LogAssertionFormat("任务ID不能为空, name:{1}", questSO.name);
                    continue;
                }
                else if (questsDict.ContainsKey(questSO.name))
                {
                    Debug.LogAssertionFormat("重复任务,ID:{0} name:{1}", questSO.name, questSO.name);
                    continue;
                }
                questsDict[questSO.name] = questSO;
            }
        }

        public bool HasObjective(ObjectiveSO objective)
        {
            foreach (var item in objectives)
            {
                if (item == objective)
                    return true;
            }
            return false; ;
        }

        public ObjectiveSO GetObjective(ObjectiveSO objective)
        {
            foreach (var item in objectives)
            {
                if (item == objective)
                {
                    return item;
                }

            }
            return null;

        }

        public static List<QuestSO> GetAllQuests()
        {
            if (questsDict == null)
            {
                BuildLookup();
            }
            return questsDict.Values.ToList<QuestSO>();
        }

        public string GetQuestName() => questName;
        public int GetQuestLevel() => questLevel;
        public string GetQuestDescription() => description;
        public List<ObjectiveSO> GetQuestObjectives() => objectives;
        public List<RewardItem> GetQuestRewardItems() => rewardItems;
        public int GetQuestRewardCoins() => rewardCoins;
        public int GetQuestRewardExp() => rewardExp;








    }
}
