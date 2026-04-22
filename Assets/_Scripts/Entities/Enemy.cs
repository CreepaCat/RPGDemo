using RPGDemo.Stats;
using UnityEngine;
using UnityEngine.AI;
using RPGDemo.Attributes;
using MyBehaviourTree;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AIController))]
[RequireComponent(typeof(BehaviourTreeRunner))]
public class Enemy : Character
{
    public Health Health => health;
    public NavMeshAgent Agent => agent;
    public Animator Animator => animator;
    public AIController AIController => aiController;
    public BehaviourTreeRunner TreeRunner => treeRunner;

    [SerializeField] ParticleSystem VFX;

    NavMeshAgent agent;
    Animator animator;
    AIController aiController;
    Health health;
    BaseStats baseStats;
    BehaviourTreeRunner treeRunner; //不使用状态机而使用行为树


    Player player;
    EnemyGroup enemyGroup; //敌人团伙

    protected override void Awake()
    {
        base.Awake();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        aiController = GetComponent<AIController>();
        health = GetComponent<Health>();
        baseStats = GetComponent<BaseStats>();
        treeRunner = GetComponent<BehaviourTreeRunner>();



        agent.isStopped = false;

        player = GameObject.FindWithTag("Player").GetComponent<Player>();

        enemyGroup = GetComponentInParent<EnemyGroup>();

        if (enemyGroup != null)
        {
            enemyGroup.AddEnemy(this);
        }
    }

    private void OnEnable()
    {
        health.OnHealthChanged += PlayTakeDamageAnimation;
        health.OnDeath += OnDeath;

        TreeRunner.blackboard.OnStateEnter += OnStateEnterCallBack;
        TreeRunner.blackboard.OnStateExit += OnStateExitCallBack;
        TreeRunner.blackboard.OnStateUpdate += OnStateUpdateCallback;
    }

    private void OnDisable()
    {
        health.OnHealthChanged -= PlayTakeDamageAnimation;

        TreeRunner.blackboard.OnStateEnter -= OnStateEnterCallBack;
        TreeRunner.blackboard.OnStateExit -= OnStateExitCallBack;
        TreeRunner.blackboard.OnStateUpdate -= OnStateUpdateCallback;
    }

    public bool IsDead() => health.IsDead();



    #region 状态切换回调方法
    private void OnStateEnterCallBack(BehaviourState state)
    {
        switch (state)
        {
            case BehaviourState.Patrol:
                aiController.SetAgentPatrol();
                break;
            case BehaviourState.Suspect:
                aiController.ResetSuspectTimer();
                break;
            case BehaviourState.Chase:
                aiController.SetAgentChase();
                break;
            case BehaviourState.Attack:
                break;
            case BehaviourState.Death:
                aiController.Disable();
                break;
        }
    }

    private void OnStateUpdateCallback(BehaviourState state)
    {
        //NO OP
    }

    private void OnStateExitCallBack(BehaviourState state)
    {
        //NO OP
    }

    #endregion


    private void OnDeath()
    {
        aiController.Disable();
        player.Experience.GainExp((int)baseStats.GetStats(StatsType.Experience));

        AnimationHandler.PlayInterruptAnimation(Animator.StringToHash("Death"), true);
        Fighter.DisableWeaponDamage();
        enemyGroup?.RemoveEnemy(this);

        if (VFX != null)
        {
            Destroy(VFX.gameObject);
        }


        StartCoroutine(DeathCoroutine(5f));

    }

    private void PlayTakeDamageAnimation(float healthChnagedValue)
    {
        if (healthChnagedValue > 0)
        {
            // animationHand
            animator.Play("Hit");
            animator.CrossFade("Hit", 0.2f);
        }


    }
    /// <summary>
    /// 根动画移动
    /// </summary>
    /// <param name="deltaPosition"></param>
    /// <param name="deltaRotation"></param>
    internal void Move(Vector3 deltaPosition, Quaternion deltaRotation)
    {
        agent.isStopped = true;
        aiController.Move(deltaPosition, deltaRotation);

    }
    /// <summary>
    /// 死亡后隐藏尸体
    /// </summary>
    /// <param name="waitTiem"></param>
    /// <returns></returns>
    IEnumerator DeathCoroutine(float waitTiem)
    {
        yield return new WaitForSeconds(waitTiem);

        while (transform.position.y > -2f)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - 1f * Time.deltaTime, transform.position.z);
            yield return null;

        }

        transform.gameObject.SetActive(false);

    }
}
