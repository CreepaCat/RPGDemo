using UnityEngine;

public class EnemyBaseState:IState
{
   
    protected Enemy enemy;
  
    protected int animaIDSpeed = Animator.StringToHash("Speed");
    protected int animaIDDeath = Animator.StringToHash("Death");

    public EnemyBaseState(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public virtual void OnEnter()
    {
        //no op
    }

    public virtual void OnExit()
    {
        //no op
    }

    public virtual void OnUpdate()
    {
        //no op

        if (enemy.IsDead())
        {
            enemy.StateMachine.TransiteToState(typeof(EnemyDeathState));
        }
    }

    public virtual void OnFixedUpdate()
    {
        //no op
    }
}
