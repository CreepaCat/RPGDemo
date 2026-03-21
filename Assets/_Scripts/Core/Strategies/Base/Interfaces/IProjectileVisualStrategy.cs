using UnityEngine;

namespace RPGDemo.Core.Strategies
{
    /// <summary>
    /// 投射物特效策略
    /// </summary>
    public interface IProjectileVisualStrategy : IVisualStrategy
    {
        GameObject projectilePrefab { get; }//投射物预制体
        ProjectileStrategy projectileStrategy { get; }//投射物策略(移动方式等)

    }
}
