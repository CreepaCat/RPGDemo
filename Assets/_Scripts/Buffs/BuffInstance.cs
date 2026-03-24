using System;
using System.Collections.Generic;
using RPGDemo.Attributes;
using RPGDemo.Core.Strategies;
using RPGDemo.Stats;
using UnityEngine;

namespace RPGDemo.Buffs
{
    /// <summary>
    /// 运行时buff实例类
    /// </summary>
    [System.Serializable]
    public class BuffInstance
    {
        public BuffSO data;
        [SerializeReference] List<EffectStrategy> effects = new();
        [HideInInspector] public float remainingTime;
        [HideInInspector] public int currentStack = 1; //buff层数
        [HideInInspector] public Character owner; // 施加者（可用于归属伤害）
        [HideInInspector] public Character target;
        [HideInInspector] public float nextTickTime;        // 下次 tick 时间戳

        public void ApplyEffects()
        {
            foreach (var effect in effects)
            {
                effect.Apply(owner, target, data.buffID);
            }
        }

        public void Setup(Character caster, Character target)
        {
            owner = caster;
            this.target = target;
        }
    }
}
