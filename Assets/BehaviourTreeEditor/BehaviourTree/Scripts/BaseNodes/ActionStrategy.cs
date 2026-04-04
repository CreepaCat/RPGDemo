using System;

namespace MyBehaviourTree
{
    [Serializable]
    public abstract class ActionStrategy : ISrategy
    {
        public virtual void OnStart(BehaviourTreeContext context) { }
        public abstract Node.State Process(BehaviourTreeContext context);
        public virtual void OnStop(BehaviourTreeContext context) { }
    }
}
