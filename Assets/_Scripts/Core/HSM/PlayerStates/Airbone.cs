using HSM;
using UnityEngine;

public class Airbone : State
{
    readonly Player _player;
    public readonly Air_Jumping Jumping;
    public readonly Air_Falling Falling;

    const float LandingVerticalSpeed = -1.9f;

    public Airbone(HSM.StateMachine stateMachine, State parent, Player player) : base(stateMachine, parent)
    {
        _player = player;
        Jumping = new(stateMachine, this, player);
        Falling = new(stateMachine, this, player);
    }

    protected override State GetInitialState()
    {
        if (_player.Locomotion.JumpPerformed)
        {
            return Jumping;
        }
        if (!_player.Locomotion.Grounded && _player.Locomotion.VerticalVelocity < 0f)
        {
            return Falling;

        }
        return null;

    }


    protected override State GetTransition()
    {
        // 落地后退出空中父状态，返回地面父状态。
        return null;


    }

    protected override void OnEnter()
    {
        Debug.Log("OnEnter Airbone State");
    }

    protected override void OnExit()
    {
        Debug.Log("OnExit Airbone State");
        _player.Animator.SetBool(PlayerAnimatorParamConfig.animIDIsFalling, false);

    }

    protected override void OnUpdate(float deltaTime)
    {

        // 空中公共运动逻辑在父状态统一处理，子状态只处理阶段行为和切换。
        _player.Locomotion.CaculateFalling();
    }
}
