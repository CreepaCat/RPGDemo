using UnityEngine;

/// <summary>
///怀疑状态，当玩家在视线或感知范围内消失时，ai会进入怀疑状态，在原地发愣，
/// 若在一定时间内玩家没有出现，则返回巡逻
/// 否则继续追击或攻击玩家 
/// </summary>
public class EnemySuspectState:EnemyBaseState
{
    public EnemySuspectState(Enemy enemy) : base(enemy)
    {
    }

    public override void OnEnter()
    {
        enemy.Agent.destination = enemy.transform.position;
       
        enemy.AIController.ResetSuspectTimer();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        //若想要更自然，可以在此状态前再加一个惊觉状态
        
        enemy.AIController.UpdateSuspectState();
        enemy.Animator.SetFloat(animaIDSpeed, enemy.Agent.velocity.magnitude);
        //进入攻击状态
        if (enemy.AIController.DistanceToPlayer() < enemy.AIController.GetAttackRange())
        {
            //enemy.StateMachine.TransiteToState(typeof(EnemyAttackState));
            Debug.Log("可以攻击");
        }
        //进入追击状态
        if (enemy.AIController.IsPlayerInSuspectRange())
        {
            enemy.StateMachine.TransiteToState(typeof(EnemyChaseState));
        }
            
        //返回巡逻状态
        if (enemy.AIController.IsSuspectOver())
        {
            enemy.StateMachine.TransiteToState(typeof(EnemyPatrolState));
        }
    }
}
