using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using RPGDemo.Stats;
using Unity.VisualScripting;


namespace RPGDemo.Buffs
{
    /// <summary>
    /// 管理每个角色身上的Buff
    /// </summary>
    public class BuffStore : MonoBehaviour, IStatModifierProvider
    {

        [SerializeField] BuffSO[] defaultBuffs = null;

        private void Start()
        {
            foreach (var buffSO in defaultBuffs)
            {
                ApplyBuff(buffSO, GetComponent<Character>(), GetComponent<Character>(), buffSO.baseDuration);
            }
        }
        private List<BuffInstance> activeBuffs = new List<BuffInstance>();

        public void ApplyBuff(BuffSO data, Character caster, Character target, float duration = -1)
        {
            // 处理叠加逻辑...
            BuffInstance instance = new BuffInstance { data = data, owner = caster, target = target };
            //若持续时间小于零 说明是永久buff
            instance.remainingTime = duration > 0 ? duration : float.PositiveInfinity;
            Debug.Log($"角色{target}获得Buff{data.buffName},持续时间{duration}");

            activeBuffs.Add(instance);
            data.OnApply(target, instance);
            //打上时间戳,获得Buff时立即tick一次
            instance.nextTickTime = Time.time;

            // 启动协程计时（永久 Buff duration=-1 不会结束）
            if (duration > 0) StartCoroutine(RemoveAfterTime(instance, duration));
        }

        //用协程管理buff的到期移除
        private IEnumerator RemoveAfterTime(BuffInstance instance, float time)
        {
            yield return new WaitForSeconds(time);
            RemoveBuff(instance);
        }

        public void RemoveBuff(BuffInstance instance)
        {
            instance.data.OnRemove(instance.target, instance);
            activeBuffs.Remove(instance);
        }

        // 每帧/每秒更新 Tick 与时间
        private void Update()
        {
            var now = Time.time;
            //计算移除
            for (int i = activeBuffs.Count - 1; i >= 0; i--)
            {
                var b = activeBuffs[i];
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
                if (b.data.tickInterval > 0f && b.nextTickTime < now)
                {
                    b.nextTickTime = now + b.data.tickInterval;
                    ApplyTickEffect(b); //应用buff效果
                    b.data.OnTick(b.target, b);


                }
            }

        }

        private void ApplyTickEffect(BuffInstance b)
        {
            //通过数值系统来判断造成的伤害或者治疗
            b.ApplyEffects();

            // 这里调用你的伤害系统（推荐使用独立 DamageSystem）
            // 例：b.target.TakeDamage(Mathf.Abs(b.data.tickDamageOrHeal) * b.currentStack, b.owner);
            // 或 b.target.Heal(...)
        }

        #region Buff数值
        //一些对角色固定属性修改的数值
        public IEnumerable<float> GetAdditiveModifiers(StatsType statsType)
        {
            foreach (var buffInstance in activeBuffs)
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
            foreach (var buffInstance in activeBuffs)
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
