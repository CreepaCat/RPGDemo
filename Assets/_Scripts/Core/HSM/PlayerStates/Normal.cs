using HSM;
using UnityEngine;

public class Normal : State
{
    public readonly Normal_Idle Normal_Idle; //最上层状态
                                             // public readonly Locomotion Locomotion;
    public readonly N_Talk Talk;
    readonly Player _player;


    public Normal(HSM.StateMachine stateMachine, State parent, Player player) : base(stateMachine, parent)
    {
        _player = player;
        Normal_Idle = new(stateMachine, this, player);
        // Locomotion = new(stateMachine, this, player);
        Talk = new(stateMachine, this, player);
    }
    protected override State GetInitialState() => Normal_Idle;


    protected override State GetTransition()
    {


        if (_player.PlayerCanversant.IsInDialogue)
        {
            // return ((Ground)Parent).Locomotion;
            return Talk;
        }

        return Normal_Idle;

    }

    protected override void OnEnter()
    {
        Debug.Log("OnEnter Normal State");
        //收剑
        _player.Weapon.SheathSword();
        // _player.Input.Interact += HandleInteractInput;
        _player.Animator.SetBool(PlayerAnimatorParamConfig.animaIDIsCombat, false);

    }

    protected override void OnUpdate(float deltaTime)
    {



    }





}
