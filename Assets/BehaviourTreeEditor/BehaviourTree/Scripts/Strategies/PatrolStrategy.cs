using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace MyBehaviourTree
{
    [System.Serializable]
    public class PatrolStrategy : ActionStrategy
    {
        public List<Transform> wayPoints;

        [SerializeField] int currentIndex;

        public override void OnStart(BehaviourTreeContext context)
        {
            base.OnStart(context);
            context.Agent.isStopped = false;
        }

        public override Node.State Process(BehaviourTreeContext context)
        {
            NavMeshAgent agent = context.Agent;
            if (agent == null) return Node.State.Failure;
            agent.SetDestination(wayPoints[currentIndex].position);

            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                return Node.State.Success;
            }
            return Node.State.Running;
        }

        public override void OnStop(BehaviourTreeContext context)
        {
            base.OnStart(context);
            context.Agent.isStopped = true;
            currentIndex = GetNextPoint();
        }

        private int GetNextPoint()
        {
            return (currentIndex + 1) % wayPoints.Count;
        }
    }

}
