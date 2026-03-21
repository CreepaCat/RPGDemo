using UnityEngine;

namespace RPGDemo.Core.Strategies
{
    /// <summary>
    /// 投射物策略
    /// </summary>
    public abstract class ProjectileStrategy : ScriptableObject, IProjectileStrategy
    {
        //是否手动计算投射物路径
        [field: SerializeField] public bool RequiresUpdate { get; private set; }

        /// <summary>
        /// 在投射物生成后调用，初始化并开始驱动移动
        /// </summary>
        /// <param name="projectile">投射物 GameObject（通常有 Rigidbody 或自定义移动组件）</param>
        /// <param name="caster">施法者</param>
        /// <param name="primaryTarget">主要目标（单体技能用）或第一个目标</param>
        /// <param name="launchPosition">发射点（手、武器、法杖等）</param>
        /// <param name="launchDirection">初始朝向（如果需要）</param>
        public abstract void InitializeAndLaunch(GameObject projectile, Character caster, Character primaryTarget,
                        Vector3 launchPosition, Vector3 launchDirection);

    }
}
