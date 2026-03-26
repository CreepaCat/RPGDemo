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
        [SerializeReference] public List<EffectStrategy> effects = new();
        [HideInInspector] public float remainingTime;
        [HideInInspector] public int currentStack = 1; //buff层数
        [HideInInspector] public Character owner; // 施加者（可用于归属伤害）
        [HideInInspector] public Character target;
        [HideInInspector] public float nextTickTime;        // 下次 tick 时间戳

        public BuffInstance(BuffSO data, Character caster, Character target, List<EffectStrategy> effects)
        {
            this.data = data;
            this.owner = caster;
            this.target = target;
            this.effects = effects;

        }

        public void ApplyEffects()
        {
            foreach (var effect in effects)
            {

                for (int i = 0; i < currentStack; i++)
                {
                    effect.Apply(owner, target, data.buffID);
                }

            }
        }

        internal float GetRemainingTimeRatio()
        {
            if (data.baseDuration < 0)
                return 1f;

            return remainingTime / data.baseDuration;
        }
    }
}
