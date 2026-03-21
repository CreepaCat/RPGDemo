using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace MyNodeEditor
{
    public class NodeView:UnityEditor.Experimental.GraphView.Node
    {
        public NodeData NodeData;
        
        
        //节点选中事件
        public System.Action<NodeView> OnNodeSelected;
        
    
        //使用自定义样式
        public NodeView(NodeData nodeData)
        {
            //  if(nodeData == null) return;
            this.NodeData = nodeData;
            // this.title = nodeData.name; //title即name
            this.viewDataKey = nodeData.guid;  //viewDataKey即 guid
            
            style.left = nodeData.position.x;
            style.top = nodeData.position.y;
            
            // var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/NodeEditor/Editor/UI/NodeEditor.uss");
            // styleSheets.Add(styleSheet);

        }
        
     
      
        /// <summary>
        /// 可视化节点选中事件回调
        /// </summary>
        public override void OnSelected()
        {
            base.OnSelected();
            if (OnNodeSelected != null)
            {
                OnNodeSelected.Invoke(this);
            }
        }
        
        public override void SetPosition(Rect newPos)
        {
            if (NodeData == null)
            {
                Debug.LogWarning("NodeData is null in SetPosition, skipping Undo and position update.");
                base.SetPosition(newPos); // 只更新视图位置
                return;
            }

            Undo.RecordObject(NodeData, "Node Tree(SetPosition)");
            base.SetPosition(newPos);
            NodeData.position.x = newPos.xMin;
            NodeData.position.y = newPos.yMin;
            EditorUtility.SetDirty(NodeData);
        }

    }
}