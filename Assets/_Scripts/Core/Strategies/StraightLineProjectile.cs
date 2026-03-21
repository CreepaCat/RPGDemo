using UnityEngine;

namespace RPGDemo.Core.Strategies
{
    /// <summary>
    /// 直线投射物策略
    /// </summary>
    [CreateAssetMenu(menuName = "RPGDemo/Strategy/Projectile/StraightLineProjectile")]
    public class StraightLineProjectile : ProjectileStrategy
    {
        public float speed = 20f;
        // public bool usePhysics = true;  // true 用 Rigidbody.velocity，false 用 transform.position

        public override void InitializeAndLaunch(GameObject proj, Character caster, Character primaryTarget, Vector3 launchPos, Vector3 launchDir)
        {
            proj.transform.position = launchPos;
            proj.transform.rotation = Quaternion.LookRotation(launchDir);

            if (!RequiresUpdate)
            {
                if (proj.TryGetComponent<Rigidbody>(out var rb))
                {
                    rb.linearVelocity = launchDir.normalized * speed;
                }
            }
            else
            {
                // 非物理：手动移动（在 ProjectileController Update 中实现）
                // 或这里添加一个协程 MoveStraight()
            }
        }

    }
}
