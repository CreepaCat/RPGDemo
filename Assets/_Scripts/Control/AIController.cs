using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(PathPatrol))]
[RequireComponent(typeof(PlayerDetector))]
public class AIController : MonoBehaviour
{
    [Header("巡逻参数")]
    [SerializeField] private float _speed = 4f;
    [SerializeField] private float _patrolTimeout = 3f;
    [SerializeField] private float _stoppingDistance = 0.5f;

    [Header("疑惑状态参数")]
    [SerializeField] private float _suspectTimeout = 3f;
    [SerializeField] private float _minSuspectRange = 8f; //最小疑惑距离，小于这个距离直接进入追击状态


    [Header("追击参数")]
    [SerializeField] private float _chaseSpeed = 6f;
    [SerializeField] private float _chaseTargetUpdateTimeout = 0.6f;
    [SerializeField] private float _chaseStoppingDistance = 1f;
    [SerializeField] private float _minChaseRange = 16f;


    [Header("攻击参数")]
    [SerializeField] private float _attackRange = 1.5f;
    [SerializeField] private float _attackDamage = 5f;




    private NavMeshAgent agent;
    private PathPatrol patrol;

    PlayerDetector playerDetector;
    AnimationHandler animationHandler;
    Rigidbody rb;
    Enemy enemy;


    private float patrolDeltaTime;
    private float chaseDeltaTime;
    private float suspectDeltaTime;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        patrol = GetComponent<PathPatrol>();
        playerDetector = GetComponent<PlayerDetector>();
        animationHandler = GetComponent<AnimationHandler>();
        rb = GetComponent<Rigidbody>();
        enemy = GetComponent<Enemy>();

        rb.isKinematic = true;
        FreezeRb();
    }

    private void FreezeRb()
    {

        rb.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
    }

    private void DefreezeRb()
    {
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    private void Start()
    {
        SetAgentPatrol();
    }

    private void Update()
    {
        if (animationHandler.UsingRootMotion || animationHandler.IsInteracting)
        {
            agent.isStopped = true;
            DefreezeRb();
        }
        else if (agent.isStopped)
        {
            agent.isStopped = false;
            FreezeRb();
        }

        if (enemy.TreeRunner.currentState == "Patrol")
        {

        }

        float speed = agent.isStopped ? 0f : agent.desiredVelocity.magnitude;
        animationHandler.Animator.SetFloat("Speed", speed);


    }

    public void SetAgentPatrol()
    {
        agent.speed = _speed;
        agent.stoppingDistance = _stoppingDistance;
        agent.destination = patrol.GetCurrentWaypoint().position;
    }

    public void SetAgentChase()
    {
        agent.speed = _chaseSpeed;
        agent.stoppingDistance = _chaseStoppingDistance;
        agent.destination = GetPlayerPosition();
    }
    public Vector3 GetPlayerPosition() => playerDetector.GetPlayerPosition();

    public float GetMinChaseRange() => _minChaseRange;
    public float GetMinSuspectRange() => _minSuspectRange;
    public float GetAttackRange() => _attackRange;

    public bool IsPlayerInSuspectRange() => DistanceToPlayer() < _minSuspectRange;
    public bool IsPlayerInChaseRange() => DistanceToPlayer() < _minChaseRange;
    public bool IsPlayerInAttackRange() => DistanceToPlayer() < _attackRange;
    public bool IsSuspectOver() => suspectDeltaTime > _suspectTimeout;
    public void ResetSuspectTimer() => suspectDeltaTime = 0f;

    public void UpdateChaseDestionation()
    {
        agent.speed = _chaseSpeed;
        agent.stoppingDistance = _chaseStoppingDistance;
        agent.destination = GetPlayerPosition();
        chaseDeltaTime += Time.deltaTime;
        if (chaseDeltaTime > _chaseTargetUpdateTimeout)
        {
            agent.destination = GetPlayerPosition();
            chaseDeltaTime = 0f;
        }

    }


    public void UpdateSuspectState()
    {
        agent.speed = 0f;
        suspectDeltaTime += Time.deltaTime;

    }
    public void UpdatePatrolDestination()
    {

        if (agent.remainingDistance <= agent.stoppingDistance && agent.pathStatus == NavMeshPathStatus.PathComplete)
        {
            patrolDeltaTime += Time.deltaTime;
            if (patrolDeltaTime >= _patrolTimeout)
            {
                agent.destination = patrol.GetNextWaypoint().position;
                patrolDeltaTime = 0f;
            }
        }
    }
    public void Move(Vector3 movement, Quaternion deltaRotation)
    {
        var targetMovement = movement;


        rb.MovePosition(rb.position + targetMovement);

        if (deltaRotation != Quaternion.identity)
        {
            rb.MoveRotation(rb.rotation * deltaRotation);
            //  _cachedMoveDirection = (rb.rotation * deltaRotation) * Vector3.forward;
        }

    }


    public float DistanceToPlayer()
    {
        return Vector3.Distance(transform.position, playerDetector.GetPlayerPosition());
    }




    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _minSuspectRange);
    }
}
