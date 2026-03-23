using System.Collections.Generic;
using RPGDemo.Core.Strategies;
using UnityEngine;

namespace RPGDemo.Inventories.ActionBar
{
    [CreateAssetMenu(fileName = "New PotionItem", menuName = "RPGDemo/Inventory/PotionItem")]
    public class PotionItem : ActionItem
    {

        public float useValue = 0;
        [Header("=== 药瓶使用策略 ===")]
        // [SerializeField] public SingleTargetStrategy targetStrategy;      // 单目标
        [SerializeField] public List<EffectStrategy> effectStrategies = new(); // 可多效果（伤害+Buff+治疗）
        [SerializeField] public CastRequirementStrategy requirementStrategy; // 蓝量/血量百分比要求
        [SerializeField] public PotionUsingVisualStrategy visualStrategy;   // 粒子+音效+动画

        /// <summary>玩家在快捷栏按下时调用</summary>
        public override bool Use(GameObject instigator)
        {
            Character caster = instigator.GetComponent<Character>();
            // 1. 检查释放条件
            if (requirementStrategy != null && !requirementStrategy.CanCast(caster))
                return false;

            // 2. 消耗资源
            requirementStrategy?.Consume(caster);

            // 3. 获取有效目标
            var targets = new List<Character>() { caster };  //药瓶只对自己释放

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

            return true;

        }

    }
}
