using UnityEngine;
using UnityEngine.AI;

namespace MyBehaviourTree
{

    public sealed class BehaviourTreeContext
    {
        public BehaviourTreeRunner Runner { get; }
        public GameObject Owner { get; }
        public Transform Transform { get; }
        public Blackboard Blackboard { get; }

        public NavMeshAgent Agent { get; }

        public BehaviourTreeContext(BehaviourTreeRunner runner, Blackboard blackboard)
        {
            Runner = runner;
            Owner = runner.gameObject;
            Transform = runner.transform;
            Blackboard = blackboard;
            Agent = GetComponent<NavMeshAgent>();
        }

        public T GetComponent<T>() where T : Component
        {
            return Owner.GetComponent<T>();
        }
    }
}
