using UnityEngine;
namespace MyBehaviourTree
{
    /// <summary>
    /// 用于执行逻辑，可选dayindebug message
    /// </summary>
    public class DebugLogNode : ActionNode
    {
        public string message;

        protected override void OnActionStart()
        {
            //  Debug.Log($"OnStart{message}");
        }

        protected override void OnActionStop()
        {
            // Debug.Log($"OnStop{message}");
        }

        protected override State OnActionUpdate()
        {
            // Debug.Log($"OnUpdate{message}");
            if (strategy != null)
            {
                return strategy.Process(context);
            }
            return State.Success;
        }
    }
}
