
using HSM;
using UnityEngine;


public class Casting : State
{
    readonly Player _player;
    public Casting(HSM.StateMachine stateMachine, State parent, Player player) : base(stateMachine, parent)
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
        Debug.Log("OnEnter Casting State");
        _player.Locomotion.CanMove = false;
    }


}
