using System.Collections.Generic;
using UnityEngine;
namespace MyBehaviourTree
{
    /// <summary>
    /// 真正的并行执行：所有子节点每帧都会被更新。
    //失败快速返回：任意一个子节点失败，整个节点立刻失败，并停止其他子节点（避免浪费性能）。
    //成功条件严格：必须所有子节点都成功，才算整个 Parallel 成功。
    /// </summary>
    public class Parallel : CompositeNode
    {
        List<State> childrenLeftToExecute = new List<State>();

        protected override void OnStart()
        {
            childrenLeftToExecute.Clear();
            children.ForEach(a =>
            {
                childrenLeftToExecute.Add(State.Running);
            });
        }

        protected override void OnStop()
        {
        }

        protected override State OnUpdate()
        {
            bool stillRunning = false;
            for (int i = 0; i < childrenLeftToExecute.Count; ++i)
            {
                if (childrenLeftToExecute[i] == State.Running)
                {
                    var status = children[i].Update();
                    if (status == State.Failure)
                    {
                        AbortRunningChildren();
                        return State.Failure;
                    }

                    if (status == State.Running)
                    {
                        stillRunning = true;
                    }

                    childrenLeftToExecute[i] = status;
                }
            }

            return stillRunning ? State.Running : State.Success;
        }

        void AbortRunningChildren()
        {
            for (int i = 0; i < childrenLeftToExecute.Count; ++i)
            {
                if (childrenLeftToExecute[i] == State.Running)
                {
                    children[i].Abort();
                }
            }
        }
    }
}
