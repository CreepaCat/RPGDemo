using HSM;
using UnityEngine;
using UnityEngine.InputSystem;

public class Ground : State
{

    public readonly Normal Normal;
    public readonly Combat Combat;
    public readonly Locomotion Locomotion;
    public readonly Rolling Rolling;

    //public readonly CombatLocomotion CombatLocomotion;
    readonly Player _player;

    bool isInCombat = false;
    bool interactPerformed = false;

    public Ground(HSM.StateMachine stateMachine, State parent, Player player) : base(stateMachine, parent)
    {
        _player = player;
        Normal = new(stateMachine, this, player);
        Combat = new(stateMachine, this, player);
        Rolling = new(stateMachine, this, player);
        Locomotion = new(stateMachine, this, player);


    }
    protected override State GetInitialState()
    {
        return isInCombat ? (State)Combat : Normal;
    }

    protected override State GetTransition()
    {
        if (_player.Locomotion.RollPerformed)
        {
            return Rolling;
        }

        if (_player.Locomotion.IsMoving)
        {
            return Locomotion;
        }


        //管理战斗和非战斗状态切换
        return isInCombat ? (State)Combat : Normal;





    }
    protected override void OnEnter()
    {

        _player.Animator.SetBool(PlayerAnimatorParamConfig.animIDIsFalling, false);
        _player.Input.Interact += InteractPerformed;
        interactPerformed = false;
    }
    protected override void OnExit()
    {
        // _player.Input.Interact -= HandleInteractInput;
        _player.Input.Interact -= InteractPerformed;
    }

    protected override void OnUpdate(float deltaTime)
    {
        Debug.Log("on state Ground");
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            isInCombat = !isInCombat;

        }

        _player.Locomotion.CaculateMovement();
        // _player.Locomotion.CheckRolling();

        if (interactPerformed)
        {
            HandleInteractInput();
            interactPerformed = false;
        }

    }

    private void HandleInteractInput()
    {
        Debug.Log("Normal State + HandleInteractInput");
        _player.Interactor.DoInteract();

    }
    private void InteractPerformed()
    {
        interactPerformed = true;
    }




}
