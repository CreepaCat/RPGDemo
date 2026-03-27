using System;
using RPGDemo.Core;
using RPGDemo.Core.DraggingFrame;
using RPGDemo.Inventories.Utils;
using UnityEngine;

namespace RPGDemo.Inventories
{
    public class InventoryDropTarget : MonoBehaviour, IDragDestination<InventoryItem>
    {

        /// <summary>
        /// 作为物品垃圾桶，容纳量为无限
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int GetMaxAcceptable(InventoryItem item)
        {
            return int.MaxValue;
        }

        public void AddItems(InventoryItem item, int amount)
        {
            Debug.Log("丢弃物品 " + amount + " items" + item.GetDisplayName());
            GameObject.FindGameObjectWithTag("Player").GetComponent<RandomDropper>().DropItem(item, amount);
            SideMessageBox.ShowDrop(item, amount);
            //触发条件检测
            ConditionHandler.GetInstance().AnyConditionChanged();
        }
    }
}
