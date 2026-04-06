using HSM;
using UnityEngine;

public class Air_Falling : State
{
    readonly Player _player;

    public Air_Falling(HSM.StateMachine stateMachine, State parent, Player player) : base(stateMachine, parent)
    {
        _player = player;
    }

    protected override State GetInitialState() => null;

    protected override State GetTransition()
    {
        return null;
    }

    protected override void OnEnter()
    {
        if (!_player.Animator.GetBool(PlayerAnimatorParamConfig.animIDIsFalling))
        {
            _player.AnimationHandler.PlayTargetAnimation(PlayerAnimatorParamConfig.clipIDFreeFall, true, false, 0.04f);
        }
        _player.Animator.SetBool(PlayerAnimatorParamConfig.animIDIsFalling, true);
    }
    protected override void OnExit()
    {
        Debug.Log("OnExit Falling State");
        _player.Animator.SetBool(PlayerAnimatorParamConfig.animIDIsFalling, false);
    }

    protected override void OnUpdate(float deltaTime)
    {
        // 下落阶段动画在进入时设置，具体速度由 Airbone 父状态统一驱动。
        if (_player.Locomotion.Grounded && _player.Locomotion.JumpPerformed)
        {
            _player.Locomotion.JumpPerformed = false;
        }
    }
}
