using UnityEngine;

public class EnemyPatrolState : EnemyBaseState
{
    public EnemyPatrolState(Enemy enemy) : base(enemy)
    {
    }

    public override void OnEnter()
    {
        Debug.Log("进入巡逻状态");
        enemy.AIController.SetAgentPatrol();
    }
    
    public override void OnUpdate()
    {
        base.OnUpdate();
        if (enemy.AIController.IsPlayerInSuspectRange())
        {
            enemy.StateMachine.TransiteToState(typeof(EnemySuspectState));
        }
        enemy.AIController.UpdatePatrolDestination();
        enemy.Animator.SetFloat(animaIDSpeed,enemy.Agent.velocity.magnitude);
        
    }
}
