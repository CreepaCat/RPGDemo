using HSM;
using UnityEngine;

public class N_Talk : State
{
    readonly Player _player;
    public N_Talk(HSM.StateMachine stateMachine, State parent, Player player) : base(stateMachine, parent)
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
