using MyNodeEditorExtension.Dialogue;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;

namespace MyNodeEditor.Extension.Dialogue
{
    
public class DialogueGraphy : EditorWindow
{
    public DialogueTreeViewer dialogueTreeViewer;
    public InspectorViewer inspectorViewer;
    
    //CACHE
    private static DialogueTree _cachedSelectedNodeTree = null;
    private MiniMap _minimap = null;
    
  //  private VisualTreeAsset m_VisualTreeAsset = null;

    [MenuItem("Tools/DialogueGraphy")]
    public static void ShowGraphWindow()
    {
        DialogueGraphy wnd = GetWindow<DialogueGraphy>();
        wnd.titleContent = new GUIContent("DialogueGraph");
    }
    /// <summary>
    /// 双击资源打开窗口
    /// </summary>
    /// <param name="instanceId"></param>
    /// <param name="line"></param>
    /// <returns></returns>
    [OnOpenAsset]
    public static bool OnOpenAsset(int instanceId, int line)
    {
        var currentSelectedNodeTree = Selection.activeObject as DialogueTree;
        if (currentSelectedNodeTree == null || ReferenceEquals(currentSelectedNodeTree, _cachedSelectedNodeTree))
        {
            return false;
        }
        
        _cachedSelectedNodeTree = currentSelectedNodeTree;
        ShowGraphWindow();
        return true;
        
    
    }

    private void OnEnable()
    {
        //对不同的窗口，需要加载不同的根visual资源
       
       ConstructViewers();
       GenerateMinimap();
       dialogueTreeViewer.PopulateView(_cachedSelectedNodeTree);
       
       BindUIEvents();
       
       
        
    }
    // Don't forget to unregister in OnDisable to avoid leaks
    private void OnDisable()
    {
        
        _cachedSelectedNodeTree = null;
       
        if (dialogueTreeViewer != null)
        {
            dialogueTreeViewer.currentNodeTree = null;
            dialogueTreeViewer.UnregisterCallback<GeometryChangedEvent>(OnGraphViewGeometryChanged);
        }
    }
 



    public void ConstructViewers()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;
        
        //将自定义UI布局克隆进root visual
        var visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/NodeEditor/Extension/Dialogue/Editor/UI/DialogueNodeEditor.uxml");
        visualTreeAsset.CloneTree(root);
        
        //添加样式
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/NodeEditor/Extension/Dialogue/Editor/UI/DialogueNodeEditor.uss");
        root.styleSheets.Add(styleSheet);

        dialogueTreeViewer = root.Q<DialogueTreeViewer>();
        inspectorViewer = root.Q<InspectorViewer>();
        dialogueTreeViewer.InspectorViewer =  inspectorViewer;
        
    
       
        //将视图树的节点选中事件与NodeEditor里的方法绑定
        dialogueTreeViewer.OnNodeSelected += OnNodeSelectionChanged;

    }
  
    /// <summary>
    /// 绑定UI交互事件
    /// </summary>
    private void BindUIEvents()
    {
        rootVisualElement.Q<ObjectField>("ObjectField").objectType = typeof(DialogueTree);
        
        //保存资源
        rootVisualElement.Q<Button>("SaveDataButton").clickable.clicked += () =>
        {
            if (dialogueTreeViewer.currentNodeTree == null)
            {
                EditorUtility.DisplayDialog("Error", "请先创建节点", "OK");
                return;
            }
            SaveData();
            Debug.Log("Save dialogue");
        };
        //加载资源，并缓存
        rootVisualElement.Q<Button>("LoadDataButton").clickable.clicked += () =>
        {
            var obj = rootVisualElement.Q<ObjectField>("ObjectField");
          
            if (obj.value == null)
            {
                EditorUtility.DisplayDialog("Error", "请先选择要加载的文件", "OK");
                return;
            }
            
            var nodeTree = obj.value as DialogueTree;
            dialogueTreeViewer.currentNodeTree = nodeTree; 

            LoadData(nodeTree);
            Debug.Log("Load dialogue");
        };
        
        //创建节点
        rootVisualElement.Q<Button>("CreateNodeButton").clickable.clicked += () =>
        {
            dialogueTreeViewer.CreateNode(typeof(DialogueNodeData));
        };

        rootVisualElement.Q<Button>("CreateTreeButton").clickable.clicked += () =>
        {
            var newTree = dialogueTreeViewer.CreateTree();
           Selection.activeObject = newTree;
            //LoadData(newTree);
            //将目前的资源焦点放在新建文件上
            
        };

    }

    private void SaveData()
    {
        DialogueGraphSaveUtility.GetInstance(dialogueTreeViewer).SaveGraph(dialogueTreeViewer.currentNodeTree);
        //inspectorViewer.UpdateSelection();
    }

    private void LoadData(DialogueTree newTree)
    {
        if (newTree == null)
        {
            EditorUtility.DisplayDialog("File Not Found", "请先选择加载文件!", "OK");
            return;
                
        }
        //todo:加载前提醒保存当前编辑文件
 
        _cachedSelectedNodeTree = newTree;
        dialogueTreeViewer.PopulateView(newTree);
        //重置节点检视面板
        inspectorViewer.UpdateSelection();
       
    }
    private void GenerateMinimap()
    {
        _minimap = new MiniMap { anchored = true };

        // 设置minimap位置
        UpdateMinimapPosition();

        dialogueTreeViewer.Add(_minimap);

        // 注册窗口缩放事件回调
        dialogueTreeViewer.RegisterCallback<GeometryChangedEvent>(OnGraphViewGeometryChanged);
    }
    /// <summary>
    /// 当窗口缩放时，更新minimap位置
    /// </summary>
    /// <param name="evt"></param>
    private void OnGraphViewGeometryChanged(GeometryChangedEvent evt)
    {
        UpdateMinimapPosition();
    }

    private void UpdateMinimapPosition()
    {
        var x = dialogueTreeViewer.layout.width - 210; // 200 width + 10 padding
        var y = 30;
        _minimap.SetPosition(new Rect(x, y, 200, 140));
    }


    /// <summary>
    /// 刷新Inspector面板
    /// </summary>
    /// <param name="view"></param>
    private void OnNodeSelectionChanged(NodeView view)
    {
        inspectorViewer.UpdateSelection(view);
    }

    private void OnSelectionChange()
    {
        //当选择发生改变时，判断是否是一个节点树
        DialogueTree nodeTree = Selection.activeObject as DialogueTree;

        //只有当条件符合时才刷新视图
        if (nodeTree != null && nodeTree != dialogueTreeViewer.currentNodeTree)
        {
            //调用对应方法显示新树
            dialogueTreeViewer.PopulateView(nodeTree);
            //dialogueTreeViewer?.UpdateNodeStates();
           
        }

    }

    /// <summary>
    /// 每10帧执行一次的面板刷新回调
    /// </summary>
    // private void OnInspectorUpdate()
    // {
    //     dialogueTreeViewer?.UpdateNodeStates();
    // }
}
}

