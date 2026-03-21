using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 节点树类，管理该对话树下的所有对话节点
///
/// 数据结构包含根节点、当前运行的节点、当前树的运行状态、所有节点列表
///
/// </summary> <summary>
///
/// </summary>
[CreateAssetMenu(menuName = "Scriptable Objects/NodeTree", fileName = "New NodeTree")]
public class NodeTree : ScriptableObject
{
    public NodeData rootNodeData;
    protected NodeData _runningNodeData;
    public NodeData.State treeState = NodeData.State.Waiting;

    [HideInInspector]
    public List<NodeLinkData> NodeLinks = new List<NodeLinkData>();

    public List<NodeData> NodeDatas = new List<NodeData>();

    public NodeLinkData GetNodeLink(string portName)
    {
        foreach (var nodeLink in NodeLinks)
        {
            nodeLink.PortName = portName;
            return nodeLink;
        }
        return null;
    }
    public NodeData GetNodeData(string guid)
    {
        foreach (var nodeData in NodeDatas)
        {
            if (nodeData.guid == guid)
            {
                return nodeData;
            }
        }
        return null;
    }

    /// <summary>
    /// 节点树的三个状态：进入、更新、退出
    /// 在节点树的更新方法里 执行当前运行节点的OnUpdate方法
    /// </summary>
    public virtual void Update()
    {
        if (treeState == NodeData.State.Running && _runningNodeData?.state == NodeData.State.Running)
        {
            _runningNodeData = _runningNodeData.OnUpdate();
        }
    }

    /// <summary>
    /// 开始执行树
    /// </summary>
    public virtual void OnTreeStart()
    {
        treeState = NodeData.State.Running;
        RestNodesState();

        //同时需要将运行节点的状态设为Running
        _runningNodeData = rootNodeData;
        _runningNodeData.state = NodeData.State.Running;
    }
    public virtual void OnTreeEnd()
    {
        treeState = NodeData.State.Waiting;
        //退出时，重置所有node的状态，防止跳过中断的对话
        //  RestNodesState();
        if (_runningNodeData != null)
        {
            _runningNodeData.state = NodeData.State.Waiting;
        }
    }

    public void RestNodesState()
    {
        NodeDatas.ForEach(node =>
       {
           node.state = NodeData.State.Waiting;
           node.started = false;
       });

    }

#if UNITY_EDITOR
    /// <summary>
    /// 创建节点
    /// </summary>
    /// <param name="type"></param>
    public NodeData CreateNode(System.Type type)
    {


        NodeData nodeData = ScriptableObject.CreateInstance(type) as NodeData;
        nodeData.name = type.Name;
        nodeData.guid = GUID.Generate().ToString();
        nodeData.tree = this;

        if (!Application.isPlaying)
        {
            //将节点添加到节点树，非运行时才执行
            AssetDatabase.AddObjectToAsset(nodeData, this);
        }

        //用特定的方法记录被创建的节点
        Undo.RegisterCreatedObjectUndo(nodeData, "Node Tree(CreateNode)");
        Undo.RecordObject(this, "Node Tree(CreateNode)");

        NodeDatas.Add(nodeData);
        AssetDatabase.SaveAssets();  //保存所有未保存的资产

        return nodeData;
    }

    public void RemoveNullNodes()
    {
        List<NodeData> temp = new List<NodeData>();
        foreach (var nodeData in NodeDatas)
        {
            if (nodeData == null) continue;
            temp.Add(nodeData);
        }
        NodeDatas = temp;
    }

    public void RemoveNullLinkData()
    {
        List<NodeLinkData> temp = new List<NodeLinkData>();
        foreach (var linkData in NodeLinks)
        {
            if (linkData == null) continue;
            temp.Add(linkData);
        }
        NodeLinks = temp;
    }

    /// <summary>
    /// 删除节点，返回被删除的节点
    /// </summary>
    /// <param name="nodeData"></param>
    /// <returns></returns>
    public NodeData DeleteNode(NodeData nodeData)
    {
        Undo.RecordObject(this, "Node Tree(DeleteNode)");
        // foreach (var child in nodeData.children)
        // {
        //     RemoveChildRalationship(nodeData, child);
        // }
        NodeDatas.Remove(nodeData);
        RemoveNullNodes();
        //AssetDatabase.RemoveObjectFromAsset(node);

        //用特定的方法记录被删除的节点
        Undo.DestroyObjectImmediate(nodeData);

        AssetDatabase.SaveAssets();
        return nodeData;
    }

    public void DisconnectChild(NodeData parent, NodeData child)
    {
        Undo.RecordObject(parent, "Node Tree(RemoveChild)");
        //同时移除缓存的连线信息
        NodeLinks.RemoveAll(l => l.BaseNodeGUID == parent.guid && l.TargetNodeGUID == child.guid);
        RemoveNullLinkData();

        //检查更新根节点
        if (parent.guid == "Start")
        {
            rootNodeData = null;
        }

        parent.DisconnectChild(child.guid);
        // parent.RemoveChild(child);


        //Undo追踪修改状态无法保存到SO文件，要通过SetDirty来通知Unity保存
        EditorUtility.SetDirty(parent);
    }
    public void LinkChild(NodeData parent, string portName, NodeData child)
    {
        Undo.RecordObject(parent, "Node Tree(AddChild)");
        // parent.AddChild(child);
        parent.LinkChild(child.guid, portName);

        //检查更新根节点
        if (parent.guid == "Start")
        {
            rootNodeData = NodeDatas.Find(x => x.guid == child.guid);
        }

        //同时添加进缓存
        NodeLinks.Add(new NodeLinkData()
        {
            BaseNodeGUID = parent.guid,
            PortName = portName,
            TargetNodeGUID = child.guid,
        });

        EditorUtility.SetDirty(parent);
    }

    /// <summary>
    /// 将节点连线关系映射到对话树容器
    /// </summary>
    public void RemapEdgeData()
    {
        foreach (var baseNode in NodeDatas)
        {
            //清除旧关系
            baseNode.children.Clear();
            var connections = NodeLinks.Where(x => x.BaseNodeGUID == baseNode.guid).ToList();
            // Debug.Log("<MappingEdgeData>" +connections.Count);

            //从合格缓存中寻找符合条件的连线
            NodeLinkData linkData = connections.FirstOrDefault(x => x.BaseNodeGUID == baseNode.guid);
            if (linkData == null) continue;

            foreach (var edge in connections)
            {
                var targetNodeGUID = edge.TargetNodeGUID;
                var targetNode = NodeDatas.FirstOrDefault(x => x.guid == targetNodeGUID);
                LinkChild(baseNode, edge.PortName, targetNode);
            }
        }

        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }


    public void ClearChildren()
    {
        //Undo.RecordObject(this, "Node Tree(DeleteNode)");
        NodeDatas.Clear();
        //AssetDatabase.RemoveObjectFromAsset(node);

        //用特定的方法记录被删除的节点
        // Undo.DestroyObjectImmediate(nodeData);

        AssetDatabase.SaveAssets();
    }

    /// <summary>
    /// 获取对应父节点的子节点
    /// </summary>
    public List<NodeData> GetChildren(NodeData parent)
    {
        List<NodeData> resultChildren = new List<NodeData>();
        resultChildren.AddRange(parent.GetChildren());
        // if (parent is SingleNodeData)
        // {
        //     resultChildren.Add((parent as SingleNodeData).GetChild());
        // }
        // else if (parent is BranchNodeData)
        // {
        //     resultChildren.AddRange((parent as BranchNodeData).GetChildren());
        // }
        return resultChildren;
    }

#endif
}
