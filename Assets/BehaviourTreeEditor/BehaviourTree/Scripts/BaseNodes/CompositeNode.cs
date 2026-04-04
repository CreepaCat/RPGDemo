using System.Collections.Generic;
using UnityEngine;

namespace MyBehaviourTree
{
    /// <summary>
    /// 组合节点，用于组合不同的子节点
    /// </summary>
    public abstract class CompositeNode : Node
    {
        [HideInInspector] public List<Node> children = new();

        public override void Abort()
        {
            for (int i = 0; i < children.Count; i++)
            {
                Node child = children[i];
                child?.Abort();
            }

            base.Abort();
        }

        public override Node Clone()
        {
            CompositeNode node = Instantiate(this);
            node.children = children.ConvertAll(c => c.Clone());
            return node;
        }

    }
}
