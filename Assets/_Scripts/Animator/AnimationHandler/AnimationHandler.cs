using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationHandler : MonoBehaviour
{
    protected Animator _animator;

    public Animator Animator => _animator;
    public bool IsInteracting => _animator.GetBool("isInteracting");
    public bool UsingRootMotion => _animator.GetBool("usingRootMotion");

    public bool CanMoveOrRotate() => !IsOnAnimaMove();

    public bool IsOnAnimaMove() => IsInteracting && UsingRootMotion;

    protected virtual void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public virtual bool PlayTargetAnimation(int targetAnimation, bool isInteractingAnima, bool usingRootMotion = false, float crossFadeTime = 0.2f)
    {

        if (IsInteracting) return false;

        _animator.SetBool(PlayerAnimatorParamConfig.animIDIsinteracting, isInteractingAnima);
        _animator.SetBool("usingRootMotion", usingRootMotion);
        _animator.Play(targetAnimation);
        _animator.CrossFade(targetAnimation, crossFadeTime);
        return true;
    }

    public virtual bool PlayInterruptAnimation(int targetAnimation, bool isInteractingAnima, bool usingRootMotion = false, float crossFadeTime = 0.2f)
    {
        _animator.SetBool(PlayerAnimatorParamConfig.animIDIsinteracting, isInteractingAnima);
        _animator.SetBool("usingRootMotion", usingRootMotion);
        _animator.Play(targetAnimation);
        _animator.CrossFade(targetAnimation, crossFadeTime);
        return true;
    }



}
