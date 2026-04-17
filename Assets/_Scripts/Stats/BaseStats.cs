using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using RPGDemo.Saving;
using UnityEngine;
using System;
using RPGDemo.Core;
using Core.AudioSystem;

namespace RPGDemo.Stats
{

    public class BaseStats : MonoBehaviour, ISaveable
    {
        [field: SerializeField] public CharacterClass CharacterClass = CharacterClass.Golem;
        [SerializeField] private int _startingLevel = 1;
        [field: SerializeField, Range(1, 99)] public int CurrentLevel { get; private set; } = 1;
        [SerializeField] Procession _processionConfig = null;

        public event Action OnStatsReady; //懒加载事件

        [SerializeField] SoundData levelUp;



        [Header("升级相关")]

        [SerializeField] GameObject _levelUpEffect = null; //升级特效

        public event Action OnLevelUp;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            CurrentLevel = _startingLevel;
        }

        public float GetStats(StatsType statsType)
        {
            if (_processionConfig == null) return -1;
            //应用数值修改
            var total = GetTotalModifier(statsType);
            return (_processionConfig.GetLevelStats(CharacterClass, statsType, CurrentLevel) +
                    total.x) *
                    (1 + total.y);
        }

        public Vector2 GetTotalModifier(StatsType statsType)
        {
            Vector2 total = Vector2.zero;
            //直接获取当前stat脚本挂载对象身上所有的数值修改
            foreach (var provider in GetComponents<IStatModifierProvider>())
            {
                //固定附加值
                foreach (var modifier in provider.GetAdditiveModifiers(statsType))
                {
                    total.x += modifier;
                }
                //百分比附加值
                foreach (var modifier in provider.GetPercentageModifiers(statsType))
                {
                    total.y += modifier * 0.01f;
                }
            }
            return total;
        }



        /// <summary>
        /// 根据总经验设置当前等级
        /// </summary>
        /// <param name="totalExperience"></param>
        /// <returns></returns>
        public int SetLevelByTotalExp(int totalExperience)
        {
            Debug.Log($"设置角色{gameObject.name}等级,当前总经验{totalExperience}");
            int maxLevelStep = _processionConfig.GetMaxLevelStep(CharacterClass);
            for (int levelStep = 1; levelStep <= maxLevelStep; levelStep++)
            {
                //Experience进度表是从0开始,比level小1
                float XPToLevelUp = _processionConfig.GetLevelStats(CharacterClass, StatsType.Experience, levelStep);
                if (XPToLevelUp > totalExperience)
                {
                    CurrentLevel = levelStep; //升级所需经验不足,停在当前等级
                    Debug.Log($"升级到等级{levelStep + 1}所需总经验不足,设置角色{gameObject.name}等级{CurrentLevel}");
                    return CurrentLevel;
                }
            }
            //已达到最高等级
            CurrentLevel = maxLevelStep + 1;
            Debug.Log($"满足升级所需经验,设置角色{gameObject.name}等级{CurrentLevel}");


            return CurrentLevel;
        }

        /// <summary>
        /// 循环调用自己,来实现一次性获得大量经验时连续升级的情况
        /// 仅当确定可以升级时才调用
        /// </summary>
        /// <param name="totalExperience"></param> <summary>

        public void LevelUp(int totalExperience)
        {
            int maxLevel = _processionConfig.GetMaxLevelStep(CharacterClass) + 1;
            if (CurrentLevel == maxLevel)
            {
                print("已到达当前最大等级");
                return;
            }

            this.CurrentLevel += 1;
            //触发条件检测
            ConditionHandler.GetInstance().AnyConditionChanged();
            OnLevelUp?.Invoke();
            print($"LevelUp 事件发布,当前等级{CurrentLevel}");
            SoundManager.Instance.CreateSound()
                  .WithSound(levelUp)
                  .WithPlayPosition(transform.position)
                  .Play();
            LevelUpEffect();
            BottomMessageBox.ShowCustom($"等级提升!当前等级Lv. {CurrentLevel}");
            if (totalExperience >= GetStats(StatsType.Experience))
            {
                LevelUp(totalExperience);
            }

        }

        /// <summary>
        /// 升级特效
        /// </summary>
        private void LevelUpEffect()
        {
            if (_levelUpEffect == null) return;
            GameObject vfx = Instantiate(_levelUpEffect, transform);
            // vfx.transform.position = transform.position + Vector3.up;
        }

        //数值存档
        //数值系统只需记录初始等级，下次从什么等级进入游戏即可
        enum StatsSaveData
        {
            CurrentLevel,
        }

        JToken ISaveable.CapatureStateAsJToken()
        {
            JObject state = new JObject();
            IDictionary<string, JToken> stateDict = state;

            string saveKey = StatsSaveData.CurrentLevel.ToString();
            stateDict[saveKey] = JToken.FromObject(CurrentLevel);

            return state;


        }

        void ISaveable.RestoreStateFromJToken(JToken s)
        {
            JObject state = s as JObject;
            IDictionary<string, JToken> stateDict = state;

            string saveKey = StatsSaveData.CurrentLevel.ToString();
            if (stateDict.ContainsKey(saveKey))
            {
                //应用数值
                CurrentLevel = stateDict[saveKey].ToObject<int>();
                OnStatsReady?.Invoke();
            }
            Debug.Log("base state RestoreStateFromJToken");
        }
    }
}
