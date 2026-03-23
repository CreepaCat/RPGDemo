using RPGDemo.Core.Strategies;
using UnityEngine;

/// <summary>
/// 追踪弹
/// </summary>
[CreateAssetMenu(menuName = "RPGDemo/Strategy/Projectile/Homing")]
public class HomingProjectile : ProjectileStrategy
{
    public float speed = 12f;
    public float turnSpeed = 180f;         // 度/秒，转向灵敏度
    public float maxLifetime = 8f;
    public float closeEnoughDistance = 0.5f;

    //这是策略SO,运行时可能存在共用，因此不能直接作为运行时数据,不能使用缓存对象来计算曲线
    public override void InitializeAndLaunch(GameObject proj, Character caster, Character primaryTarget, Vector3 launchPos, Vector3 launchDir)
    {
        proj.transform.position = launchPos;
        proj.transform.forward = launchDir;


        if (primaryTarget == null)
        {
            // 无目标时 fallback 直线
            if (proj.TryGetComponent<Rigidbody>(out var rb))
                rb.linearVelocity = launchDir * speed;
        }
    }

    // 在 ProjectileController Update 中调用
    public void UpdateHoming(Transform projTransform, Transform targetTransform, float deltaTime)
    {
        if (targetTransform == null) return;

        //不要到目标脚下
        //todo:获取目标模型的中点
        var targetPos = targetTransform.position + Vector3.up;

        Vector3 toTarget = (targetPos - projTransform.position).normalized;
        Vector3 currentDir = projTransform.forward;

        // 平滑转向
        Vector3 newDir = Vector3.RotateTowards(currentDir, toTarget, turnSpeed * Mathf.Deg2Rad * deltaTime, 0f);
        projTransform.forward = newDir;



        var rb = projTransform.GetComponent<Rigidbody>();
        // 或者用物理（更稳定，粒子尾迹完美）
        if (rb)
        {

            rb.linearVelocity = newDir * speed;
        }
        else
        {
            projTransform.position += newDir * speed * deltaTime;
        }

        // 接近目标时可加速或直接命中
        if (Vector3.Distance(projTransform.position, targetPos) < closeEnoughDistance)
        {
            // 可直接销毁或触发爆炸
            Debug.Log("HomingProjectile命中" + targetTransform);
        }
    }
}
