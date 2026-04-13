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
        _player.Health.isImmunity = true;
        _player.Animator.SetBool(PlayerAnimatorParamConfig.animIDIsRolling, true);
        _player.AnimationHandler.PlayTargetAnimation(PlayerAnimatorParamConfig.clipIDRolling, true, true, 0.1f);
    }
    protected override void OnExit()
    {
        _player.Health.isImmunity = false;
    }
    protected override void OnUpdate(float deltaTime)
    {
        if (!_player.AnimationHandler.IsRolling && _player.Locomotion.RollPerformed)
        {
            _player.Locomotion.RollPerformed = false;
        }
    }







}
