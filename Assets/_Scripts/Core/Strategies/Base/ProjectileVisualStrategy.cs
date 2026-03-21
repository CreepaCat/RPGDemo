using System.Collections.Generic;
using UnityEngine;

namespace RPGDemo.Core.Strategies
{
    /// <summary>
    /// 投射物特效策略
    /// </summary>
    public abstract class ProjectileVisualStrategy : VisualStrategy, IProjectileVisualStrategy
    {
        [field: SerializeField] public GameObject projectilePrefab { get; private set; }

        [field: SerializeField] public ProjectileStrategy projectileStrategy { get; private set; }
    }
}
