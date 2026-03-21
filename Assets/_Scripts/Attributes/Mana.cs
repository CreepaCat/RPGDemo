using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using RPGDemo.Saving;
using RPGDemo.Stats;
using UnityEngine;

namespace RPGDemo.Attributes
{
    [RequireComponent(typeof(BaseStats))]
    public class Mana : MonoBehaviour, ISaveable, ILazyInit
    {
        [SerializeField] private float maxMana = 100;
        [SerializeField] private float currentMana;

        float regenInterval = 5f;
        float lastRegenTimer = float.MaxValue;

        public bool IsInitialized { get; private set; }
        public event Action<float> OnManaChanged;

        private BaseStats baseStats;

        private void Awake()
        {
            currentMana = maxMana;
            baseStats = GetComponent<BaseStats>();
            baseStats.OnStatsReady += () => LazyInit(() =>
            {
                OnManaChanged?.Invoke(0f);
            });
        }

        private void OnEnable()
        {
            baseStats.OnLevelUp += OnLevelUp;
        }


        private void OnDisable()
        {
            baseStats.OnLevelUp -= OnLevelUp;
        }


        private void Update()
        {
            if (lastRegenTimer > regenInterval)
            {
                RegenMana(baseStats.GetStats(StatsType.ManaRegen));
                lastRegenTimer = 0f;
            }
            lastRegenTimer += Time.deltaTime;
        }


        private void OnLevelUp()
        {
            maxMana = baseStats.GetStats(StatsType.Mana);
            currentMana = maxMana;
            OnManaChanged?.Invoke(0);
        }


        public void LazyInit(Action onDone = null)
        {
            if (IsInitialized)
            {
                onDone?.Invoke();
                return;
            }

            maxMana = baseStats.GetStats(StatsType.Mana);
            currentMana = Mathf.Clamp(currentMana, 0f, maxMana);
            IsInitialized = true;
            onDone?.Invoke();
        }

        public bool TryUseMana(float manaCost)
        {
            LazyInit();

            if (manaCost < 0f) return false;
            if (currentMana < manaCost) return false;

            currentMana -= manaCost;
            OnManaChanged?.Invoke(manaCost);
            return true;
        }

        public void RecoverMana(float manaToRecover)
        {
            LazyInit();

            if (manaToRecover <= 0f) return;

            float manaBeforeRecover = currentMana;
            currentMana = Mathf.Clamp(currentMana + manaToRecover, 0f, maxMana);
            float actualRecovered = currentMana - manaBeforeRecover;

            if (actualRecovered > 0f)
            {
                OnManaChanged?.Invoke(-actualRecovered);
            }
        }

        /// <summary>
        /// 法力自动恢复，每五秒执行
        /// </summary>
        /// <param name="v"></param>
        public void RegenMana(float manaToRege)
        {
            LazyInit();
            if (GetComponent<Health>().IsDead()) return;

            if (manaToRege <= 0f) return;
            Debug.Log("自动恢复法力值");


            float manaBeforeRecover = currentMana;
            currentMana = Mathf.Clamp(currentMana + manaToRege, 0f, maxMana);
            float actualRecovered = currentMana - manaBeforeRecover;

            if (actualRecovered > 0f)
            {
                OnManaChanged?.Invoke(-actualRecovered);
            }

        }

        public float GetManaRatio()
        {
            LazyInit();
            if (maxMana <= 0f) return 0f;
            return currentMana / maxMana;
        }

        public float GetCurrentMana()
        {
            LazyInit();
            return currentMana;
        }

        public float GetMaxMana()
        {
            LazyInit();
            return maxMana;
        }

        private enum ManaSaveData
        {
            CurrentMana,
        }

        JToken ISaveable.CapatureStateAsJToken()
        {
            JObject state = new JObject();
            IDictionary<string, JToken> stateDict = state;

            string saveKey = ManaSaveData.CurrentMana.ToString();
            stateDict[saveKey] = JToken.FromObject(currentMana);

            return state;
        }

        void ISaveable.RestoreStateFromJToken(JToken s)
        {
            JObject state = s as JObject;
            IDictionary<string, JToken> stateDict = state;

            if (stateDict.ContainsKey(ManaSaveData.CurrentMana.ToString()))
            {
                currentMana = stateDict[ManaSaveData.CurrentMana.ToString()].ToObject<float>();
            }
        }



    }
}
