using UnityEngine;

namespace MyBehaviourTree
{
    public class BehaviourTreeRunner : MonoBehaviour
    {
        public BehaviourTree tree;
        public Blackboard blackboard = new();
        //  public string currentState;


        private void Awake()
        {
            if (tree == null)
            {
                return;
            }
            tree = tree.Clone();
            tree.Bind(this);

        }
        private void Start()
        {
            Transform player = GameObject.FindWithTag("Player").transform;
            blackboard.SetPlayer(player);

            blackboard.CurrentState = BehaviourState.Patrol;
        }

        private void Update()
        {
            tree?.Update();
            // if (aIController.IsPlayerInAttackRange())
            // {
            //     blackboard.CurrentState = BehaviourState.Attack;

            // }
            // // else if (aIController.IsPlayerInMinSuspectRange())
            // // {
            // //     blackboard.CurrentState = BehaviourState.Chase;

            // // }
            // else if (aIController.IsPlayerInChaseRange() && blackboard.CurrentState == BehaviourState.Attack)
            // {
            //     blackboard.CurrentState = BehaviourState.Chase;
            // }
            // else if (aIController.IsPlayerInChaseRange() && aIController.IsSuspectOver() && blackboard.CurrentState == BehaviourState.Suspect)
            // {
            //     blackboard.CurrentState = BehaviourState.Chase;
            // }
            // else if (!aIController.IsPlayerInChaseRange() && blackboard.CurrentState == BehaviourState.Chase)
            // {
            //     blackboard.CurrentState = BehaviourState.Suspect;

            // }
            // else if (aIController.IsPlayerInMinSuspectRange() && blackboard.CurrentState == BehaviourState.Patrol)
            // {
            //     blackboard.CurrentState = BehaviourState.Suspect;

            // }
            // else if (!aIController.IsPlayerInChaseRange() && aIController.IsSuspectOver() && blackboard.CurrentState == BehaviourState.Suspect)
            // {
            //     blackboard.CurrentState = BehaviourState.Patrol;

            // }

        }


    }
}
