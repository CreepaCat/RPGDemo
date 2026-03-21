using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using RPGDemo.Inventories.Pickups;
using RPGDemo.Saving;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPGDemo.Inventories.Utils
{
    public class ItemDropper : MonoBehaviour,ISaveable
    {
        //CACHE
        private List<Pickup> droppedPickups = null; //记录在本场景丢弃的物品（怪物的掉落物同样）
        private List<DropRecord> otherSceneDrops = new();  //记录在其它场景丢弃的物品

        private void Awake()
        {
            droppedPickups = new List<Pickup>();
        }

        public void DropItem(InventoryItem item, int amount)
        {
            if(amount <= 0) return;
            SpawnPickup(item, amount,GetDropLocation());
        
        }
        void SpawnPickup(InventoryItem item, int amount, Vector3 position)
        {
            Pickup pickupItem = item.SpawnPickup(position, amount);
            droppedPickups.Add(pickupItem);

        }

        protected virtual Vector3 GetDropLocation()
        {
           // return GameObject.FindGameObjectWithTag("Player").transform.position;
           return transform.position;
        }
        
        /// <summary>
        /// 掉落物结构体，用于记录其它场景的掉落物，和存档
        /// </summary>
        [System.Serializable]
        struct DropRecord
        {
            public string itemID;
            public int amount;
            public JToken JTokenPosition;
            public int sceneBuildIndex;
        }

        enum SaveData
        {
            DroppedPickups
        }
        JToken ISaveable.CapatureStateAsJToken()
        {
            Debug.Log("ItemDropper CapatureStateAsJToken");
            JObject state = new JObject();
            IDictionary<string, JToken> stateDict = state;
            
            var dropRecordsToSave = new List<DropRecord>();
            
            //先加入其它场景的掉落物记录列表
            dropRecordsToSave.AddRange(otherSceneDrops);
            
            int currentSceneBuildIndex = SceneManager.GetActiveScene().buildIndex;
            
            //再加入本场景的掉落物列表（剔除空掉落物,因为有些可能已被玩家拾取）
            var updatedDroppedPickups = new List<Pickup>();
            foreach (var pickup in droppedPickups)
            {
                if(pickup == null) continue;
                updatedDroppedPickups.Add(pickup);
            }
            droppedPickups =  updatedDroppedPickups;
            
            foreach (var droppedPickup in droppedPickups)
            {
                if(droppedPickup == null) continue;
                DropRecord dropRecord = new DropRecord()
                {
                    itemID = droppedPickup.GetItem().GetItemID(),
                    amount = droppedPickup.GetItemAmount(),
                    JTokenPosition = droppedPickup.transform.position.ToJToken(),
                    sceneBuildIndex = currentSceneBuildIndex
                };
                dropRecordsToSave.Add(dropRecord);
            }
            
            stateDict[SaveData.DroppedPickups.ToString()] = JToken.FromObject(dropRecordsToSave);
            return state;


        }

        void ISaveable.RestoreStateFromJToken(JToken s)
        {
            Debug.Log("ItemDropper RestoreStateFromJToken");
           JObject state = s as JObject;
           IDictionary<string, JToken> stateDict = state;

           if (stateDict.ContainsKey(SaveData.DroppedPickups.ToString()))
           {
               var dropRecordsList = stateDict[SaveData.DroppedPickups.ToString()].ToObject<List<DropRecord>>();
               //先清除缓存列表
               droppedPickups.Clear();
               otherSceneDrops.Clear();
               //读取本场景的掉落物并生成，若非本场景的则加入缓存列表
               int currentSceneBuildIndex = SceneManager.GetActiveScene().buildIndex;
               foreach (var droppedRecord in dropRecordsList)
               {
                   if (droppedRecord.sceneBuildIndex != currentSceneBuildIndex)
                   {
                       otherSceneDrops.Add(droppedRecord);
                       continue;
                   }
                   SpawnPickup(InventoryItem.GetItemFromID(droppedRecord.itemID), 
                       droppedRecord.amount, 
                       droppedRecord.JTokenPosition.ToVector3());
                   
               }
           }
        }
    }

   
}

