using UnityEngine;
namespace MyBehaviourTree
{
    public class BehaviourTreeRunner : MonoBehaviour
    {
        public BehaviourTree tree;
        public Blackboard blackboard = new Blackboard();
        public string currentState;

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
            blackboard.SetString("CurrentState", "Patrol");
        }

        private void Update()
        {
            tree?.Update();

            blackboard.TryGetString("CurrentState", out currentState);

        }


    }
}
