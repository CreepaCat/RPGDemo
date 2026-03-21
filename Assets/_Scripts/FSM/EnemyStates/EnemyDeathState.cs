
public class EnemyDeathState : EnemyBaseState
{
    public EnemyDeathState(Enemy enemy) : base(enemy)
    {
    }

    public override void OnEnter()
    {
        enemy.Agent.isStopped = true;
        enemy.AIController.enabled = false;
        enemy.Animator.SetBool(animaIDDeath, true);
    }
}
