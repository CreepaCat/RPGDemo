using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
namespace MyBehaviourTree
{


    public class BehaviourTreeEditor : EditorWindow
    {
        BehaviourTreeViewer treeViewer;
        InspectorViewer inspectorViewer;
        IMGUIContainer blackboardViewer;

        //反射
        SerializedObject treeObj;
        SerializedProperty blackboardProperty;

        BehaviourTree _cachedTree = null;

        private MiniMap _minimap = null;

        [MenuItem("MyTools/行为树编辑器")]
        public static void OpenWindow()
        {
            BehaviourTreeEditor wnd = GetWindow<BehaviourTreeEditor>();
            wnd.titleContent = new GUIContent("行为树编辑器");
        }

        /// <summary>
        /// 当编辑器尝试打开一个资产文件时，调用此回调方法
        /// </summary>
        /// <param name="instantId"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        [OnOpenAsset]
        public static bool OnOpenAsset(int instantId, int line)
        {
            if (Selection.activeObject is BehaviourTree)
            {
                OpenWindow();
                return true;

            }
            return false;

        }
        private void OnEnable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

            UpdateCachedTree();
            ConstructViewers();
            GenerateMinimap();

            // 注册窗口缩放事件回调
            treeViewer.RegisterCallback<GeometryChangedEvent>(OnGraphViewGeometryChanged);
            treeViewer.PopulateView(_cachedTree);


        }

        private void OnPlayModeStateChanged(PlayModeStateChange change)
        {
            switch (change)
            {
                case PlayModeStateChange.EnteredEditMode:
                    OnSelectionChange();
                    break;
                case PlayModeStateChange.ExitingEditMode:
                    break;
                case PlayModeStateChange.EnteredPlayMode:
                    OnSelectionChange();
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    break;
            }
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            _cachedTree = null;

            if (treeViewer != null)
            {
                treeViewer.tree = null;
                treeViewer.OnNodeSelected -= OnNodeSelectionChange;
                treeViewer.UnregisterCallback<GeometryChangedEvent>(OnGraphViewGeometryChanged);
            }

            if (blackboardViewer != null)
            {
                blackboardViewer.onGUIHandler -= UpdateBlackboardViewr;
            }

            treeObj?.Dispose();
            treeObj = null;
            blackboardProperty = null;
        }

        public void ConstructViewers()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            //读取UXML并复制到root
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/BehaviourTreeEditor/BehaviourTree/Editor/BehaviourTreeEditor.uxml");
            visualTree.CloneTree(root);

            //读取样式并添加到root
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/BehaviourTreeEditor/BehaviourTree/Editor/BehaviourTreeEditor.uss");
            root.styleSheets.Add(styleSheet);

            treeViewer = root.Q<BehaviourTreeViewer>();
            inspectorViewer = root.Q<InspectorViewer>();
            blackboardViewer = root.Q<IMGUIContainer>();
            //节点选择委托串联
            treeViewer.OnNodeSelected += OnNodeSelectionChange;

            //绘制黑板
            blackboardViewer.onGUIHandler += UpdateBlackboardViewr;


            // OnSelectionChange();
        }

        private void UpdateBlackboardViewr()
        {
            if (_cachedTree == null) { return; }

            treeObj?.Update();
            if (blackboardProperty != null)
                EditorGUILayout.PropertyField(blackboardProperty);
            treeObj?.ApplyModifiedProperties();
        }

        private void GenerateMinimap()
        {
            _minimap = new MiniMap { anchored = true };

            // 设置minimap位置
            UpdateMinimapPosition();

            treeViewer.Add(_minimap);


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
            var x = treeViewer.layout.width - 210; // 200 width + 10 padding
            var y = 30;
            _minimap.SetPosition(new Rect(x, y, 200, 140));
        }

        private void OnSelectionChange()
        {
            // Debug.Log("OnSelectionChange");

            UpdateCachedTree();

            if (_cachedTree)
            {
                if (Application.isPlaying) //运行时打开窗口
                {
                    treeViewer?.PopulateView(_cachedTree);
                }
                else if (AssetDatabase.CanOpenAssetInEditor(_cachedTree.GetEntityId()))
                {
                    treeViewer?.PopulateView(_cachedTree);
                }
            }

        }

        private void UpdateCachedTree()
        {
            BehaviourTree tree = Selection.activeObject as BehaviourTree;
            if (tree == null && Selection.activeGameObject != null)
            {

                var treeRunner = Selection.activeGameObject.GetComponent<BehaviourTreeRunner>();
                tree = treeRunner?.tree;
            }
            if (tree && tree != _cachedTree)
            {
                _cachedTree = tree;
            }
            if (_cachedTree)
            {
                treeObj?.Dispose(); //先清除缓存
                treeObj = new SerializedObject(_cachedTree);
                blackboardProperty = treeObj.FindProperty("blackboard");
            }

        }

        private void OnNodeSelectionChange(NodeView nodeView)
        {
            inspectorViewer.UpdateSelection(nodeView);
        }

        /// <summary>
        /// 每10帧执行一次的面板刷新回调，以此方法来执行节点样式的更新，开销不会过大
        /// </summary>
        private void OnInspectorUpdate()
        {
            treeViewer?.UpdateNodeStateView();
        }


    }
}
