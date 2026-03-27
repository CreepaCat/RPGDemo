using UnityEngine;

public static class PlayerAnimatorParamConfig
{

    //PARAM
    public static readonly int animIDSpeed = Animator.StringToHash("Speed");
    public static readonly int animIDGrounded = Animator.StringToHash("Grounded");
    public static readonly int animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    public static readonly int animIDAttack = Animator.StringToHash("Attack");


    public static readonly int animIDIsFalling = Animator.StringToHash("isFalling");
    public static readonly int animIDIsinteracting = Animator.StringToHash("isInteracting");
    public static readonly int animIDIsHandInteracting = Animator.StringToHash("isHandInteracting");
    public static readonly int animaIDIsCombat = Animator.StringToHash("isCombat");
    public static readonly int animIDCanDoCombo = Animator.StringToHash("canDoCombo");
    public static readonly int animIDIsRolling = Animator.StringToHash("isRolling");

    //CLIPS
    public static readonly int clipIDJumping = Animator.StringToHash("Jump");
    public static readonly int clipIDRolling = Animator.StringToHash("Rolling");
    public static readonly int clipIDFreeFall = Animator.StringToHash("FreeFall");
    public static readonly int clipIDUnsheath = Animator.StringToHash("Unsheath");
    public static readonly int clipIDSheath = Animator.StringToHash("Sheath");
}
