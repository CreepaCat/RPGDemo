using UnityEngine;
using UnityEngine.AI;
namespace MyBehaviourTree
{
    [System.Serializable]
    public class MoveToStrategy : ActionStrategy
    {
        [SerializeField] bool moveToPlayer;
        [SerializeField] bool moveToSelf;
        [SerializeField] private bool useBlackboardTarget = true;
        [SerializeField] private string targetKey = "Target";
        [SerializeField] private Transform targetPoint;
        [SerializeField] private float arriveDistanceOverride = -1f;

        public override Node.State Process(BehaviourTreeContext context)
        {
            if (context == null)
            {
                return Node.State.Failure;
            }

            // NavMeshAgent agent = context.GetComponent<NavMeshAgent>();
            // if (agent == null)
            // {
            //     return Node.State.Failure;
            // }
            var agent = context.Agent;

            Transform target = ResolveTarget(context);
            if (target == null)
            {
                return Node.State.Failure;
            }

            agent.SetDestination(target.position);

            float stopDistance = arriveDistanceOverride > 0f ? arriveDistanceOverride : agent.stoppingDistance;
            if (!agent.pathPending && agent.remainingDistance <= stopDistance)
            {
                return Node.State.Success;
            }

            return Node.State.Running;
        }

        private Transform ResolveTarget(BehaviourTreeContext context)
        {
            if (moveToPlayer)
            {
                return context.Blackboard.GetPlayer();
            }
            if (moveToSelf)
            {
                return context.Transform;
            }
            if (useBlackboardTarget && context.Blackboard != null &&
                context.Blackboard.TryGetTransform(targetKey, out Transform blackboardTarget))
            {
                return blackboardTarget;
            }

            return targetPoint;
        }
    }
}
