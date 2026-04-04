using UnityEngine;

namespace MyBehaviourTree
{
    /// <summary>
    /// 顺序器,按顺序执行子节点,直至所有子节点都被执行完毕
    /// </summary>
    public class SequencerNode : CompositeNode
    {
        int current;
        protected override void OnStart()
        {
            current = 0;

        }

        protected override void OnStop()
        {

        }

        protected override State OnUpdate()
        {
            var child = children[current];
            switch (child.Update())
            {
                case State.Failure:
                    return State.Failure;
                case State.Running:
                    return State.Running;
                case State.Success:
                    current++;
                    break;
            }
            return current == children.Count ? State.Success : State.Running;
        }
    }
}
