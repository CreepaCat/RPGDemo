using UnityEngine;

namespace MyBehaviourTree
{
    public class RandomSelectorNode : CompositeNode
    {
        protected int current;
        protected override void OnStart()
        {
            current = Random.Range(0, children.Count);
        }

        protected override void OnStop()
        {

        }

        protected override State OnUpdate()
        {
            return children[current].Update();
        }
    }
}
