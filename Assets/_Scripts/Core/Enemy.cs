using System;
using RPGDemo.Stats;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using RPGDemo.Attributes;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AIController))]
public class Enemy : Character
{
    public StateMachine StateMachine => stateMachine;
    public NavMeshAgent Agent => agent;
    public Animator Animator => animator;
    public AIController AIController => aiController;


    StateMachine stateMachine;
    NavMeshAgent agent;
    Animator animator;
    AIController aiController;
    Health health;
    BaseStats baseStats;

    Player player;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        aiController = GetComponent<AIController>();
        health = GetComponent<Health>();
        baseStats = GetComponent<BaseStats>();

        agent.isStopped = false;

        player = GameObject.FindWithTag("Player").GetComponent<Player>();
    }

    private void OnEnable()
    {
        health.OnHealthChanged += PlayTakeDamageAnimation;
        health.OnDeath += OnDeathCallback;
    }

    private void OnDisable()
    {
        health.OnHealthChanged -= PlayTakeDamageAnimation;
    }

    public bool IsDead() => health.IsDead();

    private void Start()
    {
        EnemyPatrolState patrolState = new EnemyPatrolState(this);
        EnemyChaseState chaseState = new EnemyChaseState(this);
        EnemySuspectState suspectState = new EnemySuspectState(this);
        EnemyDeathState deathState = new EnemyDeathState(this);

        stateMachine = new StateMachine(patrolState);
        stateMachine.AddState(chaseState);
        stateMachine.AddState(suspectState);
        stateMachine.AddState(deathState);
    }

    void Update()
    {
        stateMachine.OnUpdate();

    }

    void FixedUpdate()
    {
        stateMachine.OnFixedUpdate();
    }

    private void OnDeathCallback()
    {
        player.Experience.GainExp((int)baseStats.GetStats(StatsType.Experience));

    }

    private void PlayTakeDamageAnimation(float healthChnagedValue)
    {
        animator.Play("Hit");
        animator.CrossFade("Hit", 0.2f);
    }

}
