
namespace MyBehaviourTree
{
    /// <summary>
    /// 条件检测策略接口
    /// </summary>
    public interface ICondition
    {
        bool Check(BehaviourTreeContext context);
    }
}
