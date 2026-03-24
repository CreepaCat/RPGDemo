using System;
using RPGDemo.Core.Strategies;
using UnityEngine;

[System.Serializable]
public class AcceleratingBezierProjectile : ProjectileStrategy
{
    [Header("贝塞尔曲线控制点偏移")]
    public Vector3 startControlOffset = new Vector3(0, 5f, 0);
    public Vector3 endControlOffset = new Vector3(0, 8f, 0);

    [Header("飞行参数")]
    public float duration = 2.8f;         // 总飞行时间（秒）
    public AnimationCurve speedCurve;            // 前慢后快的速度曲线

    [Header("其他")]
    public bool lookAtMovementDirection = true;
    public float maxSpeed = 35f;                 // 限制最大速度，防止过快
    [Range(1f, 25f)] public float rotationSpeed = 12f;   // 转向速度（关键参数）

    private Vector3 p0, p1, p2, p3;
    private float timer = 0f;
    private bool isLaunched = false;

    public new bool RequiresUpdate => true;

    public override void Initialize(GameObject projectile, Character caster, Character primaryTarget,
                                    Vector3 launchPosition, Vector3 launchDirection)
    {

        //默认曲线
        if (speedCurve == null || speedCurve.keys.Length == 0)
        {
            speedCurve = new AnimationCurve(
                new Keyframe(0.0f, 0.35f),   // 非常慢启动
                new Keyframe(0.4f, 0.8f),
                new Keyframe(0.75f, 2.4f),   // 开始明显加速
                new Keyframe(1.0f, 3.8f)    // 最后猛冲
            );
        }

        p0 = launchPosition;

        Vector3 targetPos = primaryTarget != null
            ? primaryTarget.transform.position
            : launchPosition + launchDirection * 25f;

        p3 = targetPos;
        p1 = p0 + startControlOffset;
        p2 = p3 + endControlOffset;

        timer = 0f;
        isLaunched = true;

        // 初始位置和朝向
        projectile.transform.position = p0;
        if (lookAtMovementDirection)
            projectile.transform.LookAt(CalculatePoint(0.05f));

    }

    /// <summary>
    /// 使用 linearVelocity 驱动
    /// </summary>
    public override void UpdateProjectile(Rigidbody rb, Transform targetTransform, float deltaTime)
    {
        if (!isLaunched) return;

        timer += deltaTime;
        float t = Mathf.Clamp01(timer / duration);

        // 应用速度曲线（慢启动 + 后段加速）
        float easedT = speedCurve.Evaluate(t);

        Vector3 newPosition = CalculatePoint(easedT);

        // 使用 Rigidbody 移动（保持你当前喜欢的物理方式）
        rb.MovePosition(newPosition);

        // 朝向运动方向
        if (lookAtMovementDirection && t < 0.99f)
        {
            Vector3 nextPos = CalculatePoint(easedT + 0.02f);
            Vector3 direction = (nextPos - newPosition).normalized;
            if (direction.sqrMagnitude > 0.001f)
                rb.MoveRotation(Quaternion.LookRotation(direction));
        }

        // 到达终点自动销毁
        if (t >= 1f)
        {
            Destroy(rb.gameObject);
        }
    }

    private void Destroy(GameObject gameObject)
    {
        gameObject.SetActive(false);
    }

    private Vector3 CalculatePoint(float t)
    {
        float u = 1f - t;
        return u * u * u * p0 +
               3 * u * u * t * p1 +
               3 * u * t * t * p2 +
               t * t * t * p3;
    }


}
