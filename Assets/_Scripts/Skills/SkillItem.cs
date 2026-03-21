using System.Collections.Generic;
using RPGDemo.Attributes;
using RPGDemo.Core.Strategies;
using RPGDemo.Inventories.ActionBar;
using UnityEngine;


namespace RPGDemo.Skills
{
    public enum SkillType
    {
        Active, //主动技能
        Passive,//被动技能
    }

    [CreateAssetMenu(fileName = "New Skill", menuName = "RPGDemo/Skills/New Skill Item")]
    public class SkillItem : ActionItem
    {
        //public float baseDamage = 1;//基础伤害
        // Active / Passive 主动技能与被动技能
        public SkillType skillType = SkillType.Active;
        [SerializeField]
        [Range(0, 9999)] public float damageMultiplier = 1;
        [field: SerializeField] public float manaCost { get; private set; } = 10;

        [field: SerializeField] public bool isHealingSkill { get; private set; } = false;

        [field: SerializeField] public float baseHeal { get; private set; } = 0f;

        [Header("解锁配置")]
        public int unlockCost = 1;                 // 消耗技能点
        public List<SkillItem> prerequisites;          // 前置技能（可为空）
        public bool isUnlockedByDefault = false;


        /// <summary>玩家解锁时调用</summary>
        public virtual void OnUnlock(Player player)
        {
            // 子类实现具体逻辑
        }

        [Header("=== 技能策略乐高积木模块 ===")]
        [SerializeField] public TargetStrategy targetStrategy;      // 单目标 / 多目标 / 自动锁定
        [SerializeField] public RangeStrategy rangeStrategy;                 // 最远距离 + AoE 形状
        [SerializeField] public List<EffectStrategy> effectStrategies = new(); // 可多效果（伤害+Buff+治疗）
        [SerializeField] public FilterStrategy filterStrategy;               // 敌/友/无视
        [SerializeField] public CastRequirementStrategy requirementStrategy; // 怒气/血量消耗(可为空)
        [SerializeField] public VisualStrategy visualStrategy;               // 粒子+音效+动画



        // [SerializeField] public ProjectileVisualStrategy projectileVisualStrategy;  //技能产生投射物策略

        /// <summary>玩家在快捷栏按下时调用</summary>
        public override bool Use(GameObject instigator)
        {
            Character caster = instigator.GetComponent<Character>();
            // 1. 检查释放条件
            if (requirementStrategy != null && !requirementStrategy.CanCast(caster))
                return false;

            // 2. 消耗资源,用于除法力值外的资源消耗策略
            requirementStrategy?.Consume(caster);

            //法力消耗
            if (manaCost > 0f)
            {
                if (!caster.GetComponent<Mana>().TryUseMana(manaCost))
                {
                    return false;
                }
            }




            // 3. 获取有效目标（范围 + 过滤）
            var targets = targetStrategy?.GetValidTargets(caster, rangeStrategy, filterStrategy);

            //判断是否是投射类技能, 如果是投射物类的技能，可以不用管目标是否为0
            ProjectileVisualStrategy projectileVisual = visualStrategy as ProjectileVisualStrategy;
            if (projectileVisual != null)
            {
                projectileVisual.Play(caster, targets, GetItemID());
                return true;
            }

            if (targets == null || targets.Count == 0) return false;

            // 4. 对每个目标依次应用所有效果
            foreach (var target in targets)
            {
                foreach (var effect in effectStrategies)
                {
                    if (effect != null)
                        effect.Apply(caster, target, GetItemID());
                }
            }

            // 5. 播放视觉表现
            visualStrategy?.Play(caster, targets, GetItemID());

            // 6. 触发冷却（由 SkillHotbar 或 Player 统一管理）
            //  caster.StartCooldown(this, cooldown);

            return true;

        }


    }
}
