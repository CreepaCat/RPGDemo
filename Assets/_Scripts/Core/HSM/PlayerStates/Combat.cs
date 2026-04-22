using HSM;
using UnityEngine;

public class Combat : State
{
    public readonly Combat_Idle Combat_Idle;
    public readonly Attack Attack;
    public readonly Casting Casting;

    readonly Player _player;
    public Combat(HSM.StateMachine stateMachine, State parent, Player player) : base(stateMachine, parent)
    {
        _player = player;
        Combat_Idle = new(stateMachine, this, player);
        Attack = new(stateMachine, this, player);
        Casting = new(stateMachine, this, player);
    }
    protected override State GetInitialState() => Combat_Idle;



    protected override State GetTransition()
    {
        if (_player.AnimationHandler.IsCasting)
        {
            return Casting;
        }

        if (_player.Fighter.IsAttacking)
        {
            return Attack;
        }


        return Combat_Idle;

    }


    protected override void OnEnter()
    {
        Debug.Log("OnEnter Combat State");
        _player.Animator.SetBool(PlayerAnimatorParamConfig.animaIDIsCombat, true);
        _player.IsInCombat = true;
        //拔剑
        _player.Weapon.UnsheathSword();
    }

    protected override void OnUpdate(float deltaTime)
    {
        if (_player.AttackPerformed)
        {
            _player.AttackPerformed = false;
            _player.Fighter.HandleComboAttack();
        }
    }


}
