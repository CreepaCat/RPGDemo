using HSM;
using UnityEngine;

public class Air_Jumping : State
{
    readonly Player _player;

    public Air_Jumping(HSM.StateMachine stateMachine, State parent, Player player) : base(stateMachine, parent)
    {
        _player = player;
    }

    protected override State GetInitialState() => null;

    protected override State GetTransition()
    {
        // 上升结束后切换到下落阶段。
        if (_player.Locomotion.VerticalVelocity <= 0f)
        {
            return ((Airbone)Parent).Falling;
        }
        return null;
    }

    protected override void OnEnter()
    {
        Debug.Log("Jumping");
        _player.AnimatorHandler.PlayTargetAnimation(PlayerAnimatorParamConfig.clipIDJumping, true, 0.04f, false);
        _player.Animator.SetBool(PlayerAnimatorParamConfig.animIDIsFalling, false);
        _player.Locomotion.SetJump();

    }

    protected override void OnExit()
    {
        Debug.Log("OnExit Jumping State");
    }

    protected override void OnUpdate(float deltaTime)
    {
        // 跳跃阶段不额外处理，空中速度由 Airbone 父状态统一计算。
    }
}
