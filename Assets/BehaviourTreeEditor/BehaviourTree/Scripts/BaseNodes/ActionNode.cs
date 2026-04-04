using UnityEngine;


namespace MyBehaviourTree
{
    /// <summary>
    /// 最底层节点，用于执行具体逻辑
    /// 可以结合策略模式来定义不同的节点行为
    /// </summary>
    public abstract class ActionNode : Node
    {

        [SerializeReference]
        public ActionStrategy strategy;

        protected override void OnStart()
        {
            strategy?.OnStart(context);
            OnActionStart();
        }

        protected override void OnStop()
        {
            strategy?.OnStop(context);
            OnActionStop();
        }

        protected override State OnUpdate()
        {
            if (strategy != null)
            {
                return strategy.Process(context);
            }

            return OnActionUpdate();
        }

        protected virtual void OnActionStart() { }
        protected virtual void OnActionStop() { }
        protected virtual State OnActionUpdate() => State.Failure;
    }
}
