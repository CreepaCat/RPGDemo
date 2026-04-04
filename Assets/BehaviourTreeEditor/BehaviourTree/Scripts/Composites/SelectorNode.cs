using UnityEngine;

namespace MyBehaviourTree
{
    /// <summary>
    /// 选择器,按顺序执行子节点,只要有一个成功就返回成功
    /// </summary>
    public class SelectorNode : CompositeNode
    {
        protected int current;
        protected override void OnStart()
        {
            current = 0;
        }

        protected override void OnStop()
        {
            // throw new System.NotImplementedException();
        }

        protected override State OnUpdate()
        {
            for (int i = 0; i < children.Count; i++)
            {
                current = i;
                var child = children[current];
                switch (child.Update())
                {
                    case State.Failure:
                        continue;
                    case State.Running:
                        return State.Running;
                    case State.Success:
                        return State.Success;
                }

            }

            return State.Failure;
        }
    }
}
