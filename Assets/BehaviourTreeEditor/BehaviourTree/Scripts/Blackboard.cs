using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MyBehaviourTree
{
    /// <summary>
    /// 黑板的内容依赖于树的使用者传入，可由多个使用者共用
    /// 因此，当运行时修改黑板内容时，可能存在竞争条件，需要安排权限优先级
    /// </summary>
    [Serializable]
    public class Blackboard
    {
        [SerializeField] private List<TransformEntry> transformEntries = new();
        [SerializeField] private List<StringEntry> stringEntries = new();

        //共用字段
        [ShowInInspector] private static Transform player;


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

        public bool TryGetString(string key, out string value)
        {
            for (int i = 0; i < stringEntries.Count; i++)
            {
                StringEntry entry = stringEntries[i];
                if (entry != null && entry.key == key)
                {
                    value = entry.value;
                    return true;
                }
            }

            value = string.Empty;
            return false;
        }

        public void SetString(string key, string value)
        {
            for (int i = 0; i < stringEntries.Count; i++)
            {
                StringEntry entry = stringEntries[i];
                if (entry != null && entry.key == key)
                {
                    entry.value = value;
                    return;
                }
            }

            stringEntries.Add(new StringEntry { key = key, value = value });
        }
    }

    [Serializable]
    public class TransformEntry
    {
        public string key;
        public Transform value;
    }

    [Serializable]
    public class StringEntry
    {
        public string key;
        public string value;
    }
}
