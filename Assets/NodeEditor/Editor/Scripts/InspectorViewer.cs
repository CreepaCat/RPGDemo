using System;
using MyNodeEditor;
using UnityEditor;
using UnityEngine.UIElements;

[UxmlElement]
public partial class InspectorViewer : VisualElement
{
    //代表这是一个自定义UIToolkit控件类
   // [Obsolete]
   // public new class UxmlFactory : UxmlFactory<InspectorViewer, VisualElement.UxmlTraits> { }

    Editor editor;
    //节点选中事件
    

    public InspectorViewer()
    {
       

    }

    /// <summary>
    /// 选中节点时更新检视面板
    /// </summary>
    /// <param name="obleteNodeView"></param>
    // internal void UpdateSelection(ObleteNodeView obleteNodeView)
    // {
    //     //先清除之前的元素
    //     Clear();
    //     UnityEngine.Object.DestroyImmediate(editor);
    //     if(obleteNodeView == null) return;
    //
    //     //对选中的节点创建editor
    //     editor = Editor.CreateEditor(obleteNodeView.NodeData);
    //     IMGUIContainer container = new IMGUIContainer(() =>
    //     {
    //         if (editor.target)
    //         {
    //             editor.OnInspectorGUI(); //创建自定义inspector面板
    //         }
    //     });
    //     Add(container); //将该容器添加到此VisualElement里
    //     
    //  
    // }
    
    internal void UpdateSelection(NodeView nodeView = null)
    {
        //先清除之前的元素
        Clear();
        UnityEngine.Object.DestroyImmediate(editor);
        if(nodeView == null) return;
    
        //对选中的节点创建editor
        editor = Editor.CreateEditor(nodeView.NodeData);
        IMGUIContainer container = new IMGUIContainer(() =>
        {
            if (editor.target)
            {
                editor.OnInspectorGUI(); //创建自定义inspector面板
            }
        });
        Add(container); //将该容器添加到此VisualElement里
        
     
    }
}



