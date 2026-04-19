using HSM;

public class PlayerRoot : State
{
    public readonly Ground Ground; //最上层状态
    public readonly Airbone Airbone;
    public readonly Rolling Rolling;

    public readonly Death Death;


    readonly Player _player;

    public PlayerRoot(HSM.StateMachine stateMachine, Player player) : base(stateMachine, null)
    {
        _player = player;
        Ground = new(stateMachine, this, player);
        Airbone = new(stateMachine, this, player);
        Rolling = new(stateMachine, this, player);
        Death = new(stateMachine, this, player);


    }

    protected override State GetInitialState()
    {
        if (_player.Health.IsDead())
        {
            return Death;
        }
        if (_player.Locomotion.RollPerformed)
        {
            return Rolling;
        }
        if (_player.Locomotion.JumpPerformed || !_player.Locomotion.Grounded)
        {
            return Airbone;
        }
        return Ground;

    }

    protected override State GetTransition()
    {

        if (_player.Health.IsDead())
        {
            return Death;
        }
        if (_player.Locomotion.RollPerformed)
        {
            return Rolling;
        }
        if (_player.Locomotion.JumpPerformed || !_player.Locomotion.Grounded)
        {
            return Airbone;
        }
        return Ground;

    }

}
