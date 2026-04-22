using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using RPGDemo.Saving;
using UnityEngine;
using UnityEngine.Events;

namespace RPGDemo.Inventories.Pickups
{
    public class PickupSpawner : MonoBehaviour, ISaveable
    {
        [SerializeField] InventoryItem item;
        [SerializeField] int amount = 1;

        public UnityEvent<int> OnPickup;

        private void Awake()
        {
            SpawnPickup();
        }

        private void SpawnPickup()
        {
            Pickup pickup = item.SpawnPickup(transform.position, amount);
            pickup.transform.SetParent(transform);
        }

        public Pickup GetPickup() => GetComponentInChildren<Pickup>();

        public bool IsCollected() => GetPickup() == null;

        public void PickupCallback(int quantityPickedup)
        {
            Debug.Log($"拾取了{item.GetDisplayName()} x{quantityPickedup}");
            OnPickup?.Invoke(quantityPickedup);
        }

        public void DestroyPickup()
        {
            if (GetPickup())
            {
                Destroy(GetPickup().gameObject);
            }
        }

        enum SaveData
        {
            IsCollected
        }


        JToken ISaveable.CapatureStateAsJToken()
        {
            Debug.Log("CaptureStateAsJToken PickupSpawner");
            JObject state = new JObject();
            IDictionary<string, JToken> stateDict = state;

            state[SaveData.IsCollected.ToString()] = JToken.FromObject(IsCollected());

            return state;
        }

        void ISaveable.RestoreStateFromJToken(JToken s)
        {
            Debug.Log("RestoreStateFromJToken PickupSpawner");
            JObject state = s as JObject;
            IDictionary<string, JToken> stateDict = state;

            if (stateDict.ContainsKey(SaveData.IsCollected.ToString()))
            {
                bool isCollected = stateDict[SaveData.IsCollected.ToString()].ToObject<bool>();
                if (isCollected)
                {
                    // Debug.Log("物品已被拾取，销毁");
                    DestroyPickup();
                }
            }
        }
    }
}
