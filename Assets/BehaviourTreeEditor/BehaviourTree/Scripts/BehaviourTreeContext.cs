using RPGDemo.Attributes;
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
        public AIController AIController { get; }
        public AnimationHandler AnimationHandler { get; }
        public Health Health { get; }

        public BehaviourTreeContext(BehaviourTreeRunner runner, Blackboard blackboard)
        {
            Runner = runner;
            Owner = runner.gameObject;
            Transform = runner.transform;
            Blackboard = blackboard;
            Agent = GetComponent<NavMeshAgent>();
            AIController = GetComponent<AIController>();
            AnimationHandler = GetComponent<AnimationHandler>();
            Health = GetComponent<Health>();
        }

        public T GetComponent<T>() where T : Component
        {
            return Owner.GetComponent<T>();
        }
    }
}
