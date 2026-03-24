using System.Collections.Generic;
using RPGDemo.Core.Strategies;
using RPGDemo.Stats;
using Unity.VisualScripting;
using UnityEngine;

namespace RPGDemo.Buffs
{
    /// <summary>
    /// buff 配置类
    /// </summary>
    [CreateAssetMenu(menuName = "RPGDemo/Buff/New Buff")]
    public class BuffSO : ScriptableObject
    {
        public string buffID;
        public string buffName;
        [TextArea] public string description;
        public Sprite icon;
        public float baseDuration = 10f; //基础持续时间，-1代表是永续buff
        public bool isStackable = false; //是否可叠层
        public int maxStack = 5; //最大叠层数
        public float tickInterval = 1f;  // 每隔多少秒触发一次 OnTick
        // public bool isDamageOrHeal;
        // public float damageValue = 0f;
        // public float healValue = 0f;
        public GameObject vfxPrefab;           // Buff 特效

        [Header("Buff数值")]
        public List<StatBonus> additiveBonuses = null; // 角色属性影响
        public List<StatBonus> percentageBonuses = null; // 角色属性影响



        public virtual void OnApply(Character target, BuffInstance instance) { }
        public virtual void OnRemove(Character target, BuffInstance instance) { }
        public virtual void OnTick(Character target, BuffInstance instance) { } // 每秒调用
    }


}
