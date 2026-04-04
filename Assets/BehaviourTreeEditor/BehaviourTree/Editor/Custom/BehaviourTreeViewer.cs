using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using System;
using System.Linq;
using System.Collections.Generic;

namespace MyBehaviourTree
{
    [UxmlElement]
    public partial class BehaviourTreeViewer : GraphView
    {

        public Action<NodeView> OnNodeSelected;

        public BehaviourTree tree;

        public BehaviourTreeViewer()
        {
            Insert(0, new GridBackground()); //网格背景
            this.AddManipulator(new ContentZoomer());  //窗口缩放
            this.AddManipulator(new ContentDragger()); //窗口拖动
            this.AddManipulator(new SelectionDragger()); //选择拖动
            this.AddManipulator(new RectangleSelector()); //矩形选择框
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);



            //读取样式并添加到root
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/BehaviourTreeEditor/BehaviourTree/Editor/BehaviourTreeEditor.uss");
            styleSheets.Add(styleSheet);

            Undo.undoRedoPerformed += OnUndoRedo;
        }

        private void OnUndoRedo()
        {
            PopulateView(tree);
            AssetDatabase.SaveAssets();
        }

        public void PopulateView(BehaviourTree tree)
        {
            if (tree == null) return;
            this.tree = tree;

            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements.ToList());
            graphViewChanged += OnGraphViewChanged;

            if (tree.rootNode == null)
            {
                tree.rootNode = tree.CreateNode(typeof(RootNode)) as RootNode;
                EditorUtility.SetDirty(tree);
                AssetDatabase.SaveAssets();
            }

            // 创建节点view
            tree.nodes.ForEach(n => CreateNodeView(n));

            // 创建连线
            tree.nodes.ForEach(n =>
            {
                var children = BehaviourTree.GetChildren(n);
                children.ForEach(c =>
                {
                    NodeView parentView = FindNodeView(n);
                    NodeView childView = FindNodeView(c);

                    Edge edge = parentView.output.ConnectTo(childView.input);
                    AddElement(edge);
                });
            });
        }

        private NodeView FindNodeView(Node node)
        {
            return GetNodeByGuid(node.guid) as NodeView;
        }

        private void CreateNodeView(Node n)
        {
            NodeView nodeView = new NodeView(n)
            {
                OnNodeSelected = OnNodeSelected //委托串联
            };

            //todo:将创建的新node置于窗口中间
            AddElement(nodeView);

        }

        private void CreateNode(System.Type type)
        {
            Node node = tree.CreateNode(type);
            CreateNodeView(node);
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {

            return ports.ToList().Where(
                //出口与入口方向不能一致
                endPort => endPort.direction != startPort.direction &&
                //不能同节点自连
                endPort.node != startPort.node
            ).ToList();
        }

        public void UpdateNodeStateView()
        {
            nodes.ForEach((n) =>
            {
                NodeView node = n as NodeView;
                if (node != null)
                    node.UpdateStateView();
            });
        }

        #region 事件函数

        /// <summary>
        /// 重写鼠标右键菜单
        /// </summary>
        /// <param name="evt"></param>
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            {
                var types = TypeCache.GetTypesDerivedFrom<ActionNode>();
                foreach (var type in types)
                {
                    evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (a) =>
                    {
                        CreateNode(type);
                    });
                }
            }
            {
                var types = TypeCache.GetTypesDerivedFrom<DecoratorNode>();
                foreach (var type in types)
                {
                    evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (a) =>
                    {
                        CreateNode(type);
                    });
                }
            }
            {
                var types = TypeCache.GetTypesDerivedFrom<CompositeNode>();
                foreach (var type in types)
                {
                    evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (a) =>
                    {
                        CreateNode(type);
                    });
                }
            }
            //  base.BuildContextualMenu(evt);

        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            if (graphViewChange.elementsToRemove != null)
            {
                graphViewChange.elementsToRemove.ForEach(elem =>
                {
                    NodeView nodeView = elem as NodeView;
                    if (nodeView != null)
                    {
                        tree.DeleteNode(nodeView.node);
                    }

                    Edge edge = elem as Edge;
                    if (edge != null)
                    {
                        NodeView parentView = edge.output.node as NodeView;
                        NodeView childView = edge.input.node as NodeView;
                        tree.RemoveChild(parentView.node, childView.node);
                    }
                });
            }

            if (graphViewChange.edgesToCreate != null)
            {
                graphViewChange.edgesToCreate.ForEach(edge =>
                {
                    NodeView parentView = edge.output.node as NodeView;
                    NodeView childView = edge.input.node as NodeView;
                    tree.AddChild(parentView.node, childView.node);
                });
            }

            //将composite类的Node的子节点按从左到右的顺序排序
            nodes.ForEach((n) =>
            {
                NodeView view = n as NodeView;
                view.SortChildren();
            });

            return graphViewChange;

        }
    }
    #endregion
}
