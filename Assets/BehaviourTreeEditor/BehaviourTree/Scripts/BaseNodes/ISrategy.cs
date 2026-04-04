namespace MyBehaviourTree
{
    /// <summary>
    /// Action策略，用来定义执行不同的行为
    /// </summary>
    public interface ISrategy
    {
        void OnStart(BehaviourTreeContext context);
        Node.State Process(BehaviourTreeContext context);
        void OnStop(BehaviourTreeContext context);

    }
}
