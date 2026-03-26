using UnityEngine;

namespace RPGDemo.Core.Strategies
{
    /// <summary>
    /// 投射物策略
    /// </summary>
    [System.Serializable]
    public abstract class ProjectileStrategy : IProjectileStrategy
    {
        //是否手动计算投射物路径
        public bool RequiresUpdate = true;

        public abstract void Initialize(GameObject projectile, Character caster, Character target, Vector3 launchPosition, Vector3 launchDirection);


        public abstract void UpdateProjectile(Rigidbody rb, Transform target, float deltaTime);




    }
}
