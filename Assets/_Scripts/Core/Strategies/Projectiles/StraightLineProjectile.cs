using UnityEngine;

namespace RPGDemo.Core.Strategies
{
    /// <summary>
    /// 直线投射物策略
    /// </summary>
   // [CreateAssetMenu(menuName = "RPGDemo/Strategy/Projectile/StraightLine")]
    public class StraightLineProjectile : ProjectileStrategy
    {
        public float speed = 20f;
        public new bool RequiresUpdate { get; private set; } = false;

        public override void Initialize(GameObject projectile, Character caster, Character target, Vector3 launchPosition, Vector3 launchDirection)
        {
            var rb = projectile.GetComponent<Rigidbody>();
            if (rb)
                rb.linearVelocity = rb.transform.forward * speed;
        }

        //直线发射无需计算
        public override void UpdateProjectile(Rigidbody rb, Transform target, float deltaTime)
        {
            rb.MovePosition(rb.transform.position + speed * rb.transform.forward * deltaTime);


        }



    }
}
