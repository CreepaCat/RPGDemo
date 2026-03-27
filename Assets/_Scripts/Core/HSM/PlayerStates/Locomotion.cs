using HSM;
using UnityEngine;

public class Locomotion : State
{
    readonly Player _player;
    public Locomotion(HSM.StateMachine stateMachine, State parent, Player player) : base(stateMachine, parent)
    {
        _player = player;
    }

    protected override State GetInitialState() => null;

    protected override void OnEnter()
    {
        _player.Locomotion.CanMove = true;
    }


}
