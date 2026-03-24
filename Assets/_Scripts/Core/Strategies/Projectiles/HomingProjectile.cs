using RPGDemo.Core.Strategies;
using UnityEngine;

/// <summary>
/// 追踪弹
/// </summary>
//[CreateAssetMenu(menuName = "RPGDemo/Strategy/Projectile/Homing")]
public class HomingProjectile : ProjectileStrategy
{
    public float speed = 12f;
    public float turnSpeed = 180f;         // 度/秒，转向灵敏度
    public float maxLifetime = 8f;
    public float closeEnoughDistance = 0.5f;

    public override void Initialize(GameObject projectile, Character caster, Character target, Vector3 launchPosition, Vector3 launchDirection)
    {
        //no op;
        if (target == null)
        {
            //projectile.GetComponent<Rigidbody>().linearVelocity = speed *
            Destroy(projectile);

        }
    }

    void Destroy(GameObject gameObject)
    {
        gameObject.SetActive(false);
    }

    // 在 ProjectileController Update 中调用
    public override void UpdateProjectile(Rigidbody rb, Transform targetTransform, float deltaTime)
    {
        // Transform projTransform = rb.transform;
        // if (targetTransform == null) return;

        // //todo:获取目标模型的中点
        // var targetPos = targetTransform.position + Vector3.up;

        // Vector3 toTarget = (targetPos - projTransform.position).normalized;
        // Vector3 currentDir = projTransform.forward;

        // // 平滑转向
        // Vector3 newDir = Vector3.RotateTowards(currentDir, toTarget, turnSpeed * Mathf.Deg2Rad * deltaTime, 0f);
        // projTransform.forward = newDir;

        // rb.linearVelocity = newDir * speed;

        // // 接近目标时可加速或直接命中
        // if (Vector3.Distance(projTransform.position, targetPos) < closeEnoughDistance)
        // {
        //     //speed += 0.2f;
        //     // 可直接销毁或触发爆炸
        //     Debug.Log("HomingProjectile命中" + targetTransform);
        // }
        if (targetTransform == null) return;

        // todo: 获取目标模型的中点（可以再优化为实际的 collider center）
        Vector3 targetPos = targetTransform.position + Vector3.up;

        Vector3 toTarget = (targetPos - rb.position).normalized;   // 用 rb.position 更准确
        Vector3 currentDir = rb.rotation * Vector3.forward;        // 当前朝向（推荐用 rigidbody 的 rotation）

        // 平滑转向（推荐改用 Quaternion.RotateTowards，更平滑且不会抖动）
        Quaternion targetRot = Quaternion.LookRotation(toTarget);
        Quaternion newRot = Quaternion.RotateTowards(rb.rotation, targetRot, turnSpeed * Mathf.Deg2Rad * Time.fixedDeltaTime);

        // 计算下一帧应该移动到的位置（保持恒定速度）
        Vector3 moveDirection = newRot * Vector3.forward;           // 用新的朝向计算前进方向
        Vector3 nextPosition = rb.position + moveDirection * speed * Time.fixedDeltaTime;

        // 执行移动和旋转（必须在 FixedUpdate 中调用）
        rb.MoveRotation(newRot);
        rb.MovePosition(nextPosition);

        // 接近目标时处理命中
        if (Vector3.Distance(rb.position, targetPos) < closeEnoughDistance)
        {
            Debug.Log("HomingProjectile命中 " + targetTransform.name);
            // 这里可以触发爆炸、销毁等
            // Destroy(gameObject); 或 rb.isKinematic = true; 等
        }
    }
}
