using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationHandler : MonoBehaviour
{
    protected Animator _animator;

    public Animator Animator => _animator;
    public bool IsInteracting => _animator.GetBool("isInteracting");
    public bool UsingRootMotion => _animator.GetBool("usingRootMotion");

    protected virtual void Awake()
    {
        _animator = GetComponent<Animator>();
    }



}
