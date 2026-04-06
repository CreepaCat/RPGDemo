using System;
using System.Collections.Generic;
using UnityEngine;

namespace MyBehaviourTree
{
    /// <summary>
    /// 敌人的一些通用状态
    /// </summary>
    public enum BehaviourState
    {
        Patrol,
        Suspect,
        Chase,
        Attack,
        Death
    }



    /// <summary>
    /// 黑板的内容依赖于树的使用者传入，可由多个使用者共用
    /// 因此，当运行时修改黑板内容时，可能存在竞争条件，需要安排权限优先级
    /// </summary>
    [Serializable]
    public class Blackboard
    {
        [SerializeField] public BehaviourState CurrentState = BehaviourState.Patrol;
        [SerializeField] public List<Transform> WayPoints = new();
        [SerializeField] private List<TransformEntry> transformEntries = new();


        //共用字段
        private static Transform player;



        public Action<BehaviourState> OnStateEnter;
        public Action<BehaviourState> OnStateExit;
        public Action<BehaviourState> OnStateUpdate;


        public Transform GetPlayer() => player;

        public void SetPlayer(Transform playerTrans)
        {
            player = playerTrans;
        }



        public bool TryGetTransform(string key, out Transform value)
        {
            for (int i = 0; i < transformEntries.Count; i++)
            {
                TransformEntry entry = transformEntries[i];
                if (entry != null && entry.key == key)
                {
                    value = entry.value;
                    return value != null;
                }
            }

            value = null;
            return false;
        }

        public void AddTransformEntry(string key, Transform value)
        {

            for (int i = 0; i < transformEntries.Count; i++)
            {
                TransformEntry entry = transformEntries[i];
                if (entry != null && entry.key == key)
                {
                    entry.value = value;
                    return;
                }
            }

            TransformEntry newEntry = new TransformEntry()
            {
                key = key,
                value = value
            };
            transformEntries.Add(newEntry);

        }

    }

    [Serializable]
    public class TransformEntry
    {
        public string key;
        public Transform value;
    }
}
