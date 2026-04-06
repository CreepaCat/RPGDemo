using HSM;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerRoot : State
{
    public readonly Ground Ground; //最上层状态
    public readonly Airbone Airbone; //最上层

    public readonly Death Death;


    readonly Player _player;
    float LandingVerticalSpeed = -1.9f;
    bool isGroundedOrLanding = false;

    public PlayerRoot(HSM.StateMachine stateMachine, Player player) : base(stateMachine, null)
    {
        _player = player;
        Ground = new(stateMachine, this, player);
        Airbone = new(stateMachine, this, player);
        Death = new(stateMachine, this, player);


    }

    protected override State GetInitialState()
    {
        if (_player.Health.IsDead())
        {
            return Death;
        }
        if (_player.Locomotion.JumpPerformed || !_player.Locomotion.Grounded)
        {
            return Airbone;
        }
        return Ground;

    }

    protected override State GetTransition()
    {
        if (_player.Health.IsDead())
        {
            return Death;
        }
        if (_player.Locomotion.JumpPerformed || !_player.Locomotion.Grounded)
        {
            return Airbone;
        }
        return Ground;
        //  return null;


    }

}
