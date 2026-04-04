using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace MyBehaviourTree
{
    [CreateAssetMenu(menuName = "MyBehaviourTree/new tree", fileName = "New BehaviourTree")]
    public class BehaviourTree : ScriptableObject
    {

        public Node rootNode;
        public Node.State treeState = Node.State.Running;
        public List<Node> nodes = new();

        public Blackboard blackboard = new();

        public Node.State Update()
        {

            // return rootNode.Update();
            if (rootNode.state == Node.State.Running)
            {
                treeState = rootNode.Update();
            }
            return treeState;

        }

        public void Bind(BehaviourTreeRunner runner)
        {
            if (runner == null || rootNode == null)
            {
                return;
            }
            blackboard = runner.blackboard;

            BehaviourTreeContext context = new BehaviourTreeContext(runner, runner.blackboard);
            Traverse(rootNode, n => n.Bind(context));
        }

        /// <summary>
        /// 克隆方法，用于创建运行时SO文件，确保在运行时的修改不会影响到原文件
        /// </summary>
        /// <returns></returns>
        public BehaviourTree Clone()
        {
            BehaviourTree virtualTree = Instantiate(this);
            //因为Node的克隆方法保证了自己的子节点也被克隆
            //因此克隆根节点整棵树的节点都被克隆了
            virtualTree.rootNode = rootNode.Clone();
            virtualTree.nodes = new List<Node>();
            Traverse(virtualTree.rootNode, (n) =>
            {
                virtualTree.nodes.Add(n);
            });
            return virtualTree;
        }

        public void Traverse(Node node, System.Action<Node> visiter)
        {
            if (node)
            {
                visiter?.Invoke(node);
                var children = GetChildren(node);
                children.ForEach((c) =>
                {
                    Traverse(c, visiter); //递归遍历整颗树的节点
                });
            }

        }


        #region  Editor


        public Node CreateNode(System.Type type)
        {
            var node = ScriptableObject.CreateInstance(type) as Node;
            node.name = type.Name;
            node.guid = GUID.Generate().ToString();


            Undo.RecordObject(this, "BehaviourTree (Create Node)");//节点的创建记录在本树上
            //先记录原状态，再添加
            nodes.Add(node);
            if (!Application.isPlaying) //仅在非运行状态可创建节点
            {

                AssetDatabase.AddObjectToAsset(node, this);
            }
            //用特殊的方法记录对应节点的创建
            Undo.RegisterCreatedObjectUndo(node, "BehaviourTree (Create Node)");

            AssetDatabase.SaveAssets();

            return node;
        }

        public void DeleteNode(Node node)
        {
            Undo.RecordObject(this, "BehaviourTree (Delete Node)");//节点的删除记录在本树上
            nodes.Remove(node);

            //AssetDatabase.RemoveObjectFromAsset(node);
            Undo.DestroyObjectImmediate(node);

            AssetDatabase.SaveAssets();


        }

        public static List<Node> GetChildren(Node n)
        {
            List<Node> result = new();
            switch (n)
            {
                case RootNode root:
                    if (root.child != null)
                        result.Add(root.child);
                    break;
                case CompositeNode cn:
                    result.AddRange(cn.children);
                    break;
                case DecoratorNode dn:
                    if (dn.child != null)
                        result.Add(dn.child);
                    break;

                default:
                    break;
            }
            return result;
        }

        public void RemoveChild(Node parent, Node child)
        {



            if (parent is DecoratorNode decorator && decorator != null)
            {
                Undo.RecordObject(decorator, "BehaviourTree (Remove Child)");
                decorator.child = null;
                EditorUtility.SetDirty(decorator);
            }
            else if (parent is CompositeNode composite && composite != null)
            {
                Undo.RecordObject(composite, "BehaviourTree (Remove Child)");
                composite.children.Remove(child); //todo:清除null child
                if (composite is SwitchNode switchNode)
                {
                    switchNode.SyncCasesWithChildren();
                }
                EditorUtility.SetDirty(composite);
            }
            else if (parent is RootNode rootNode && rootNode != null)
            {
                Undo.RecordObject(rootNode, "BehaviourTree (Remove Child)");
                rootNode.child = null;
                EditorUtility.SetDirty(rootNode);
            }





        }

        public void AddChild(Node parent, Node child)
        {

            if (parent is DecoratorNode decorator && decorator != null)
            {
                Undo.RecordObject(decorator, "BehaviourTree (Add Child)");
                decorator.child = child;
                EditorUtility.SetDirty(decorator);
            }
            else if (parent is CompositeNode composite && composite != null)
            {
                Undo.RecordObject(composite, "BehaviourTree (Add Child)");
                composite.children.Add(child);
                if (composite is SwitchNode switchNode)
                {
                    switchNode.SyncCasesWithChildren();
                }
                EditorUtility.SetDirty(composite);
            }
            else if (parent is RootNode rootNode && rootNode != null)
            {
                Undo.RecordObject(rootNode, "BehaviourTree (Add Child)");
                rootNode.child = child;
                EditorUtility.SetDirty(rootNode);
            }


        }
    }
    #endregion
}
