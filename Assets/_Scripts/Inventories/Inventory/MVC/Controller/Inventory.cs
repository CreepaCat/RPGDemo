using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using RPGDemo.Core;
using RPGDemo.Saving;
using UnityEngine;

namespace RPGDemo.Inventories
{
    /// <summary>
    ///角色背包
    /// </summary>
    public class Inventory : MonoBehaviour, ISaveable, IPredicateEvaluator
    {

        [SerializeField] private InventoryItem[] tesetItems;
        [SerializeField] private int maxSlotsNum = 20;
        [SerializeField] InventorySlot[] slots;

        public event Action OnInventoryUpdated;
        public int GetMaxSlotsNum() => maxSlotsNum;

        [System.Serializable]
        public struct InventorySlot
        {
            public InventoryItem item;
            public int amount;
            public int index;
        }

        private void Awake()
        {
            slots = new InventorySlot[maxSlotsNum];
            for (int i = 0; i < maxSlotsNum; i++)
            {
                slots[i].index = i;//记录slot的索引
            }

            //添加测试物品
            foreach (var item in tesetItems)
            {
                if (item == null) continue;
                AddItemToFirstFoundSlot(item, 1);
            }
        }


        /// <summary>
        /// 使用静态方法来规避单例
        /// </summary>
        /// <returns></returns>
        public static Inventory GetPlayerInventory()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            return player?.GetComponent<Inventory>();
        }

        public int GetSize() => maxSlotsNum;



        public InventoryItem GetItemInSlot(int index) => slots[index].item;



        public int GetItemAmountInSlot(int index) => slots[index].amount;

        public int GetTotalAmount(InventoryItem item)
        {
            //todo:同时计算加入ActionBar里物品的数量
            int total = 0;
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].item == item)
                {
                    total += slots[i].amount;
                }
            }
            return total;
        }

        public void RemoveFromSlot(int index, int amount)
        {
            if (index < 0 || index >= maxSlotsNum) return;
            slots[index].amount -= amount;
            if (slots[index].amount <= 0)
            {
                slots[index].amount = 0;
                slots[index].item = null;
            }
            Debug.Log("removing item ,OnInventoryUpdated");
            OnInventoryUpdated?.Invoke();

        }

        /// <summary>
        /// 添加到指定格子,用于物品拖拽换格子
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public bool AddToSlot(int index, InventoryItem item, int amount)
        {
            //先判断该格子的item是否相同,若该格子不为空且物品不同，则将新item添加到第一个空格
            if (slots[index].item != null && !ReferenceEquals(item, slots[index].item))
            {
                return AddItemToFirstFoundSlot(item, amount);

            }
            //否则进行物品放置(可能需要堆叠)
            SplitAdd(index, item, amount);

            //触发条件检测
            ConditionHandler.GetInstance().AnyConditionChanged();
            OnInventoryUpdated?.Invoke();
            return true;
        }

        public bool AddItemToFirstFoundSlot(InventoryItem item, int amountToAdd)
        {
            //find slot,先找空格，若没有则找可堆叠格
            int index = FindValidatedSlot(item, amountToAdd);

            if (index < 0)
            {
                Debug.Log("背包没有单个空格满足添加条件");
                return false;
            }

            //判断拆分添加
            SplitAdd(index, item, amountToAdd);

            //触发条件检测
            ConditionHandler.GetInstance().AnyConditionChanged();

            OnInventoryUpdated?.Invoke();
            return true;
        }

        //批量物品添加
        public bool AddItemDict(Dictionary<InventoryItem, int> itemDict)
        {
            var virtualSlots = VirtualAdd(itemDict);
            if (virtualSlots == null)
            {
                BottomMessageBox.ShowInventorySpaceNotEnough();
                return false;
            }
            //若可添加，直接把虚拟背包赋值给当前背包，进行全部添加
            slots = virtualSlots;
            ConditionHandler.GetInstance().AnyConditionChanged();
            OnInventoryUpdated?.Invoke();
            return true;
        }



        public bool HasSpaceFor(Dictionary<InventoryItem, int> itemDict)
        {
            return VirtualAdd(itemDict) == null ? false : true;

        }
        public bool HasSingleSlotSpaceFor(InventoryItem item, int amount)
        {
            return FindValidatedSlot(item, amount) >= 0;
        }



        /// <summary>
        /// 背包物品条件判断
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public bool? Evaluate(Predicate predicate, IEnumerable<ConditionSO.Parameter> parameters)
        {
            if (predicate == Predicate.HasItem)
            {
                var paraList = new List<ConditionSO.Parameter>(parameters);
                var para = paraList[0]; //一般就一个
                if (para == null)
                {
                    return true;
                }
                InventoryItem item = (InventoryItem)para.scriptableObject;
                int quantity = para.number;

                foreach (var slot in slots)
                {
                    if (slot.item == item && GetTotalAmount(item) >= quantity)
                    {
                        Debug.Log("有要求物品 ：" + item.name + "要求数量" + quantity);
                        return true;
                    }
                }
                return false;
            }
            return null;
        }

        public bool HasEmptySlot()
        {
            return FindEmptySlot() < 0 ? false : true;
        }

        /// <summary>
        /// 批量移除物品
        /// </summary>
        /// <param name="item"></param>
        /// <param name="amount"></param>

        public void RemoveItem(InventoryItem item, int amount)
        {
            Debug.Log("从背包移除物品");
            int remaind = amount;
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].item == null
                || !ReferenceEquals(slots[i].item, item))
                    continue;

                if (slots[i].amount <= remaind)
                {
                    slots[i].item = null;
                    remaind -= slots[i].amount;
                    slots[i].amount = 0;
                }
                else
                {
                    slots[i].amount -= remaind;
                    break;
                }

            }
            OnInventoryUpdated?.Invoke();
        }

        #region 背包整理

        public IEnumerable<InventorySlot> GetFilterSlots(ItemCategory category)
        {
            foreach (var slot in slots)
            {
                if (slot.item == null || slot.item.GetCategory() != category) continue;
                yield return slot;
            }
        }

        /// <summary>
        /// 背包整理：合并同类堆叠，并按分类+名称排序后紧凑排列到前排格子。
        /// </summary>
        public void OrganizeInventory()
        {
            //将背包所有物品转为字典缓存
            Dictionary<InventoryItem, int> totalPerItem = new Dictionary<InventoryItem, int>();

            for (int i = 0; i < slots.Length; i++)
            {
                var item = slots[i].item;
                var amount = slots[i].amount;

                if (item == null || amount <= 0) continue;

                if (!totalPerItem.ContainsKey(item))
                {
                    totalPerItem[item] = 0;
                }
                totalPerItem[item] += amount;
            }
            //所有物品按品类排序
            List<InventoryItem> sortedItems = new List<InventoryItem>(totalPerItem.Keys);
            sortedItems.Sort((a, b) =>
            {
                //按品类
                int categoryCompare = a.GetCategory().CompareTo(b.GetCategory());
                if (categoryCompare != 0) return categoryCompare;
                //按名字
                int nameCompare = string.Compare(a.GetDisplayName(), b.GetDisplayName(), StringComparison.Ordinal);
                if (nameCompare != 0) return nameCompare;
                //按id
                return string.Compare(a.GetItemID(), b.GetItemID(), StringComparison.Ordinal);
            });

            //清空当前背包格，并记录索引
            for (int i = 0; i < slots.Length; i++)
            {
                slots[i].item = null;
                slots[i].amount = 0;
                slots[i].index = i;
            }

            //按排序后的list顺序，将字典缓存添加到背包
            int nextSlotIndex = 0;
            foreach (var item in sortedItems)
            {
                int remain = totalPerItem[item];
                int maxStack = item.GetMaxStackableAmount();

                while (remain > 0 && nextSlotIndex < slots.Length)
                {
                    int toPut = Mathf.Min(remain, maxStack);
                    slots[nextSlotIndex].item = item;
                    slots[nextSlotIndex].amount = toPut;
                    slots[nextSlotIndex].index = nextSlotIndex;

                    remain -= toPut;
                    nextSlotIndex++;
                }

                if (remain > 0)
                {
                    Debug.LogWarning($"背包整理时空间不足，物品[{item.GetDisplayName()}]剩余数量:{remain}");
                }
            }

            // ConditionHandler.GetInstance().AnyConditionChanged();
            OnInventoryUpdated?.Invoke();
        }
        #endregion


        //PRIVATE

        /// <summary>
        /// 虚拟添加，判断是否有足够背包空间
        /// </summary>
        /// <param name="itemDict"></param>
        /// <returns></returns>
        private InventorySlot[] VirtualAdd(Dictionary<InventoryItem, int> itemDict)
        {
            if (itemDict == null || itemDict.Count == 0) return null;

            //用虚拟数组背包格检查格子占用
            InventorySlot[] virtualSlots = new InventorySlot[slots.Length];
            Array.Copy(slots, virtualSlots, slots.Length);

            foreach (var itemPair in itemDict)
            {
                InventoryItem item = itemPair.Key;
                int amountToAdd = itemPair.Value;

                if (item == null) return null;
                if (amountToAdd <= 0) continue;

                int maxStack = item.GetMaxStackableAmount();
                if (maxStack <= 0) return null;

                int remain = amountToAdd;

                //先尝试堆叠到现有同类格子
                for (int i = 0; i < virtualSlots.Length && remain > 0; i++)
                {
                    if (!ReferenceEquals(virtualSlots[i].item, item)) continue;
                    if (virtualSlots[i].amount >= maxStack) continue;

                    int stackSpace = maxStack - virtualSlots[i].amount;
                    int toStack = Mathf.Min(stackSpace, remain);
                    virtualSlots[i].amount += toStack;
                    remain -= toStack;
                }

                //再尝试占用空格
                for (int i = 0; i < virtualSlots.Length && remain > 0; i++)
                {
                    if (virtualSlots[i].item != null && virtualSlots[i].amount > 0) continue;

                    int toPut = Mathf.Min(maxStack, remain);
                    virtualSlots[i].item = item;
                    virtualSlots[i].amount = toPut;
                    remain -= toPut;
                }

                if (remain > 0) return null;
            }
            return virtualSlots;
        }

        private void SplitAdd(int index, InventoryItem item, int amountToAdd)
        {
            if (amountToAdd > item.GetMaxStackableAmount() - slots[index].amount)
            {
                slots[index].item = item;
                int toBeAdd = item.GetMaxStackableAmount() - slots[index].amount;
                slots[index].amount = item.GetMaxStackableAmount();
                //Debug.Log("拆分添加");
                int rest = amountToAdd - toBeAdd;
                AddItemToFirstFoundSlot(item, rest);
            }
            else
            {
                // Debug.Log("普通添加");
                slots[index].item = item;
                slots[index].amount += amountToAdd;
            }
        }

        private int FindValidatedSlot(InventoryItem item, int amountToAdd)
        {
            //先寻找可叠放格
            int index = FindStack(item, amountToAdd);

            if (index < 0)
            {
                index = FindEmptySlot();
            }

            return index;
        }

        private int FindEmptySlot()
        {

            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].item == null || slots[i].amount == 0)
                {

                    return i;
                }
            }
            return -1;
        }

        private int FindStack(InventoryItem item, int amountToAdd)
        {

            for (int i = 0; i < slots.Length; i++)
            {
                if (ReferenceEquals(slots[i].item, item)
                    && slots[i].amount + amountToAdd <= item.GetMaxStackableAmount())
                {

                    return i;
                }
            }

            return -1;
        }

        [System.Serializable]
        private struct InventorySlotSaveData
        {
            public string itemID;
            public int amount;
        }

        enum SaveData
        {
            Slot,
        }

        JToken ISaveable.CapatureStateAsJToken()
        {
            Debug.Log("Inventory CapatureStateAsJToken");
            JObject state = new JObject();
            IDictionary<string, JToken> stateDict = state;

            InventorySlotSaveData[] dataToSave = new InventorySlotSaveData[slots.Length];

            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].item == null) continue;
                dataToSave[i].itemID = slots[i].item.GetItemID();
                dataToSave[i].amount = slots[i].amount;
            }
            stateDict[SaveData.Slot.ToString()] = JToken.FromObject(dataToSave);
            return state;
        }

        void ISaveable.RestoreStateFromJToken(JToken s)
        {
            JObject state = s as JObject;
            IDictionary<string, JToken> stateDict = state;

            if (stateDict.ContainsKey(SaveData.Slot.ToString()))
            {
                InventorySlotSaveData[] saveData = stateDict[SaveData.Slot.ToString()].ToObject<InventorySlotSaveData[]>();

                for (int i = 0; i < saveData.Length; i++)
                {
                    slots[i].item = InventoryItem.GetItemFromID(saveData[i].itemID);
                    slots[i].amount = saveData[i].amount;

                }
            }
            Debug.Log("Inventory Restored");
            OnInventoryUpdated?.Invoke();
        }


    }
}
