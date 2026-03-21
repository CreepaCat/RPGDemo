using UnityEngine;
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using RPGDemo.Saving;

namespace RPGDemo.Inventories
{
    /// <summary>
    /// 用于player持有货币的组件
    /// </summary>
    public class Purse : MonoBehaviour, ISaveable
    {
        [SerializeField] private float startingBalance = 0f;
        [SerializeField] private float currentBalance = 0f;

        public event Action OnBalanceUpdated;

        private void Awake()
        {
            currentBalance = Mathf.Max(0f, startingBalance);
        }

        public static Purse GetPlayerPurse()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            return player?.GetComponent<Purse>();
        }

        public float GetBalance() => currentBalance;

        public bool CanAfford(float amount)
        {
            if (amount < 0f) return false;
            return currentBalance >= amount;
        }

        public void AddMoney(float amount)
        {
            if (amount <= 0f) return;
            currentBalance += amount;
            OnBalanceUpdated?.Invoke();
        }

        public bool SpendMoney(float amount)
        {
            if (amount <= 0f) return false;
            if (!CanAfford(amount)) return false;

            currentBalance -= amount;
            OnBalanceUpdated?.Invoke();
            return true;
        }

        private enum SaveData
        {
            CurrentBalance
        }

        JToken ISaveable.CapatureStateAsJToken()
        {
            JObject state = new JObject();
            IDictionary<string, JToken> stateDict = state;

            stateDict[SaveData.CurrentBalance.ToString()] = JToken.FromObject(currentBalance);
            return state;
        }

        void ISaveable.RestoreStateFromJToken(JToken s)
        {
            JObject state = s as JObject;
            IDictionary<string, JToken> stateDict = state;

            if (stateDict.ContainsKey(SaveData.CurrentBalance.ToString()))
            {
                currentBalance = stateDict[SaveData.CurrentBalance.ToString()].ToObject<float>();
                currentBalance = Mathf.Max(0f, currentBalance);
            }

            OnBalanceUpdated?.Invoke();
        }
    }
}
