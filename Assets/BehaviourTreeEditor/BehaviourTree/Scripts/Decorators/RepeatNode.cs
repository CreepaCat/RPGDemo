using UnityEngine;

namespace MyBehaviourTree
{
    public class RepeatNode : DecoratorNode
    {
        protected override void OnStart()
        {

        }

        protected override void OnStop()
        {
            //throw new System.NotImplementedException();
        }

        protected override State OnUpdate()
        {

            child.Update();
            return State.Running;
        }
    }
}
