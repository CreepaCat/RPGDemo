using System;
using System.Collections.Generic;
using System.Linq;
using MyNodeEditor.Extension.Dialogue;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace MyNodeEditorExtension.Dialogue
{
    public class DialogueGraphSaveUtility
    {
        private DialogueTreeViewer _targetGraph = null;
        private DialogueTree _targetDialogueTree = null;

        private List<Edge> Edges => _targetGraph.edges.ToList();
        private List<MyNodeEditor.Extension.Dialogue.DialogueNodeView> NodeViews => _targetGraph.nodes.ToList().Cast<MyNodeEditor.Extension.Dialogue.DialogueNodeView>().ToList();

        public static DialogueGraphSaveUtility GetInstance(DialogueTreeViewer targetGraph)
        {
            return new DialogueGraphSaveUtility()
            {
                _targetGraph = targetGraph,
                _targetDialogueTree = targetGraph.currentNodeTree,
            };
        }
        public void SaveGraph(DialogueTree targetDialogueTree)
        {
            var dialogueTreeToSave = targetDialogueTree;
            if (dialogueTreeToSave == null)
            {
                dialogueTreeToSave = ScriptableObject.CreateInstance<DialogueTree>();
                dialogueTreeToSave.name = Guid.NewGuid().ToString();

            }

            _targetDialogueTree = dialogueTreeToSave;


            if (!SaveNodes(dialogueTreeToSave)) return;


            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                AssetDatabase.CreateFolder("Assets", "Resources");

            UnityEngine.Object loadedAsset = AssetDatabase.LoadAssetAtPath($"Assets/Resources/{dialogueTreeToSave.name}.asset", typeof(DialogueTree));

            if (loadedAsset == null || !AssetDatabase.Contains(loadedAsset))
            {
                AssetDatabase.CreateAsset(dialogueTreeToSave, $"Assets/Resources/{dialogueTreeToSave.name}.asset");
            }
            else
            {
                // Debug.Log("存在同名文件，覆盖保存");
                DialogueTree container = loadedAsset as DialogueTree;
                container.NodeLinks = dialogueTreeToSave.NodeLinks;
                container.NodeDatas = dialogueTreeToSave.NodeDatas;

                EditorUtility.SetDirty(container);
            }

            AssetDatabase.SaveAssets();

        }

        public void RepaintCurrentGraph()
        {
            _targetGraph.SetGraphViewWriteBackSuppressed(true);
            try
            {
                GenerateDialogueNodes();
                ConnectDialogueNodes();
                _targetDialogueTree.RemapEdgeData();
            }
            finally
            {
                _targetGraph.SetGraphViewWriteBackSuppressed(false);
            }
        }



        public void LoadGraph(DialogueTree dialogueTreeToLoad)
        {
            _targetDialogueTree = dialogueTreeToLoad;
            if (dialogueTreeToLoad == null)
            {
                EditorUtility.DisplayDialog("File Not Found", "请先选择加载文件!", "OK");
                return;
            }

            _targetGraph.SetGraphViewWriteBackSuppressed(true);
            try
            {
                ClearGraph();

                GenerateDialogueNodes();
                ConnectDialogueNodes();
                _targetDialogueTree.RemapEdgeData();
            }
            finally
            {
                _targetGraph.SetGraphViewWriteBackSuppressed(false);
            }

        }

        /// <summary>
        /// 清除视图
        /// </summary>
        public void ClearGraph()
        {

            foreach (var perNode in NodeViews)
            {
                //起始节点永远不删除
                if (perNode.NodeData.isEntryPoint) continue;
                //清除连线
                Edges.Where(x => x.input.node == perNode).ToList()
                    .ForEach(edge => _targetGraph.RemoveElement(edge));
                //清除节点
                _targetGraph.RemoveElement(perNode);
            }
        }

        /// <summary>
        ///根据缓存数据创建节点视图 和 选项
        /// </summary>
        private void GenerateDialogueNodes()
        {

            foreach (var perNodeDate in _targetDialogueTree.NodeDatas.Cast<DialogueNodeData>().ToList())
            {
                var tempNodeView = _targetGraph.CreateNodeView(perNodeDate);



                if (perNodeDate.OptionList != null && perNodeDate.OptionList.Count > 0)
                {
                    //生成所有后续选项
                    foreach (var portData in perNodeDate.OptionList)
                    {
                        _targetGraph.AddChoicePort(tempNodeView, portData);

                    }
                }

                tempNodeView.RefreshPorts();
                tempNodeView.RefreshExpandedState();

            }
        }

        /// <summary>
        /// 节点端口连线
        /// </summary>
        private void ConnectDialogueNodes()
        {
            for (var i = 0; i < NodeViews.Count; i++)
            {
                var connections = _targetDialogueTree.NodeLinks.Where(x => x.BaseNodeGUID == NodeViews[i].NodeData.guid).ToList();
                var outputChildCount = NodeViews[i].outputContainer.childCount; // 获取实际子元素数

                for (int k = 0; k < outputChildCount; k++)
                {
                    var port = NodeViews[i].outputContainer[k].Q<Port>();
                    NodeLinkData linkData = null;

                    //从合格缓存中寻找符合条件的连线
                    foreach (var connection in connections)
                    {
                        if (connection.PortName == port.portName)
                        {
                            linkData = connection;
                            break;
                        }
                    }

                    if (linkData == null) continue;


                    var targetNodeGUID = linkData.TargetNodeGUID;
                    var targetNode = NodeViews.FirstOrDefault(x => x.NodeData.guid == targetNodeGUID);
                    if (targetNode == null) continue;
                    LinkNodesTogether(port, (Port)targetNode.inputContainer[0]);
                    targetNode.SetPosition(new Rect(
                        _targetDialogueTree.NodeDatas.First(x => x.guid == targetNodeGUID).position,
                        _targetGraph.DefaultNodeSize));



                }


            }
        }



        private void LinkNodesTogether(Port outputSocket, Port inputSocket)
        {
            if (outputSocket == null) return;
            if (outputSocket.node == inputSocket.node) return;
            var tempEdge = new Edge()
            {
                output = outputSocket,
                input = inputSocket
            };

            tempEdge.input?.Connect(tempEdge);
            tempEdge.output?.Connect(tempEdge);
            _targetGraph.Add(tempEdge);


        }

        /// <summary>
        /// 保存节点
        /// </summary>
        /// <param name="dialogueTree"></param>
        /// <returns></returns>
        private bool SaveNodes(DialogueTree dialogueTree)
        {

            if (!Edges.Any()) return false;
            var connectedSockets = Edges.Where(x => x.input.node != null).ToArray();

            //保存节点前先清空旧节点缓存
            dialogueTree.NodeLinks.Clear();
            dialogueTree.NodeDatas.Clear();

            var rootNodeGuid = string.Empty;

            for (var i = 0; i < connectedSockets.Count(); i++)
            {
                var outputNode = (connectedSockets[i].output.node as MyNodeEditor.Extension.Dialogue.DialogueNodeView);
                var inputNode = (connectedSockets[i].input.node as MyNodeEditor.Extension.Dialogue.DialogueNodeView);

                //同时保存根节点
                if (outputNode.NodeData.guid == "Start")
                {
                    rootNodeGuid = inputNode.NodeData.guid;
                }

                dialogueTree.NodeLinks.Add(new NodeLinkData
                {
                    BaseNodeGUID = outputNode.NodeData.guid,
                    PortName = connectedSockets[i].output.portName,
                    TargetNodeGUID = inputNode.NodeData.guid,
                });
            }

            foreach (var node in NodeViews.Where(node => !node.NodeData.isEntryPoint))
            {
                // Debug.Log("保存节点" + node.NodeData.guid);
                dialogueTree.NodeDatas.Add(node.NodeData);
                //保存根节点
                if (node.NodeData.guid == rootNodeGuid)
                {
                    dialogueTree.rootNodeData = node.NodeData;
                }
            }

            if (rootNodeGuid == string.Empty)
            {
                dialogueTree.rootNodeData = null;
            }

            return true;
        }


    }
}
