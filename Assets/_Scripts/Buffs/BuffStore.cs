using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using RPGDemo.Stats;
using Unity.VisualScripting;
using System.Linq;
using System;


namespace RPGDemo.Buffs
{
    /// <summary>
    /// 管理每个角色身上的Buff
    /// </summary>
    public class BuffStore : MonoBehaviour, IStatModifierProvider
    {

        /// <summary>
        /// 使用运行时实例来添加buff(数据驱动)
        /// </summary>
        [SerializeField] BuffInstance[] defaultBuffs = null;
        //todo:改用Hash表
        private Dictionary<BuffInstance, Coroutine> activeBuffs = new();
        public event Action OnStoreUpdated;

        private void Start()
        {
            foreach (var buffInstance in defaultBuffs)
            {
                if (buffInstance == null || buffInstance.data == null) return;
                ApplyBuff(buffInstance, GetComponent<Character>(), GetComponent<Character>(), buffInstance.data.baseDuration);
            }
        }



        public IEnumerable<BuffInstance> GetActiveBuffs()
        {
            foreach (var buffInstance in activeBuffs.Keys.ToList())
            {
                yield return buffInstance;
            }
        }


        bool HasBuff(BuffSO buffSO)
        {
            return GetInstance(buffSO) != null;
        }

        bool HasBuffInstance(BuffInstance buffInstance)
        {
            return activeBuffs.ContainsKey(buffInstance);
        }

        public BuffInstance GetInstance(BuffSO buffSO)
        {
            BuffInstance instance = null;
            foreach (var buffInstance in activeBuffs.Keys)
            {
                if (buffInstance.data == buffSO)
                {
                    return instance = buffInstance;
                }
            }
            return instance;
        }

        public void ApplyBuff(BuffInstance instance, Character caster, Character target, float duration = -1)
        {
            //必须new一个对象，防止共用一个引用对象造成数据干扰
            BuffInstance newInstance = new BuffInstance(instance.data, caster, target, instance.effects);

            if (HasBuff(instance.data)) //如果身上已有该buff
            {
                newInstance = GetInstance(newInstance.data);
                //处理叠加逻辑,只增加叠层，不直接触发层数效果，不然数值很爆炸
                if (newInstance.data.isStackable)
                {
                    newInstance.currentStack++;
                    newInstance.currentStack = Mathf.Clamp(newInstance.currentStack, newInstance.currentStack, newInstance.data.maxStack);
                    Debug.Log($"角色{target.name}的{newInstance.data.buffName}buff叠加,当前层数{newInstance.currentStack}");
                }
                //关闭当前buff移除协程，停止buff移除协程并开启新携程
                if (activeBuffs[newInstance] != null)
                {
                    StopCoroutine(activeBuffs[newInstance]);
                    Coroutine newCoroutine = StartCoroutine(RemoveAfterTime(newInstance, duration));
                    activeBuffs[newInstance] = newCoroutine;
                }
                //刷新持续时间
                newInstance.remainingTime = duration > 0 ? duration : float.PositiveInfinity;
                // newInstance.data.OnApply(target, newInstance);
                return;
            }


            //刷新持续时间并立即Tick一次buff
            newInstance.remainingTime = duration > 0 ? duration : float.PositiveInfinity;
            newInstance.data.OnApply(target, newInstance);

            newInstance.nextTickTime = Time.time;
            Coroutine buffRemovingCorountine = null;
            if (duration > 0)
            {
                buffRemovingCorountine = StartCoroutine(RemoveAfterTime(newInstance, duration));
            }
            activeBuffs[newInstance] = buffRemovingCorountine;
            Debug.Log($"角色{target}获得Buff{newInstance.data.buffName},持续时间{duration}");
            OnStoreUpdated?.Invoke();
        }

        //用协程管理buff的到期移除
        private IEnumerator RemoveAfterTime(BuffInstance instance, float time)
        {
            yield return new WaitForSeconds(time);
            RemoveBuff(instance);
        }

        public void RemoveBuff(BuffInstance instance)
        {
            if (activeBuffs.ContainsKey(instance) && activeBuffs[instance] != null)
                StopCoroutine(activeBuffs[instance]);
            instance.data.OnRemove(instance.target, instance);
            activeBuffs.Remove(instance);
            OnStoreUpdated?.Invoke();
        }

        // 每帧/每秒更新 Tick 与时间
        private void Update()
        {
            var now = Time.time;

            var activeBuffsArray = activeBuffs.Keys.ToArray();
            //计算移除
            for (int i = activeBuffsArray.Length - 1; i >= 0; i--)
            {
                var b = activeBuffsArray[i];
                //时间结束移除Buff
                if (b.remainingTime < float.PositiveInfinity)
                {
                    b.remainingTime -= Time.deltaTime;
                    if (b.remainingTime <= 0)
                    {
                        RemoveBuff(b);
                        continue;
                    }
                }

                //周期Tick
                if (b.data.tickInterval > 0f && now > b.nextTickTime)
                {
                    b.nextTickTime = now + b.data.tickInterval;
                    ApplyTickEffect(b); //应用buff效果
                                        //b.data.OnTick(b.target, b);


                }
            }

        }

        /// <summary>
        /// buff实例的伤害或治疗等效果，使用Effect策略
        /// </summary>
        /// <param name="b"></param>
        private void ApplyTickEffect(BuffInstance b)
        {
            Debug.Log("tick buff" + b.data.buffName);
            //通过数值系统来判断造成的伤害或者治疗
            b.ApplyEffects();
        }

        #region Buff对角色属性的影响数值
        //一些对角色固定属性修改的数值
        public IEnumerable<float> GetAdditiveModifiers(StatsType statsType)
        {
            var activeBuffsArray = activeBuffs.Keys.ToArray();
            foreach (var buffInstance in activeBuffsArray)
            {
                //如果是造成伤害，直接调用takedamage,否则直接修改数值
                foreach (var statBonus in buffInstance.data.additiveBonuses)
                {
                    if (statBonus.statsType == statsType)
                        yield return statBonus.value;
                }
            }
        }

        public IEnumerable<float> GetPercentageModifiers(StatsType statsType)
        {
            var activeBuffsArray = activeBuffs.Keys.ToArray();
            foreach (var buffInstance in activeBuffsArray)
            {
                //如果是造成伤害，直接调用takedamage,否则直接修改数值
                foreach (var statBonus in buffInstance.data.percentageBonuses)
                {
                    if (statBonus.statsType == statsType)
                        yield return statBonus.value;
                }
            }
        }


        #endregion
    }
}
