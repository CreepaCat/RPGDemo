using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using RPGDemo.Saving;
using RPGDemo.Stats;
using UnityEngine;


namespace RPGDemo.Attributes
{
    [RequireComponent(typeof(BaseStats))]
    public class Health : MonoBehaviour, ISaveable, ILazyInit
    {
        [SerializeField] private float maxHealth = 100;
        [SerializeField] private float currentHealth;
        [SerializeField] private DamageNumberWorldUI damageNumberPrefab;
        [SerializeField] private Transform damageNumberStartPos;

        //自动恢复
        private float regenInterval = 5f;
        private float lastRegenTimer = float.MaxValue;

        public bool IsInitialized { get; private set; }
        public event Action<float> OnHealthChanged;
        //public event Action<float> OnHeal;
        public event Action OnDeath;
        private BaseStats baseStats;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            currentHealth = maxHealth;
            baseStats = GetComponent<BaseStats>();
            baseStats.OnStatsReady += () => LazyInit(() =>
            {
                OnHealthChanged?.Invoke(0);
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
                RegenHealth(baseStats.GetStats(StatsType.HealthRegen));
                lastRegenTimer = 0f;
            }
            lastRegenTimer += Time.deltaTime;
        }

        public bool IsDead()
        {
            LazyInit();
            return currentHealth <= 0f;
        }

        public bool IsFullHealth()
        {
            LazyInit();
            return currentHealth >= maxHealth;
        }
        private void OnLevelUp()
        {
            maxHealth = baseStats.GetStats(StatsType.Health);
            currentHealth = maxHealth;
            OnHealthChanged?.Invoke(0);
        }


        public void TakeDamage(float damageToTake)
        {
            //  LazyInit();

            if (IsDead()) return;
            if (damageToTake < 0f)
            {
                //或调用takeHeal
                return;
            }


            currentHealth -=
            currentHealth - damageToTake < 0f
            ? currentHealth
            : damageToTake;

            OnHealthChanged?.Invoke(damageToTake);

            if (IsDead())
            {
                Debug.Log(transform.name + "角色死亡");
                OnDeath?.Invoke();
            }
            else
            {
                Debug.Log(transform.name + "受到伤害" + damageToTake);
                DamageNumberWorldUI damageNumber = Instantiate(damageNumberPrefab);
                damageNumber.transform.position = damageNumberStartPos.position;
                damageNumber.SetTextNumber(damageToTake);
            }
        }

        public void Heal(float healToTake)
        {
            if (IsDead()) return;
            if (healToTake < 0f)
            {
                return;
            }


            currentHealth += currentHealth + healToTake >= maxHealth
            ? (maxHealth - currentHealth)
            : healToTake;

            OnHealthChanged?.Invoke(-healToTake);
            Debug.Log(transform.name + "受到治疗" + healToTake);


        }

        /// <summary>
        /// 生命自动恢复，不同于Heal,不会触发Heal事件
        ///
        /// </summary>
        /// <param name="value"></param>
        public void RegenHealth(float value)
        {
            if (IsDead()) return;
            if (value < 0f)
            {
                return;
            }
            Debug.Log("自动恢复生命值");


            currentHealth += currentHealth + value >= maxHealth
            ? (maxHealth - currentHealth)
            : value;

            OnHealthChanged?.Invoke(-value);
        }


        public float GetHealthRatio()
        {
            // EnsureInitialized();
            LazyInit();
            if (maxHealth <= 0f) return 0f;
            return currentHealth / maxHealth;
        }

        public void LazyInit(Action onDone = null)
        {
            if (IsInitialized)
            {
                onDone?.Invoke();
                return;
            }

            maxHealth = baseStats.GetStats(StatsType.Health);
            //将当前血量限制在正常范围
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            IsInitialized = true;
            onDone?.Invoke();
        }



        enum HealthSaveData
        {
            CurrentHealth,
        }

        JToken ISaveable.CapatureStateAsJToken()
        {

            JObject state = new JObject();
            IDictionary<string, JToken> stateDict = state;

            string saveKey = HealthSaveData.CurrentHealth.ToString();
            stateDict[saveKey] = JToken.FromObject(currentHealth);

            return state;
        }

        void ISaveable.RestoreStateFromJToken(JToken s)
        {
            JObject state = s as JObject;
            IDictionary<string, JToken> stateDict = state;

            if (stateDict.ContainsKey(HealthSaveData.CurrentHealth.ToString()))
            {
                currentHealth = stateDict[HealthSaveData.CurrentHealth.ToString()].ToObject<float>();

            }

        }


    }
}
