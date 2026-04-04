using UnityEngine;

namespace MyBehaviourTree
{
    /// <summary>
    /// 装饰节点，用于装饰行为，只有一个子节点
    /// </summary>
    public abstract class DecoratorNode : Node
    {
        [HideInInspector] public Node child;

        public override void Abort()
        {
            child?.Abort();
            base.Abort();
        }

        public override Node Clone()
        {
            DecoratorNode dn = Instantiate(this);
            dn.child = child?.Clone();
            return dn;
        }

    }


}
