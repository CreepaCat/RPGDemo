using System;
using System.Collections.Generic;
using UnityEngine;

namespace MyBehaviourTree
{
    /// <summary>
    /// 类似FSM的状态切换节点:
    /// 读取黑板中的状态值，执行对应case的子节点。
    /// 子节点必须是StateNode
    /// </summary>
    public class SwitchNode : CompositeNode
    {
        public BehaviourState CurrentState;

        private StateNode currentRunningState;

        protected override void OnStart()
        {
            currentRunningState = null;
        }

        protected override void OnStop()
        {
            AbortRunningChild();
        }

        protected override State OnUpdate()
        {
            if (children.Count == 0)
            {
                return State.Failure;
            }

            var runningState = ResolveTargetChild();
            if (runningState == null)
            {
                return State.Failure;
            }
            if (currentRunningState != runningState)
            {
                AbortRunningChild();
                currentRunningState = runningState;
            }

            return runningState.Update();
        }

        public override void Abort()
        {
            AbortRunningChild();
            base.Abort();
        }

        private void AbortRunningChild()
        {
            currentRunningState?.Abort();
        }

        private StateNode ResolveTargetChild()
        {
            if (context?.Blackboard != null)
            {
                CurrentState = context.Blackboard.CurrentState;
            }

            foreach (var child in children)
            {
                StateNode state = child as StateNode;
                if (state == null) continue;
                if (state.RepeatState == CurrentState)
                {
                    return state;
                }
            }
            return null;
        }
    }
}
