using UnityEngine;

namespace RPGDemo.Core.Strategies
{
    /// <summary>
    /// 投射物策略
    /// </summary>
    public interface IProjectileStrategy
    {
        /// <summary>
        /// 在投射物生成后调用，初始化并开始驱动移动
        /// </summary>
        /// <param name="projectile">投射物 GameObject（通常有 Rigidbody 或自定义移动组件）</param>
        /// <param name="caster">施法者</param>
        /// <param name="primaryTarget">主要目标（单体技能用）或第一个目标</param>
        /// <param name="launchPosition">发射点（手、武器、法杖等）</param>
        /// <param name="launchDirection">初始朝向（如果需要）</param>
        void InitializeAndLaunch(
            GameObject projectile,
            Character caster,
            Character primaryTarget,
            Vector3 launchPosition,
            Vector3 launchDirection
        );

        /// <summary>
        /// 可选：是否需要持续更新（追踪类需要，纯直线/抛物线可 false）
        /// </summary>
        bool RequiresUpdate { get; }

    }
}
