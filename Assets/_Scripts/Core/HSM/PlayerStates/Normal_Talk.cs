using HSM;
using UnityEngine;

public class Normal_Talk : State
{
    readonly Player _player;
    public Normal_Talk(HSM.StateMachine stateMachine, State parent, Player player) : base(stateMachine, parent)
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
        Debug.Log("OnEnter N_Talk State");
        //收剑
        _player.Weapon.SheathSword();
        _player.Locomotion.CanMove = false;
        _player.DisablePlayerControl();
    }

    protected override void OnExit()
    {
        _player.Locomotion.CanMove = true;
        _player.EnablePlayerControl();
    }

}
