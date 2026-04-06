using UnityEngine;

namespace MyBehaviourTree
{

    public class WaitNode : ActionNode
    {
        public float duration = 1f;
        float startTime;

        protected override void OnActionStart()
        {
            startTime = Time.time;
        }

        protected override void OnActionStop() { }

        protected override State OnActionUpdate()
        {
            if (Time.time > startTime + duration)
            {
                return State.Success;
            }
            return State.Running;
        }
    }
}
