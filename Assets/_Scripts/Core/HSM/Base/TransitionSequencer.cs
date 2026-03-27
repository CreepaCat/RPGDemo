using System.Collections.Generic;
using UnityEngine;

namespace HSM
{
    /// <summary>
    /// 状态转换序列,为异步状态转换预留扩展
    /// </summary>
    public class TransitionSequencer
    {

        public readonly StateMachine StateMachine;

        public TransitionSequencer(StateMachine stateMachine)
        {
            StateMachine = stateMachine;
        }

        /// <summary>
        /// 请求状态转换
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public void RequestTransition(State from, State to)
        {
            StateMachine.ChangeState(from, to);
        }

        /// <summary>
        /// 获取两个节点的最近共同祖先节点
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static State LowestCommonAncestor(State a, State b)
        {
            //1、用哈希表存 a 的path to root
            HashSet<State> aPathToRoot = new();
            foreach (var s in a.PathToRoot())
            {
                aPathToRoot.Add(s);
            }

            //2、向上遍历b，直到pathToRoot中存在该节点
            for (State s = b; s != null; s = s.Parent)
            {
                if (aPathToRoot.Contains(s))
                {
                    return s;
                }
            }
            Debug.Log($"状态{a}和{b} 没有找到最近共同祖先");

            return null;
        }

    }
}
