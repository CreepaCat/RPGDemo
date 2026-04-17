using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using RPGDemo.Saving;
using RPGDemo.Stats;
using UnityEngine;

namespace RPGDemo.Inventories
{
    /// <summary>
    /// MVC Controller
    /// </summary>
    public class Equipment : MonoBehaviour, ISaveable, IStatModifierProvider
    {
        private Dictionary<EquipLocation, EquipableItem> _equiments = new();
        public event Action OnEquipmentUpdated;
        public event Action OnWeaponUpdated;

        void Awake()
        {

        }

        void Start()
        {
            //通知UI绘制
            OnEquipmentUpdated?.Invoke();
            OnWeaponUpdated?.Invoke();
            //todo：根据字典配置将所有装备装上
            // foreach (var pair in _equiments)
            // {
            //     AddItem(pair.Key, pair.Value);
            // }

        }

        public InventoryItem GetEquipableItemInSlot(EquipLocation location)
        {
            return _equiments.ContainsKey(location) ? _equiments[location] : null;
        }
        public void SetSlot(EquipLocation equipLocation)
        {
            if (_equiments.ContainsKey(equipLocation))
            {
                Debug.Log($"已有装备槽{equipLocation},不能重复添加");
                return;
            }
            _equiments[equipLocation] = null;
        }

        public bool HasEquippedWeapon()
        {
            if (!_equiments.ContainsKey(EquipLocation.Weapon))
            {
                return false;
            }
            return _equiments[EquipLocation.Weapon] != null;
        }

        public void AddItem(EquipLocation location, EquipableItem item)
        {

            Debug.Assert(item.GetAllowedEquipLocation() == location);


            _equiments[location] = item;
            if (location == EquipLocation.Weapon)
            {
                Player.GetInstance().IsInCombat = true;
                OnWeaponUpdated?.Invoke();
            }
            OnEquipmentUpdated?.Invoke();
        }

        public void RemoveItem(EquipLocation location)
        {

            _equiments[location] = null;
            if (location == EquipLocation.Weapon)
            {
                OnWeaponUpdated?.Invoke();
            }
            OnEquipmentUpdated?.Invoke();
        }

        #region 应用装备数值
        /// <summary>
        /// 附加数值修改
        /// </summary>
        /// <param name="statsType"></param>
        /// <returns></returns>
        public IEnumerable<float> GetAdditiveModifiers(StatsType statsType)
        {

            //遍历字典里的装备,读取对应数值修改项
            foreach (var item in _equiments.Values)
            {
                if (item == null) continue;
                foreach (var v in item.GetAdditiveModifiers(statsType))
                {
                    yield return v;
                }
            }

        }

        public IEnumerable<float> GetPercentageModifiers(StatsType statsType)
        {

            //遍历字典里的装备,读取对应数值修改项
            foreach (var item in _equiments.Values)
            {
                if (item == null) continue;
                foreach (var v in item.GetPercentageModifiers(statsType))
                {
                    yield return v;
                }
            }
        }
        #endregion

        //装备槽物品存档
        [System.Serializable]
        class EquipmentRecord
        {
            public EquipLocation equipLocation;
            public string itemID;
        }
        enum EquipmentSaveData
        {
            EquipmentRecord,
        }

        JToken ISaveable.CapatureStateAsJToken()
        {
            JObject state = new JObject();
            IDictionary<string, JToken> stateDict = state;

            string saveKey = EquipmentSaveData.EquipmentRecord.ToString();
            List<EquipmentRecord> equipmentDataToSave = new();
            //转换记录数据
            foreach (var pairs in _equiments)
            {
                equipmentDataToSave.Add(new EquipmentRecord()
                {
                    equipLocation = pairs.Key,
                    itemID = pairs.Value?.GetItemID()
                });
            }

            stateDict[saveKey] = JToken.FromObject(equipmentDataToSave);
            return state;

        }

        void ISaveable.RestoreStateFromJToken(JToken s)
        {
            JObject state = s as JObject;
            IDictionary<string, JToken> stateDict = state;
            string saveKey = EquipmentSaveData.EquipmentRecord.ToString();
            if (stateDict.ContainsKey(saveKey))
            {
                List<EquipmentRecord> equipmentRecords = stateDict[saveKey].ToObject<List<EquipmentRecord>>();
                //还原数据
                _equiments.Clear();
                foreach (var record in equipmentRecords)
                {
                    Debug.Log("读取装备槽");

                    _equiments[record.equipLocation] = InventoryItem.GetItemFromID(record.itemID) as EquipableItem;
                }
            }

        }


    }
}
