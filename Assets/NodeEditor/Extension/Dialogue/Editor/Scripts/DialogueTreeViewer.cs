using System;
using System.Collections.Generic;
using System.Linq;
using MyNodeEditorExtension.Dialogue;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace MyNodeEditor.Extension.Dialogue
{
    [UxmlElement]
    public partial class DialogueTreeViewer : GraphView
    {
        public InspectorViewer InspectorViewer = null;
        public DialogueTree currentNodeTree;
        private bool _undoRegistered = false;
        private bool _suppressGraphViewWriteBack = false;


        //节点选中事件
        public Action<NodeView> OnNodeSelected;

        public Vector2 DefaultNodeSize = new Vector2(220, 150);

        //
        // [System.Obsolete]
        // //此行代码代表此类是一个自定义UIToolkit控件
        // public new class UxmlFactory : UxmlFactory<DialogueTreeViewer, GraphView.UxmlTraits> { }

        public DialogueTreeViewer()
        {
            Insert(0, new GridBackground()); //网格背景
            this.AddManipulator(new ContentZoomer());  //窗口缩放
            this.AddManipulator(new ContentDragger()); //窗口拖动
            this.AddManipulator(new SelectionDragger()); //选择拖动
            this.AddManipulator(new RectangleSelector()); //矩形选择框

            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            //自定义样式
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/NodeEditor/Extension/Dialogue/Editor/UI/DialogueTreeViewer.uss");
            styleSheets.Add(styleSheet);

            CreateEntryPoint();

            //撤销重做事件
            Undo.undoRedoPerformed += OnUndoRedo;
            _undoRegistered = true;

        }

        public void DisposeViewer()
        {
            if (_undoRegistered)
            {
                Undo.undoRedoPerformed -= OnUndoRedo;
                _undoRegistered = false;
            }

            graphViewChanged -= OnGraphViewChanged;
            OnNodeSelected = null;
        }

        public void SetGraphViewWriteBackSuppressed(bool suppressed)
        {
            _suppressGraphViewWriteBack = suppressed;
        }

        /// <summary>
        /// 鼠标反键菜单
        /// </summary>
        /// <param name="evt"></param>

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            //运行时不准创建和删除节点
            if (Application.isPlaying)
            {
                return;

            }
            var type = typeof(DialogueNodeData);

            //todo:改用search窗口创建节点，并将创建的节点设置在鼠标旁边
            var mousePos = evt.localMousePosition; //鼠标坐标
            var localMousePosition = contentViewContainer.WorldToLocal(mousePos);//映射到窗口坐标位置
            localMousePosition += new Vector2(InspectorViewer.contentContainer.contentRect.width, 0f);   // 居中调整(侧边栏宽度)

            evt.menu.AppendAction($"New DialogueNode", (a) =>
            {
                var nodeView = CreateNode(type);
                nodeView.SetPosition(new Rect(localMousePosition, DefaultNodeSize));
            });
            base.BuildContextualMenu(evt);


        }

        public DialogueTree CreateTree()
        {
            var newNodeTree = ScriptableObject.CreateInstance<DialogueTree>();
            newNodeTree.name = Guid.NewGuid().ToString();

            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                AssetDatabase.CreateFolder("Assets", "Resources");

            UnityEngine.Object loadedAsset = AssetDatabase.LoadAssetAtPath($"Assets/Resources/{newNodeTree.name}.asset", typeof(DialogueTree));

            if (loadedAsset == null || !AssetDatabase.Contains(loadedAsset))
            {
                AssetDatabase.CreateAsset(newNodeTree, $"Assets/Resources/{newNodeTree.name}.asset");
            }

            currentNodeTree = newNodeTree;

            AssetDatabase.SaveAssets();
            return currentNodeTree;
        }




        #region 公共方法

        /// <summary>
        /// 通过菜单创建节点资产 和 可视化节点
        /// </summary>
        /// <param name="type"></param>
        public NodeView CreateNode(Type type)
        {
            if (Application.isPlaying)
            {
                EditorUtility.DisplayDialog("运行时创建节点", "运行时无法创建新节点", "Ok");
                return null;

            }
            if (currentNodeTree == null)
            {
                //在资源文件夹下创建对话树资源
                CreateTree();
                //树创建后注册视图change事件
                RegistGraphViewChangeEvent();
            }

            Debug.Log($"尝试创建节点: {type.Name}");
            DialogueNodeData nodeData = (DialogueNodeData)currentNodeTree.CreateNode(type); //创建节点资产

            var contentCenter = GetGraphViewCenter();
            //设置初始位置在窗口中间 + 侧边栏宽度
            nodeData.position = new Vector2(contentCenter.x + InspectorViewer.contentContainer.contentRect.width, contentCenter.y);

            return CreateNodeView(nodeData);
        }

        public DialogueNodeView CreateNodeView(DialogueNodeData nodeDate)
        {
            var tempNodeView = new DialogueNodeView(nodeDate);
            //节点样式
            // if (nodeDate.HasCondition())
            // {
            //     tempNodeView.styleSheets.Add(Resources.Load<StyleSheet>("DialogueNodeView_HasCondition"));
            // }
            // else
            // {
            //     tempNodeView.styleSheets.Add(Resources.Load<StyleSheet>("DialogueNodeView"));
            // }
            tempNodeView.UpdateStyleSheet();


            // tempDialogueNode.styleSheets.Add(Resources.Load<StyleSheet>("Node"));
            var inputPort = GetPortInstance(tempNodeView, Direction.Input, Port.Capacity.Multi);
            inputPort.portName = "Input";
            tempNodeView.inputContainer.Add(inputPort);
            tempNodeView.RefreshExpandedState();
            tempNodeView.RefreshPorts();
            tempNodeView.SetPosition(new Rect(nodeDate.position, DefaultNodeSize));


            var dialogueTextField = CreateDialogueTextField(tempNodeView);

            CreateNodeViewButtons(tempNodeView);
            CreateEventAddArea(tempNodeView);

            // var button = new Button(() => { AddChoicePort(tempNodeView); })
            // {
            //     text = "添加选项"
            // };
            // tempNodeView.titleButtonContainer.Add(button);

            tempNodeView.OnNodeSelected += OnNodeSelected;

            AddElement(tempNodeView);
            return tempNodeView;
        }
        /// <summary>
        /// 添加节点选项端口
        /// </summary>
        /// <param name="dialogueNodeCache"></param>
        /// <param name="optionData"></param>
        public void AddChoicePort(DialogueNodeView dialogueNodeCache, DialogueNodeData.OptionData optionData = null)
        {
            var generatedPort = GetPortInstance(dialogueNodeCache, Direction.Output);
            //隐藏出口端口标签
            var portLabel = generatedPort.contentContainer.Q<Label>("type");
            portLabel.style.display = DisplayStyle.None;

            //使用自定义端口数据，便于保存
            var outputPortCount = dialogueNodeCache.outputContainer.Query("connector").ToList().Count();
            if (optionData == null)
            {
                optionData = new DialogueNodeData.OptionData()
                {
                    //端口名即为唯一id
                    PortName = GUID.Generate().ToString(),
                    OptionText = $"Option {outputPortCount + 1}",
                };
                dialogueNodeCache.NodeData.AddPort(optionData);
            }


            var optionText = optionData.OptionText;
            generatedPort.portName = optionData.PortName;

            var optionTextField = CreateOptionTextField(optionData, optionText);
            generatedPort.contentContainer.Add(new Label("  "));
            generatedPort.contentContainer.Add(optionTextField);
            var deleteButton = new Button(() => RemovePort(dialogueNodeCache, generatedPort))
            {
                text = "X"
            };
            generatedPort.contentContainer.Add(deleteButton);


            dialogueNodeCache.outputContainer.Add(generatedPort);
            dialogueNodeCache.RefreshPorts();
            dialogueNodeCache.RefreshExpandedState();
        }



        /// <summary>
        /// 将新树赋值给当前树，清除之前的节点,用来显示新树
        /// </summary>
        /// <param name="tree"></param>
        internal void PopulateView(DialogueTree tree)
        {
            if (tree == null)
            {
                //EditorUtility.DisplayDialog("File Not Found", "请先选择加载文件!", "OK");
                return;

            }

            this.currentNodeTree = tree;

            RegistGraphViewChangeEvent();

            //直接通过加载框加载
            DialogueGraphSaveUtility.GetInstance(this).RepaintCurrentGraph();

        }
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.ToList().Where(
                //开始端口 和 结束端口 的方向不能是一样的（即不能同时是输出或输入端口）
                endport => endport.direction != startPort.direction
                           //两个端口不能是同一个节点上的，即节点不能自己与自己相连
                           && endport.node != startPort.node).ToList();
        }
        #endregion


        #region nodeview UI创建

        private void CreateNodeViewButtons(DialogueNodeView tempNodeView)
        {
            var btnEvent = new Button()
            {
                text = "显示事件"
            };
            btnEvent.clickable.clicked += () =>
            {
                ClickEventButton(tempNodeView, btnEvent);
            };

            tempNodeView.titleButtonContainer.Add(btnEvent);
            var button = new Button(() => { AddChoicePort(tempNodeView); })
            {
                text = "添加选项"
            };
            tempNodeView.titleButtonContainer.Add(button);
        }

        private void ClickEventButton(DialogueNodeView nodeView, Button btnEvent)
        {
            if (nodeView.isEventAreaShown)
            {
                nodeView.isEventAreaShown = false;
                btnEvent.text = "显示事件";
                nodeView.inputContainer.Q<VisualElement>("EventAddArea").style.display = DisplayStyle.None;

            }
            else
            {
                nodeView.isEventAreaShown = true;
                btnEvent.text = "隐藏事件";
                nodeView.inputContainer.Q<VisualElement>("EventAddArea").style.display = DisplayStyle.Flex;
            }
        }

        private void CreateEventAddArea(DialogueNodeView tempNodeView)
        {
            VisualElement eventAddArea = new VisualElement() { name = "EventAddArea" };
            eventAddArea.style.flexDirection = FlexDirection.Column;
            eventAddArea.style.flexGrow = 1f;
            eventAddArea.style.width = DefaultNodeSize.x * 0.5f;
            eventAddArea.style.display = DisplayStyle.None;

            var enterEventObjectField = CreateEventObjectField("EnterEvent", "OnEnter", typeof(DialogueNodeEventSO), tempNodeView);
            enterEventObjectField.value = tempNodeView.NodeData.OnEnterAction;
            var exitEventObjectField = CreateEventObjectField("ExitEvent", "OnExit", typeof(DialogueNodeEventSO), tempNodeView);
            exitEventObjectField.value = tempNodeView.NodeData.OnExitAction;

            eventAddArea.Add(enterEventObjectField);
            eventAddArea.Add(exitEventObjectField);

            tempNodeView.inputContainer.Add(eventAddArea);
        }

        private ObjectField CreateEventObjectField(string objectFiledname, string labelname, Type fieldType, DialogueNodeView tempNodeView)
        {
            ObjectField eventObjectField = new ObjectField()
            {
                name = objectFiledname,
                label = labelname,
                objectType = fieldType,
                // value = tempNodeView.NodeData.OnEnterAction,
            };

            //数据绑定事件
            eventObjectField.RegisterValueChangedCallback((evt) =>
            {
                if (objectFiledname == "EnterEvent")
                {
                    tempNodeView.NodeData.OnEnterAction = evt.newValue as DialogueNodeEventSO;
                }
                else if (objectFiledname == "ExitEvent")
                {
                    tempNodeView.NodeData.OnExitAction = evt.newValue as DialogueNodeEventSO;
                }
            });
            eventObjectField.style.flexDirection = FlexDirection.Column;
            eventObjectField.style.width = 100;
            eventObjectField.labelElement.style.fontSize = 12;
            eventObjectField.labelElement.style.width = 20;  // 或 new Length(30, LengthUnit.Percent); 根据需要调整
            eventObjectField.labelElement.style.flexGrow = 0;  // 防止标签扩展
            eventObjectField.labelElement.style.flexShrink = 0;  // 防止标签压缩
            return eventObjectField;
        }


        private TextField CreateDialogueTextField(DialogueNodeView tempDialogueNodeView)
        {
            //对话文本输入框
            var dialogueTextField = new TextField("");
            dialogueTextField.name = tempDialogueNodeView.TextFieldName_DialogueContent;
            dialogueTextField.multiline = true;
            dialogueTextField.style.whiteSpace = WhiteSpace.Normal;     // 自动换行
            dialogueTextField.style.unityTextAlign = TextAnchor.UpperLeft; // 可选：左上对齐

            // 最大高度 + 垂直滚动（内容超高时自动出现滚动条）
            dialogueTextField.style.height = new StyleLength(StyleKeyword.Auto);
            // dialogueTextField.style.minHeight = 50;
            dialogueTextField.style.maxWidth = DefaultNodeSize.x + 10;
            dialogueTextField.style.maxHeight = 100; // 限制最大高度，避免无限拉伸

            //对话文本数据绑定
            dialogueTextField.SetValueWithoutNotify(tempDialogueNodeView.NodeData.dialogueContent);

            dialogueTextField.RegisterValueChangedCallback(evt =>
            {

                //实时保存对话输入
                tempDialogueNodeView.NodeData.dialogueContent = evt.newValue; //数据容器发生改变要setdirty
                                                                              // Debug.Log($"<实时保存对话输入>: {tempDialogueNodeView.NodeData.dialogueContent}");
                EditorUtility.SetDirty(tempDialogueNodeView.NodeData);
            });
            tempDialogueNodeView.mainContainer.Add(dialogueTextField);
            return dialogueTextField;
        }
        private TextField CreateOptionTextField(DialogueNodeData.OptionData optionData, string optionText)
        {

            var optionTextField = new TextField()
            {
                name = optionData.PortName, //用端口名作为数据绑定的id
                value = optionText
            };
            //数据绑定
            // tempDialogueNodeView.NodeData.OnChanged += tempDialogueNodeView.UpdateDialogueContent;
            optionTextField.SetValueWithoutNotify(optionData.OptionText);
            optionTextField.style.maxWidth = DefaultNodeSize.x - 120;
            optionTextField.RegisterValueChangedCallback(evt =>
            {
                //实时更新portOption文本缓存
                optionData.OptionText = evt.newValue;

            });
            return optionTextField;
        }

        #endregion

        #region 视图同步
        private void OnUndoRedo()
        {
            PopulateView(currentNodeTree);

            AssetDatabase.SaveAssets();
        }
        /// <summary>
        /// 获取窗口中心位置
        /// </summary>
        /// <returns></returns>
        private Vector2 GetGraphViewCenter()
        {
            // 获取 GraphView 的可见区域（viewport）大小
            var viewSize = this.viewport.layout.size;

            // 计算窗口中心在世界坐标中的位置
            var centerInWorld = viewSize / 2f;

            // 将世界坐标转换为内容坐标（考虑缩放和滚动）
            var centerInContent = this.contentViewContainer.WorldToLocal(centerInWorld);

            // 减去节点大小的一半，使节点真正居中
            centerInContent -= DefaultNodeSize / 2f;
            return centerInContent;

        }




        private void RegistGraphViewChangeEvent()
        {
            //注册视图修改事件
            graphViewChanged -= OnGraphViewChanged;
            // DeleteElements(graphElements);
            var loader = DialogueGraphSaveUtility.GetInstance(this);
            loader.ClearGraph();
            graphViewChanged += OnGraphViewChanged;
        }

        private void RemovePort(DialogueNodeView dialogueNodeCache, Port socket)
        {
            //移除节点时同时要移除节点相关连线

            var targetEdge = edges.ToList().Where(x => x.output.portName == socket.portName && x.output.node == socket.node); ;
            if (targetEdge.Any())
            {
                var edge = targetEdge.First();
                RemoveEdgeRelationship(edge);

                edge.input.Disconnect(edge);
                edge.output.Disconnect(edge);
                RemoveElement(targetEdge.First());
                //同时移除节点父子关系
                //连接线输出端的为父节点，输入端的为子节点

            }
            //移除端口缓存
            dialogueNodeCache.NodeData.RemovePortByName(socket.portName);
            dialogueNodeCache.outputContainer.Remove(socket);



            dialogueNodeCache.RefreshPorts();
            dialogueNodeCache.RefreshExpandedState();
        }

        private void RemoveEdgeRelationship(Edge edge)
        {
            if (edge == null) return;
            DialogueNodeView parentView = edge.output.node as DialogueNodeView;
            DialogueNodeView childView = edge.input.node as DialogueNodeView;
            //移除父子关系
            currentNodeTree.DisconnectChild(parentView.NodeData, childView.NodeData);
        }


        /// <summary>
        /// 创建端口
        /// </summary>
        /// <param name="dialogueNodeView"></param>
        /// <param name="nodeDirection"></param>
        /// <param name="capacity"></param>
        /// <returns></returns>
        private Port GetPortInstance(DialogueNodeView dialogueNodeView, Direction nodeDirection,
            Port.Capacity capacity = Port.Capacity.Single)
        {
            return dialogueNodeView.InstantiatePort(Orientation.Horizontal, nodeDirection, capacity, typeof(float));
        }
        /// <summary>
        /// 创建起始节点,起始节点每次打开窗口都自动创建，且不保存为树的子节点
        /// </summary>
        /// <returns></returns>
        private DialogueNodeView CreateEntryPoint()
        {

            DialogueNodeData entryNodeData = ScriptableObject.CreateInstance<DialogueNodeData>();

            entryNodeData.guid = "Start";
            entryNodeData.name = "START";
            // entryNodeData.tree = currentNodeTree; //必须将当前树赋值，当前树可能为空
            entryNodeData.isEntryPoint = true;
            entryNodeData.position = new Vector2(100, 200);

            return CreateEntryNodeView(entryNodeData);
        }

        private DialogueNodeView CreateEntryNodeView(DialogueNodeData entryNodeData)
        {
            var nodeCache = new DialogueNodeView(entryNodeData);
            nodeCache.title = entryNodeData.name;

            var generatedPort = GetPortInstance(nodeCache, Direction.Output);
            generatedPort.portName = "Next";
            nodeCache.outputContainer.Add(generatedPort);

            nodeCache.capabilities &= ~Capabilities.Movable;
            nodeCache.capabilities &= ~Capabilities.Deletable;

            nodeCache.RefreshExpandedState();
            nodeCache.RefreshPorts();
            nodeCache.SetPosition(new Rect(100, 200, 100, 150));
            AddElement(nodeCache);
            return nodeCache;
        }
        #endregion


        /// <summary>
        /// 当视图发生改变时的回调方法
        /// </summary>
        /// <param name="graphViewChange"></param>
        /// <returns></returns>
        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            if (_suppressGraphViewWriteBack)
            {
                return graphViewChange;
            }

            Debug.Log("OnGraphViewChanged");
            //处理移除元素的情况（节点 或连线）
            if (graphViewChange.elementsToRemove != null)
            {
                Debug.Log("OnElementsToRemove");

                //遍历所有被移除的元素
                graphViewChange.elementsToRemove.ForEach(elem =>
                {
                    //若被移除的元素是可视化节点，则移除对应的节点资产
                    DialogueNodeView dialogueNodeView = elem as DialogueNodeView;
                    if (dialogueNodeView != null)
                    {
                        currentNodeTree.DeleteNode(dialogueNodeView.NodeData);

                    }

                    //处理删除节点连接线的情况
                    Edge edge = elem as Edge;
                    if (edge != null)
                    {
                        RemoveEdgeRelationship(edge);
                    }
                });

            }
            //处理添加连线的情况
            if (graphViewChange.edgesToCreate != null)
            {
                Debug.Log("OnEdgesToCreate");
                //此方法是添加的连线还没有保存到data,所以不能直接使用dialogueTree的remap来更新父子关系
                graphViewChange.edgesToCreate.ForEach(edge =>
                {
                    //连接线输出端的为父节点，输入端的为子节点
                    DialogueNodeView parentView = edge.output.node as DialogueNodeView;
                    DialogueNodeView childView = edge.input.node as DialogueNodeView;
                    //添加父子关系
                    currentNodeTree.LinkChild(parentView.NodeData, edge.output.portName, childView.NodeData);
                });


            }



            return graphViewChange;
        }

    }
}
