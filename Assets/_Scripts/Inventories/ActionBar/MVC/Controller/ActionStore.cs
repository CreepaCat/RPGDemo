using System;
using System.Collections.Generic;
using System.Linq;
using Core.AudioSystem;
using Newtonsoft.Json.Linq;
using RPGDemo.Saving;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RPGDemo.Inventories.ActionBar
{
    /// <summary>
    /// 角色身上的快捷栏物品缓存
    /// </summary>
    public class ActionStore : MonoBehaviour, ISaveable
    {
        [SerializeField] SoundData usingPotion;
        //冷却物品缓存
        Dictionary<ActionItem, float> cooldownItems = new Dictionary<ActionItem, float>();

        //快捷栏物品
        Dictionary<int, ActionSlot> actionItems = new Dictionary<int, ActionSlot>();

        public event Action OnActionStoreUpdated;
        public event Action<int> OnUse;


        private void Start()
        {
            // OnActionStoreUpdated?.Invoke();
        }

        private void Update()
        {
            for (int i = 0; i < 4; i++)
            {
                if (Keyboard.current[Key.Digit1 + i].wasPressedThisFrame)
                {
                    Debug.Log($"按下快捷键{(Key)(Key.Digit1 + i)}");
                    Use(i, gameObject);
                }
            }

            //更新物品冷却
            UpdateCooldownItems();
        }

        private void UpdateCooldownItems()
        {
            var keys = cooldownItems.Keys.ToList();
            foreach (var key in keys)
            {
                cooldownItems[key] -= Time.deltaTime;
                if (cooldownItems[key] <= 0f)
                {
                    //移除
                    cooldownItems.Remove(key);
                }
            }
        }

        //运行时数据
        [System.Serializable]
        class ActionSlot
        {
            public ActionItem actionItem;
            public int amount;
        }

        public bool Use(int index, GameObject user)
        {
            Debug.Log($"{gameObject.name}使用了快捷键{index}");
            if (!actionItems.ContainsKey(index)) return false;

            var actionItem = actionItems[index].actionItem;

            if (actionItem == null || actionItems[index].amount < 1) return false;
            //检查冷却
            if (cooldownItems.ContainsKey(actionItem))
            {
                return false;
            }


            if (actionItem.Use(user))
            {
                if (actionItem.IsConsumable())
                {
                    actionItems[index].amount -= 1;
                    SoundManager.Instance.CreateSound()
                            .WithSound(usingPotion)
                            .WithPlayPosition(transform.position)
                            .Play();

                }
                else
                {
                    print("使用非消耗品");
                }
                AddCooldownItem(actionItem);

                OnActionStoreUpdated?.Invoke(); //刷新UI
                return true;

            }

            return false;

        }
        #region 物品冷却相关
        public void AddCooldownItem(ActionItem actionItem)
        {
            Debug.Log($"物品{actionItem.GetDisplayName()}进入冷却");
            cooldownItems[actionItem] = actionItem.cooldown;
        }

        public float GetCooldownRatio(ActionItem actionItem)
        {
            if (!cooldownItems.ContainsKey(actionItem)) return 0;
            return cooldownItems[actionItem] / actionItem.cooldown;
        }

        #endregion




        public ActionItem GetItemInSlot(int index)
        {
            if (!actionItems.ContainsKey(index))
            {
                return null;
            }
            return actionItems[index].actionItem;
        }

        public int GetAmountInSlot(int index)
        {
            if (!actionItems.ContainsKey(index))
            {
                return 0;
            }
            return actionItems[index].amount;
        }

        public void AddItems(int index, ActionItem item, int amountToAdd)
        {

            //（快捷栏物品应直接反应背包同类物品的总数）
            //因此，若添加物品 和 原物品相同，则直接添加数量
            //若没添加过该物品，则新建加入
            if (actionItems.ContainsKey(index))
            {
                if (ReferenceEquals(actionItems[index].actionItem, item))
                {

                    print("替换相同物品到快捷栏" + item.GetDisplayName());
                    actionItems[index].amount += amountToAdd;
                }
            }
            else
            {
                AddToSlot(index, item, amountToAdd);
            }

            OnActionStoreUpdated?.Invoke();
        }

        public void RemoveItems(int index, int amountToRemove)
        {
            if (!actionItems.ContainsKey(index)) return;

            actionItems[index].amount -= amountToRemove;
            if (actionItems[index].amount <= 0)
            {
                actionItems.Remove(index);
            }

            OnActionStoreUpdated?.Invoke();

        }

        public int GetMaxAcceptable(int index, InventoryItem item)
        {
            ActionItem actionItem = item as ActionItem;
            if (actionItem == null)
            {
                print("非快捷物品");
                return 0;
            }

            if (actionItem.IsConsumable())
            {
                print("消耗品");
                return actionItem.GetMaxStackableAmount();
            }

            if (actionItems.ContainsKey(index)
                && object.ReferenceEquals(actionItems[index].actionItem, actionItem))
            {
                Debug.Log("相同物品");
                return 0;
            }

            //技能也是一种快捷物品,可使用的道具也是

            print("一般快捷品，只能放一个");

            return 1;
        }

        private void AddToSlot(int index, ActionItem item, int amountToAdd)
        {
            var slot = new ActionSlot();
            slot.actionItem = item;
            slot.amount = amountToAdd;
            actionItems[index] = slot; //直接修改快捷栏缓存

        }

        enum SaveData
        {
            ActionStore,
        }

        [System.Serializable]
        struct ActionSlotSaveData
        {
            public string itemID;
            public int amount;
            public int index;
        }


        JToken ISaveable.CapatureStateAsJToken()
        {
            Debug.Log("ActionSlot CapatureStateAsJToken");
            JObject state = new JObject();
            IDictionary<string, JToken> stateDict = state;

            string key = SaveData.ActionStore.ToString();
            List<ActionSlotSaveData> saveData = new();
            foreach (var keyValuePair in actionItems)
            {
                if (keyValuePair.Value.actionItem == null) continue;
                ActionSlotSaveData saveItem = new ActionSlotSaveData()
                {
                    itemID = keyValuePair.Value.actionItem.GetItemID(),
                    amount = keyValuePair.Value.amount,
                    index = keyValuePair.Key
                };
                saveData.Add(saveItem);
            }
            stateDict[key] = JToken.FromObject(saveData);

            return state;
        }

        void ISaveable.RestoreStateFromJToken(JToken s)
        {
            Debug.Log("ActionSlot RestoreStateFromJToken");
            JObject state = s as JObject;
            IDictionary<string, JToken> stateDict = state;
            string savedKey = SaveData.ActionStore.ToString();

            if (stateDict.ContainsKey(savedKey))
            {
                List<ActionSlotSaveData> saveData = new();
                saveData = stateDict[savedKey].ToObject<List<ActionSlotSaveData>>();

                foreach (var savedItem in saveData)
                {
                    var actionSlot = new ActionSlot();
                    actionSlot.actionItem = InventoryItem.GetItemFromID(savedItem.itemID) as ActionItem;
                    actionSlot.amount = savedItem.amount;
                    actionItems[savedItem.index] = actionSlot;
                }

                OnActionStoreUpdated?.Invoke();
            }
        }


    }
}
