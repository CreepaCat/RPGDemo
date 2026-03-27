using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace HSM
{
    /// <summary>
    /// 状态机建造类，通过反射递归遍历根节点的所有子节点，并布线到目标状态机
    /// </summary>
    public class StateMachineBuilder
    {
        readonly State root;

        public StateMachineBuilder(State root)
        {
            this.root = root;
        }

        public StateMachine Build()
        {
            var m = new StateMachine(root);
            var hashSet = new HashSet<State>();
            Wire(root, m, hashSet);

            foreach (var s in hashSet)
            {
                m.StateDict[s.GetType()] = s;
            }
            return m;
        }
        /// <summary>
        /// 通过反射对状态树进行布线
        /// </summary>
        /// <param name="s"></param>
        /// <param name="m"></param>
        /// <param name="visited"></param>
        void Wire(State s, StateMachine m, HashSet<State> visited)
        {
            if (s == null) return;
            if (!visited.Add(s)) return;


            //获得对象公共类或非公共类或所有继承类
            var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;
            var machineField = typeof(State).GetField("StateMachine", flags);
            if (machineField != null) machineField.SetValue(s, m);  //将状态机记录到该节点


            //todo:将state加进machine字典，key为type

            foreach (var fld in s.GetType().GetFields(flags))
            {
                if (!typeof(State).IsAssignableFrom(fld.FieldType)) continue; //只考虑State类型和其继承类型
                if (fld.Name == "Parent") continue; // 跳过父节点，防止死循环

                var child = (State)fld.GetValue(s);
                if (child == null) continue;
                if (!ReferenceEquals(child.Parent, s)) continue; // 确保是直接子节点

                Wire(child, m, visited); // 向子节点递归
            }
        }
    }

}
