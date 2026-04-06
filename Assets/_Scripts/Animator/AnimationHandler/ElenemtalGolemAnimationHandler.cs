using UnityEngine;

public class ElenemtalGolemAnimationHandler : AnimationHandler
{
    private Enemy _enemy;

    protected override void Awake()
    {
        base.Awake();
        _enemy = GetComponent<Enemy>();
    }

    /// <summary>
    ///计算并应用动画位移和旋转，此函数在LateUpdate执行
    /// </summary>
    private void OnAnimatorMove()
    {
        //非Interacting动画不使用根动画计算位移
        if (!IsInteracting || Time.deltaTime <= 0f) return;

        //由于Fall和Jump都属于Interacting动画，但不由根动画计算位移，所以排除
        if (!UsingRootMotion) return;

        Vector3 deltaPosition = _animator.deltaPosition;
        deltaPosition.y = 0;
        _enemy.Move(deltaPosition, _animator.deltaRotation);
    }
}
