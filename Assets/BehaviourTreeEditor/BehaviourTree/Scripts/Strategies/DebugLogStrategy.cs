using MyBehaviourTree;
using UnityEngine;

public class DebugLogStrategy : ActionStrategy
{
    public string message;
    public override Node.State Process(BehaviourTreeContext context)
    {
        Debug.Log($"Process{message}");
        return Node.State.Success;
    }
}
