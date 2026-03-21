using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 抽象类Node
/// 包含节点状态枚举、节点是否开始布尔值、子节点列表
/// </summary>
///
// [CreateAssetMenu(menuName = "DialogueNode", fileName = "New Node", order = 0)]
public abstract class NodeData : ScriptableObject
{
    public enum State { Running, Waiting }
    public State state = State.Waiting;
    public bool started = false;


    [HideInInspector]
    public string guid;  //唯一ID
    [HideInInspector]
    public bool isEntryPoint = false; //是否为起始节点
    [HideInInspector]
    public Vector2 position;  //显示位置
    [HideInInspector]
    public NodeTree tree; //所属树

    // [HideInInspector]
    public List<NodeData> children = new List<NodeData>();


    public void AddChild(NodeData child)
    {
        if (children.Contains(child)) return;
        children.Add(child);
    }

    public void RemoveChild(NodeData child)
    {
        Debug.Log($"{this.guid} removing child {child.guid}");
        children.Remove(child);
        RemoveNullChildren();

    }

    public List<NodeData> GetChildren()
    {
        return children;
    }

    public void ClearChildren()
    {
        children.Clear();
    }

    public void RemoveNullChildren()
    {
        List<NodeData> tempList = new List<NodeData>();
        foreach (NodeData child in children)
        {
            if (child == null) continue;
            tempList.Add(child);
        }
        children = tempList;
    }




    /// <summary>
    /// 节点的三个状态：进入、更新、退出
    ///
    /// 节点的更新方法，每次更新获取更新后的当前节点
    /// </summary>
    /// <returns></returns>
    public NodeData OnUpdate()
    {
        //update时若节点没有开始，执行进入方法
        if (!started)
        {
            OnStart();
            started = true;
        }
        //获取逻辑更新后的节点
        NodeData currentNodeData = OnLogicUpdate();

        //update时若节点不为Running状态，执行退出方法
        if (state != State.Running)
        {
            OnStop();
            started = false;
        }
        return currentNodeData;
    }

    /// <summary>
    /// 每次进入此节点时执行
    /// </summary>
    public abstract void OnStart();

    /// <summary>
    /// 节点的逻辑更新方法
    /// </summary>
    public abstract NodeData OnLogicUpdate();
    /// <summary>
    /// 每次退出此节点时执行
    /// </summary>
    public abstract void OnStop();

    /// <summary>
    /// 当移除父子关系时，移除选项缓存
    /// </summary>
    /// <param name="guid"></param>
    public abstract void DisconnectChild(string guid);

    public abstract void LinkChild(string guid, string portName);

}
