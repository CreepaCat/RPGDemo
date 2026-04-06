using System;
using UnityEngine;

namespace MyBehaviourTree
{
    /// <summary>
    /// 条件节点用于特定条件检查，应当永远处于运行状态，除非被Abort
    /// </summary>
    public class ConditionNode : ActionNode
    {
        public BehaviourState toState;
        public bool isNagetive;

        [SerializeReference] private ICondition condition;

        protected override Node.State OnActionUpdate()
        {

            if (condition == null)
                return Node.State.Failure;

            if (context.Health.IsDead())
            {
                context.Blackboard.CurrentState = BehaviourState.Death;
                return State.Running;
            }
            bool result = condition.Check(context) == !isNagetive;

            if (result)
            {
                context.Blackboard.CurrentState = toState;
            }
            return State.Running;
        }
    }
}
