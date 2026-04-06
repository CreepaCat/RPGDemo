using UnityEngine;

namespace MyBehaviourTree
{
    [System.Serializable]
    public class AttackStrategy : ActionStrategy
    {
        public override void OnStart(BehaviourTreeContext context)
        {
            base.OnStart(context);
            context.AIController.Attack();
        }
        public override Node.State Process(BehaviourTreeContext context)
        {
            return Node.State.Success;
        }
    }


}
