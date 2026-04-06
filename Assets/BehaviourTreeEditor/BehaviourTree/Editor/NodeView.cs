using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using System;
using UnityEditor.UIElements;
namespace MyBehaviourTree
{
    public class NodeView : UnityEditor.Experimental.GraphView.Node
    {
        public Node node;
        public Port input;
        public Port output;

        public Action<NodeView> OnNodeSelected;
        public NodeView(Node node) : base("Assets/BehaviourTreeEditor/BehaviourTree/Editor/NodeView.uxml")
        {
            this.node = node;
            this.title = node?.name/*.Replace("Node", "")*/;
            this.viewDataKey = node.guid;

            style.left = node.position.x;
            style.top = node.position.y;
            CreateInputPorts();
            CreateOutputPorts();

            SetNodeViewClass(node);

            var serializedNode = new SerializedObject(node);
            Label descriptionLabel = this.Q<Label>("description");
            descriptionLabel.bindingPath = "description";
            descriptionLabel.Bind(serializedNode);

        }

        private void SetNodeViewClass(Node node)
        {
            if (node is ActionNode)
            {
                this.AddToClassList("action");
            }
            else if (node is RootNode)
            {
                AddToClassList("root");
            }
            else if (node is CompositeNode)
            {
                AddToClassList("composite");
            }
            else if (node is DecoratorNode)
            {
                AddToClassList("decorator");
            }
        }

        private void CreateOutputPorts()
        {
            if (node is ActionNode)
            {
                // no op
            }
            else if (node is CompositeNode)
            {
                output = CreatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi);
            }
            else if (node is DecoratorNode)
            {
                output = CreatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single);
            }
            else if (node is RootNode)
            {
                output = CreatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single);
            }

            if (output != null)
            {
                output.portName = "";
                Label label = output.Q<Label>("type"); //隐藏type标签显示，防止占位使端口偏移
                if (label != null)
                {
                    label.style.display = DisplayStyle.None;
                }


                outputContainer.Add(output);
            }
        }

        private void CreateInputPorts()
        {
            if (node is RootNode) //根节点没有进入端口
                return;
            input = CreatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single);

            if (input != null)
            {
                input.portName = "";
                Label label = input.Q<Label>("type");
                if (label != null)
                {
                    label.style.display = DisplayStyle.None;
                }
                inputContainer.Add(input);
            }
        }


        private Port CreatePort(Orientation orientation, Direction direction, Port.Capacity capacity)
        {
            return InstantiatePort(orientation, direction, capacity, typeof(bool));

        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            Undo.RecordObject(node, "Behaviour Tree (Set Position)");
            node.position.x = newPos.xMin;
            node.position.y = newPos.yMin;
            EditorUtility.SetDirty(node);
        }

        public override void OnSelected()
        {
            base.OnSelected();
            OnNodeSelected?.Invoke(this);
        }


        /// <summary>
        /// 将composite类的Node的子节点按从左到右的顺序排序
        /// </summary>
        public void SortChildren()
        {
            CompositeNode cn = node as CompositeNode;
            if (cn && !(cn is SwitchNode))
            {
                cn.children.Sort(SortByHorizontalPosition);
            }
        }

        private int SortByHorizontalPosition(Node left, Node right)
        {
            //List的Sort算法默认由小到大排列
            return left.position.x < right.position.x ? -1 : 1;
        }

        public void UpdateStateView()
        {
            RemoveFromClassList("running");
            RemoveFromClassList("success");
            RemoveFromClassList("failure");
            switch (node.state)
            {
                case Node.State.Running:
                    if (node.started)
                    {
                        AddToClassList("running");
                    }
                    break;
                case Node.State.Success:
                    AddToClassList("success");
                    break;
                case Node.State.Failure:
                    AddToClassList("failure");
                    break;
            }
        }
    }
}
