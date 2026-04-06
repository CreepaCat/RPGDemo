


using HSM;

public class Death : State
{
    readonly Player _player;
    public Death(HSM.StateMachine stateMachine, State parent, Player player) : base(stateMachine, parent)
    {
        _player = player;

    }

    protected override void OnEnter()
    {
        //base.OnEnter();
        _player.Animator.Play("Death");
    }
}
