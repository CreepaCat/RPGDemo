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
        private ObjectField _treeObjectField;
        private Button _saveDataButton;
        private Button _loadDataButton;
        private Button _createNodeButton;
        private Button _createTreeButton;
        private bool _uiEventsBound = false;

        //CACHE
        private static DialogueTree _cachedSelectedNodeTree = null;
        private MiniMap _minimap = null;

        //  private VisualTreeAsset m_VisualTreeAsset = null;

        [MenuItem("MyTools/对话编辑器")]
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
            rootVisualElement.Clear();
            ConstructViewers();
            GenerateMinimap();
            dialogueTreeViewer.PopulateView(_cachedSelectedNodeTree);

            BindUIEvents();



        }
        // Don't forget to unregister in OnDisable to avoid leaks
        private void OnDisable()
        {
            CleanupWindow();
            _cachedSelectedNodeTree = null;
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
            dialogueTreeViewer.InspectorViewer = inspectorViewer;



            //将视图树的节点选中事件与NodeEditor里的方法绑定
            dialogueTreeViewer.OnNodeSelected += OnNodeSelectionChanged;

        }

        /// <summary>
        /// 绑定UI交互事件
        /// </summary>
        private void BindUIEvents()
        {
            if (_uiEventsBound) return;

            _treeObjectField = rootVisualElement.Q<ObjectField>("ObjectField");
            _saveDataButton = rootVisualElement.Q<Button>("SaveDataButton");
            _loadDataButton = rootVisualElement.Q<Button>("LoadDataButton");
            _createNodeButton = rootVisualElement.Q<Button>("CreateNodeButton");
            _createTreeButton = rootVisualElement.Q<Button>("CreateTreeButton");

            _treeObjectField.objectType = typeof(DialogueTree);

            _saveDataButton.clickable.clicked += OnSaveDataClicked;
            _loadDataButton.clickable.clicked += OnLoadDataClicked;
            _createNodeButton.clickable.clicked += OnCreateNodeClicked;
            _createTreeButton.clickable.clicked += OnCreateTreeClicked;
            _uiEventsBound = true;

        }

        private void UnbindUIEvents()
        {
            if (!_uiEventsBound) return;

            if (_saveDataButton != null) _saveDataButton.clickable.clicked -= OnSaveDataClicked;
            if (_loadDataButton != null) _loadDataButton.clickable.clicked -= OnLoadDataClicked;
            if (_createNodeButton != null) _createNodeButton.clickable.clicked -= OnCreateNodeClicked;
            if (_createTreeButton != null) _createTreeButton.clickable.clicked -= OnCreateTreeClicked;

            _treeObjectField = null;
            _saveDataButton = null;
            _loadDataButton = null;
            _createNodeButton = null;
            _createTreeButton = null;
            _uiEventsBound = false;
        }

        private void OnSaveDataClicked()
        {
            if (dialogueTreeViewer.currentNodeTree == null)
            {
                EditorUtility.DisplayDialog("Error", "请先创建节点", "OK");
                return;
            }
            SaveData();
            Debug.Log("Save dialogue");
        }

        private void OnLoadDataClicked()
        {
            if (_treeObjectField == null || _treeObjectField.value == null)
            {
                EditorUtility.DisplayDialog("Error", "请先选择要加载的文件", "OK");
                return;
            }

            var nodeTree = _treeObjectField.value as DialogueTree;
            dialogueTreeViewer.currentNodeTree = nodeTree;

            LoadData(nodeTree);
            Debug.Log("Load dialogue");
        }

        private void OnCreateNodeClicked()
        {
            dialogueTreeViewer.CreateNode(typeof(DialogueNodeData));
        }

        private void OnCreateTreeClicked()
        {
            var newTree = dialogueTreeViewer.CreateTree();
            Selection.activeObject = newTree;
            LoadData(newTree);
            //将目前的资源焦点放在新建文件上
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

        private void CleanupWindow()
        {
            UnbindUIEvents();

            if (dialogueTreeViewer != null)
            {
                dialogueTreeViewer.OnNodeSelected -= OnNodeSelectionChanged;
                dialogueTreeViewer.UnregisterCallback<GeometryChangedEvent>(OnGraphViewGeometryChanged);
                dialogueTreeViewer.currentNodeTree = null;
                dialogueTreeViewer.DisposeViewer();
            }

            _minimap = null;
            rootVisualElement.Clear();
        }

        private void OnSelectionChange()
        {
            if (dialogueTreeViewer == null) return;
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
