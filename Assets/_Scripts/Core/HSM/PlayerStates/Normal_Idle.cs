using HSM;
using UnityEngine;

public class Normal_Idle : State
{
    readonly Player _player;
    public Normal_Idle(HSM.StateMachine stateMachine, State parent, Player player) : base(stateMachine, parent)
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
        _player.Locomotion.CanMove = true;
    }





}
