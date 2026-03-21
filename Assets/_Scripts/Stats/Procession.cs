using System.Collections.Generic;
using UnityEngine;

namespace RPGDemo.Stats
{

    [CreateAssetMenu(fileName = "New Procession", menuName = "Procession/New Procession")]
    public class Procession : ScriptableObject
    {
        //CONFIG
        [SerializeField] CharacterProcession[] characterClassesProcession = null;

        private Dictionary<CharacterClass, Dictionary<StatsType, float[]>> _lookupTable;

        public float GetLevelStats(CharacterClass characterClass, StatsType statsType, int level)
        {
            if (_lookupTable == null)
            {
                BuildLookup();

            }
            if (_lookupTable.ContainsKey(characterClass))
            {
                if (_lookupTable[characterClass].ContainsKey(statsType))
                {
                    int statsLength = _lookupTable[characterClass][statsType].Length;
                    level = Mathf.Min(statsLength, level);
                    return _lookupTable[characterClass][statsType][level - 1];//索引从0开始,而等级从1开始
                }
            }
            //返回-1代表不存在该数值
            return -1;
        }


        public int GetMaxLevelStep(CharacterClass characterClass)
        {
            if (_lookupTable == null)
                BuildLookup();
            Debug.Log($"GetMaxLevel lookupTable characterClass:{characterClass} StatsType:{StatsType.Experience}");
            return _lookupTable[characterClass][StatsType.Experience].Length; //等级从1开始
        }

        private void BuildLookup()
        {
            //初始化缓存字典
            _lookupTable = new();
            foreach (var characterProcession in characterClassesProcession)
            {
                Dictionary<StatsType, float[]> innerDict = new();
                foreach (var stats in characterProcession.stats)
                {
                    innerDict[stats.statsType] = stats.levels;
                }

                _lookupTable[characterProcession.characterClass] = innerDict;
            }
        }
        [System.Serializable]
        public class CharacterProcession
        {

            public string name;
            public CharacterClass characterClass;
            public List<CharacterStats> stats;
        }

        [System.Serializable]
        public class CharacterStats
        {
            public StatsType statsType;
            public float[] levels;
        }




    }
}
