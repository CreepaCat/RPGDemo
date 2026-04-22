using HSM;
using UnityEngine;

public class Attack : State
{
    readonly Player _player;
    public Attack(HSM.StateMachine stateMachine, State parent, Player player) : base(stateMachine, parent)
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
        Debug.Log("OnEnter Attack State");
        _player.Animator.SetBool("DoCombo", false);
        _player.Locomotion.CanMove = false;
    }

    protected override void OnExit()
    {
        Debug.Log("OnExit Attack State");
        _player.Animator.SetBool("DoCombo", false);
    }

}
