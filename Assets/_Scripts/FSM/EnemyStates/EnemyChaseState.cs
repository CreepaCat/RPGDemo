public class EnemyChaseState : EnemyBaseState
{
    public EnemyChaseState(Enemy enemy) : base(enemy)
    {
    }

     public override void OnEnter()
    {
        enemy.AIController.SetAgentChase();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (!enemy.AIController.IsPlayerInChaseRange())
        {
            enemy.StateMachine.TransiteToState(typeof(EnemySuspectState));
        }
        enemy.AIController.UpdateChaseDestionation();
        enemy.Animator.SetFloat(animaIDSpeed,enemy.Agent.velocity.magnitude);
    }
}
