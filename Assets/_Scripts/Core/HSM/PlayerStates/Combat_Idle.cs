using HSM;
using UnityEngine;

public class Combat_Idle : State
{

    readonly Player _player;
    public Combat_Idle(HSM.StateMachine stateMachine, State parent, Player player) : base(stateMachine, parent)
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
        Debug.Log("OnEnter Combat_Idle State");
        _player.Locomotion.CanMove = true;
    }



}
