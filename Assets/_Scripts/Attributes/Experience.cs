using UnityEngine;
using RPGDemo.Stats;
using RPGDemo.Saving;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace RPGDemo.Attributes
{
    [RequireComponent(typeof(BaseStats))]
    public class Experience : MonoBehaviour, ISaveable
    {
        [SerializeField] private int totalExperience = 100;

        public int GetTotalExperience() => totalExperience;

        public void GainExp(int expToGain)
        {
            if (expToGain < 0) return;
            Debug.Log($"获得{expToGain}经验");
            totalExperience += expToGain;
            // totalExperience = Mathf.Min(totalExperience, maxExperience);

            var baseStats = GetComponent<BaseStats>();
            if (totalExperience >= baseStats.GetStats(StatsType.Experience))
            {
                baseStats.LevelUp(totalExperience);
            }

        }

        enum SaveData
        {
            Exp,
        }

        JToken ISaveable.CapatureStateAsJToken()
        {
            JObject state = new JObject();
            IDictionary<string, JToken> stateDict = state;
            stateDict[SaveData.Exp.ToString()] = totalExperience;

            return state;
        }

        void ISaveable.RestoreStateFromJToken(JToken s)
        {
            JObject state = s as JObject;
            IDictionary<string, JToken> stateDict = state;
            if (stateDict.ContainsKey(SaveData.Exp.ToString()))
            {
                stateDict[SaveData.Exp.ToString()] = totalExperience;
            }


        }
    }
}
