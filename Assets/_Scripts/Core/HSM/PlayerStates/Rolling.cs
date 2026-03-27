using HSM;

public class Rolling : State
{
    readonly Player _player;
    public Rolling(HSM.StateMachine stateMachine, State parent, Player player) : base(stateMachine, parent)
    {
        _player = player;
    }

    protected override State GetInitialState() => null;

    protected override State GetTransition()
    {

        return null; //返回

    }

    protected override void OnEnter()
    {
        _player.Locomotion.CanMove = false;
        _player.Animator.SetBool(PlayerAnimatorParamConfig.animIDIsRolling, true);
        _player.AnimatorHandler.PlayTargetAnimation(PlayerAnimatorParamConfig.clipIDRolling, true, 0.1f, true);
    }
    protected override void OnUpdate(float deltaTime)
    {
        if (!_player.AnimatorHandler.IsRolling && _player.Locomotion.RollPerformed)
        {
            _player.Locomotion.RollPerformed = false;
        }
    }





}
