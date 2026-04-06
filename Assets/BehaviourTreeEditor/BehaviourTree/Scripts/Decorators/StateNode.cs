using UnityEngine;

namespace MyBehaviourTree
{
    /// <summary>
    /// 定义一种FSM状态
    /// </summary>
    public class StateNode : RepeatNode
    {
        public BehaviourState RepeatState;
        //使用事件或者回调方法通知状态进入和退出
        protected override void OnStart()
        {
            base.OnStart();
            context.Blackboard.OnStateEnter?.Invoke(RepeatState);

        }

        protected override State OnUpdate()
        {
            context.Blackboard.OnStateUpdate?.Invoke(RepeatState);
            return base.OnUpdate();

        }

        protected override void OnStop()
        {
            base.OnStop();
            context.Blackboard.OnStateExit?.Invoke(RepeatState);

        }
    }
}
