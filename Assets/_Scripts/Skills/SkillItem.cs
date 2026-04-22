using System.Collections;
using System.Collections.Generic;
using RPGDemo.Attributes;
using RPGDemo.Core.Strategies;
using RPGDemo.Inventories.ActionBar;
using RPGDemo.Projectiles;
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
        public SkillType skillType = SkillType.Active;
        [field: SerializeField] public float manaCost { get; private set; } = 10;

        [Header("解锁配置")]
        public int unlockCost = 1;                 // 消耗技能点
        public List<SkillItem> prerequisites;          // 前置技能（可为空）
        public bool isUnlockedByDefault = false;


        /// <summary>玩家解锁时调用</summary>
        public virtual void OnUnlock(Player player)
        {
            // 子类实现具体逻辑
        }
        [Header("== 生成投射物 ==")]
        //todo:使用投射物工厂生产投射物
        [SerializeField] public ProjectileController projectilePrefab;  //生成投射物
        [SerializeReference] public ProjectileStrategy projectileStrategy; //投射物控制策略
        public Character.CastType castType = Character.CastType.RightHand;
        public Vector3 castOffset = Vector3.zero;

        [Header("=== 技能策略乐高积木模块 ===")]
        //todo:使用目标选择系统
        [SerializeReference] public TargetStrategy targetStrategy;      // 单目标 / 多目标 / 自动锁定
        [SerializeReference] public RangeStrategy rangeStrategy;                 // 最远距离 + AoE 形状
        [SerializeReference] public FilterStrategy filterStrategy;               // 敌/友/无视

        [SerializeReference] public AnimationStrategy animationStrategy;//播放动画

        //todo：效果拆分，分为应用在自身的效果 和 应用在目标身上的效果
        [SerializeReference] public List<EffectStrategy> effectStrategies = new(); // 可多效果（伤害+Buff+治疗）
        [SerializeReference] public CastRequirementStrategy requirementStrategy; // 怒气/血量消耗(可为空)

        [SerializeReference] public List<VisualStrategy> visualStrategies = new();      // 粒子+音效

        /// <summary>玩家在快捷栏按下时调用</summary>
        public override bool Use(GameObject instigator)
        {
            Character caster = instigator.GetComponent<Character>();
            // 1. 检查释放条件
            if (requirementStrategy != null && !requirementStrategy.CanCast(caster))
                return false;

            //若不能播放技能动画，则无法释放该技能
            if (animationStrategy != null && !animationStrategy.CanPlayAnimation(caster))
            {
                return false;
            }

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
            // 获取有效目标（范围 + 过滤）
            var targets = targetStrategy?.GetValidTargets(caster, rangeStrategy, filterStrategy);

            Character primary = null;
            if (targets != null && targets.Count > 0)
            {
                primary = targets[0];
            }

            Vector3 dir = primary != null ?
            (primary.transform.position - caster.transform.position).normalized
            : caster.transform.forward;


            //todo:禁用角色输入，并插值转向
            caster.transform.rotation = Quaternion.LookRotation(dir);
            if (caster.transform.TryGetComponent(out Rigidbody rb))
            {
                rb.MoveRotation(Quaternion.LookRotation(dir));
                rb.angularVelocity = Vector3.zero;
            }

            float delayTiem = animationStrategy == null ? 0f : animationStrategy.DelayTime;
            caster.StartCoroutine(CastSkillCroutine(delayTiem, caster, targets));
            return true;




        }

        IEnumerator CastSkillCroutine(float delayCastTime, Character caster, List<Character> targets)
        {

            animationStrategy?.PlayAnimation(caster);

            //释放者身上的粒子效果与音效
            PlayVisuals(caster, targets);
            yield return new WaitForSeconds(delayCastTime);
            //判断是否是投射类技能, 如果是投射物类的技能，可以不用管目标是否为0
            if (projectilePrefab != null)
            {
                ApplyProjectile(caster, targets);
                yield break;

            }
            // 若不是投射类技能，对每个目标依次应用所有效果
            ApllyEffectToTargets(caster, targets);


        }

        private void ApplyProjectile(Character caster, List<Character> targets)
        {
            var castPoint = caster.GetCastTransform(castType);
            var launchPos = castPoint.transform.position + castOffset;


            //是否是多目标，对每个目标单独生成投射物
            if (targets == null || targets.Count == 0)
            {
                SpawnProjectile(caster, null, launchPos);

            }
            else
            {
                //多目标
                foreach (var target in targets)
                {
                    SpawnProjectile(caster, target, launchPos);
                }
            }

        }
        /// <summary>
        /// 生成投射物
        /// </summary>
        /// <param name="caster"></param>
        /// <param name="launchPos"></param>
        private void SpawnProjectile(Character caster, Character target, Vector3 launchPos)
        {
            //如果没有目标，就直线发射
            Vector3 dir = target != null
            ? (target.transform.position + Vector3.up - launchPos).normalized
            : caster.transform.forward;

            ProjectileController projectileController = Instantiate(projectilePrefab, launchPos, Quaternion.LookRotation(dir));

            projectileController.Launch(projectileStrategy, caster, target, GetItemID(), launchPos, dir);

            //命中回调函数
            projectileController.SetCallback((hitTarget) =>
            {
                foreach (var effect in effectStrategies)
                {
                    if (effect != null)
                        effect.Apply(caster, hitTarget, GetItemID());
                }
            });
        }

        private void ApllyEffectToTargets(Character caster, List<Character> targets)
        {

            foreach (var target in targets)
            {
                foreach (var effect in effectStrategies)
                {
                    if (effect != null)
                        effect.Apply(caster, target, GetItemID());
                }
            }
        }

        private void PlayVisuals(Character caster, List<Character> targets)
        {
            if (visualStrategies == null || targets == null) return;
            foreach (var visual in visualStrategies)
            {
                visual?.Play(caster, targets, GetItemID());
            }
        }
    }
}
