using UnityEngine;

namespace RPGDemo.Core.Strategies
{
    /// <summary>
    /// 投射物策略
    /// </summary>
    public interface IProjectileStrategy
    {


        /// <summary>
        /// 可选：是否需要持续更新（追踪类需要，纯直线/抛物线可 false）
        /// </summary>
        bool RequiresUpdate { get; }

        /// <summary>
        /// 在投射物生成后调用，初始化并开始驱动移动
        /// </summary>
        void Initialize(GameObject projectile, Character caster, Character target,
                                    Vector3 launchPosition, Vector3 launchDirection);


        void UpdateProjectile(Rigidbody rb, Transform target, float deltaTime);

    }
}
