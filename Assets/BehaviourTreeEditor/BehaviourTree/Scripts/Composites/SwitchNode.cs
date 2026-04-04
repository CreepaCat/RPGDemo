using System;
using System.Collections.Generic;
using UnityEngine;

namespace MyBehaviourTree
{
    /// <summary>
    /// 类似FSM的状态切换节点:
    /// 读取黑板中的状态值，执行对应case的子节点。
    /// </summary>
    public class SwitchNode : CompositeNode
    {
        public string stateKey = "CurrentState";
        public int defaultChildIndex = -1;
        public List<string> caseValues = new(); //与黑板设定的状态值对应

        private int runningChildIndex = -1;

        protected override void OnStart()
        {
            SyncCasesWithChildren();
            runningChildIndex = -1;
        }

        protected override void OnStop()
        {
            AbortRunningChild();
        }

        protected override State OnUpdate()
        {
            SyncCasesWithChildren();
            if (children.Count == 0)
            {
                return State.Failure;
            }

            int targetIndex = ResolveTargetChildIndex();
            if (targetIndex < 0 || targetIndex >= children.Count)
            {
                AbortRunningChild();
                return State.Failure;
            }

            if (runningChildIndex != -1 && runningChildIndex != targetIndex)
            {
                children[runningChildIndex]?.Abort();
            }

            runningChildIndex = targetIndex;
            return children[targetIndex].Update();
        }

        public void SyncCasesWithChildren()
        {
            if (caseValues == null)
            {
                caseValues = new List<string>();
            }

            while (caseValues.Count < children.Count)
            {
                caseValues.Add(string.Empty);
            }

            if (caseValues.Count > children.Count)
            {
                caseValues.RemoveRange(children.Count, caseValues.Count - children.Count);
            }

            if (defaultChildIndex >= children.Count)
            {
                defaultChildIndex = -1;
            }
        }

        public override void Abort()
        {
            AbortRunningChild();
            base.Abort();
        }

        private int ResolveTargetChildIndex()
        {
            string currentState = string.Empty;
            if (context?.Blackboard != null)
            {
                context.Blackboard.TryGetString(stateKey, out currentState);
            }

            for (int i = 0; i < caseValues.Count; i++)
            {
                if (string.Equals(caseValues[i], currentState, StringComparison.Ordinal))
                {
                    return i;
                }
            }

            return defaultChildIndex;
        }

        private void AbortRunningChild()
        {
            if (runningChildIndex >= 0 && runningChildIndex < children.Count)
            {
                children[runningChildIndex]?.Abort();
            }

            runningChildIndex = -1;
        }
    }
}
