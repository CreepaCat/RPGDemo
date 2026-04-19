
using System;
using System.Collections.Generic;
using UnityEngine;


namespace HSM
{

    public class StateMachine
    {
        public readonly State Root;
        public readonly TransitionSequencer Sequencer;

        internal readonly Dictionary<System.Type, State> StateDict = new();

        //Cache
        bool started; //状态机是否已启动，防止重复启动

        public StateMachine(State root)
        {
            Root = root;
            Sequencer = new TransitionSequencer(this);
        }

        public void Start()
        {
            if (started) return;
            started = true;
            Root.InnerEnter();
        }


        /// <summary>
        /// 供MonoBehaiviour调用的逻辑Tick方法
        public void Tick(float deltaTime)
        {
            if (!started) Start();
            InternalTick(deltaTime);

        }

        //
        internal void InternalTick(float deltaTime) => Root.InnerUpdate(deltaTime);


        public void ChangeState(State from, State to)
        {

            if (from == to || from == null || to == null) return;


            //1、获取最近共同祖先节点lca
            State lca = TransitionSequencer.LowestCommonAncestor(from, to);
            if (lca == null) return;

            //2、从from节点退回到lca节点
            for (State s = from; s != lca; s = s.Parent)
            {
                s.InnerExit();
            }
            //3、栈式存储to到lca的节点路径
            Stack<State> stack = new();

            for (State s = to; s != lca; s = s.Parent)
            {
                stack.Push(s);
            }

            //4、根据路径逐步执行节点进入
            while (stack.Count > 0)
                stack.Pop().InnerEnter();

        }

        internal State GetState(Type type)
        {
            if (!StateDict.ContainsKey(type))
            {
                Debug.LogError($"状态机没有状态： {type}");
                return null;

            }

            return StateDict[type];
        }
    }
}
