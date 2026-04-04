using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace MyBehaviourTree
{
    //这里继承的是VisualElement而非GraphyView,否则IMGUIContainer不会正常显示
    [UxmlElement]
    public partial class InspectorViewer : VisualElement
    {
        Editor editor;
        public InspectorViewer()
        {

        }

        internal void UpdateSelection(NodeView nodeView)
        {
            Debug.Log("UpdateSelection");
            Clear();
            //销毁旧的editor缓存
            UnityEngine.Object.DestroyImmediate(editor);
            //创建新editor缓存
            editor = Editor.CreateEditor(nodeView.node);
            //用IMGUIContainer来显示内容
            IMGUIContainer container = new IMGUIContainer(() =>
            {
                if (editor.target) //防止undo redo时对象丢失报空
                {

                    editor.OnInspectorGUI();
                }
            });

            Add(container);
        }
    }
}
