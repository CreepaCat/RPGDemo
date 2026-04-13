using HSM;
using UnityEngine;

public class Combat : State
{
    public readonly Combat_Idle Combat_Idle;
    // public readonly C_Locomotion Locomotion;
    public readonly Attack Attack;
    public readonly Casting Casting;

    readonly Player _player;

    bool attackPerformed = false;
    public Combat(HSM.StateMachine stateMachine, State parent, Player player) : base(stateMachine, parent)
    {
        _player = player;
        Combat_Idle = new(stateMachine, this, player);
        // Locomotion = new(stateMachine, this, player);
        Attack = new(stateMachine, this, player);
        Casting = new(stateMachine, this, player);
    }
    protected override State GetInitialState() => Combat_Idle;



    protected override State GetTransition()
    {
        //如果在战斗返回战斗idle
        //如果在普通状态，返回普通Idle
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
        _player.Animator.SetBool(PlayerAnimatorParamConfig.animaIDIsCombat, true);
        Debug.Log("OnEnter Combat State");

        (Parent as Ground).isInCombat = true;
        //拔剑
        _player.Weapon.UnsheathSword();
        _player.Input.Attack += AttackPerformed;
        attackPerformed = false;
    }

    protected override void OnExit()
    {
        _player.Input.Attack -= AttackPerformed;
    }

    protected override void OnUpdate(float deltaTime)
    {
        if (attackPerformed)
        {
            attackPerformed = false;
            _player.Fighter.HandleComboAttack();
        }
    }

    void AttackPerformed() => attackPerformed = true;


}
