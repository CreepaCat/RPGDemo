using System;
using UnityEngine;

public class PlayerAnimationHandler : AnimationHandler
{
    //对CC使用根动画位移时，不需要实际开启根动画，只需要将计算结果应用到CC即可

    private Player _player;
    // public bool IsInteracting => _animator.GetBool(PlayerAnimatorParamConfig.animIDIsinteracting);
    public bool IsHandInteracting => _animator.GetBool(PlayerAnimatorParamConfig.animIDIsHandInteracting);

    public bool CanDoCombo => _animator.GetBool(PlayerAnimatorParamConfig.animIDCanDoCombo);

    public bool IsCombat => _animator.GetBool(PlayerAnimatorParamConfig.animaIDIsCombat);
    //public bool UsingRootMotion => _animator.GetBool("usingRootMotion");

    public bool IsRolling => _animator.GetBool(PlayerAnimatorParamConfig.animIDIsRolling);
    public bool IsCasting => _animator.GetBool("isCasting");

    public void UpdateCanDoCombo(bool canDoComboFlag)
    {
        _animator.SetBool(PlayerAnimatorParamConfig.animIDCanDoCombo, canDoComboFlag);
    }

    protected override void Awake()
    {
        base.Awake();

        _player = GetComponent<Player>();
        _animator.applyRootMotion = false;
    }

    // public bool PlayTargetAnimation(int targetAnimation, bool isInteractingAnima, float crossFadeTime = 0.2f, bool usingRootMotion = false)
    // {

    //     if (IsInteracting) return false;

    //     _animator.SetBool(PlayerAnimatorParamConfig.animIDIsinteracting, isInteractingAnima);
    //     _animator.SetBool("usingRootMotion", usingRootMotion);
    //     _animator.Play(targetAnimation);
    //     _animator.CrossFade(targetAnimation, crossFadeTime);
    //     return true;
    // }

    /// <summary>
    /// 手部遮罩动画
    /// </summary>
    /// <param name="targetAnimation"></param>
    /// <param name="isHandInteractingAnima"></param>
    /// <param name="crossFadeTime"></param>
    public bool PlayTargetHandMaskedAnimation(int targetAnimation, bool isHandInteractingAnima, float crossFadeTime = 0.1f)
    {
        if (IsHandInteracting || IsInteracting) return false;

        _animator.SetBool(PlayerAnimatorParamConfig.animIDIsHandInteracting, isHandInteractingAnima);
        _animator.Play(targetAnimation);
        _animator.CrossFade(targetAnimation, crossFadeTime);
        return true;
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
        _player.Move(deltaPosition, _animator.deltaRotation);
    }

}
