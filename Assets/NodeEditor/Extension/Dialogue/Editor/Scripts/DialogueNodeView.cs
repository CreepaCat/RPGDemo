using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace MyNodeEditor.Extension.Dialogue
{
    public class DialogueNodeView:NodeView
    {
        public new readonly DialogueNodeData NodeData;
        public bool isEventAreaShown = false;

        public string TextFieldName_DialogueContent { get; private set; } = "DialogueContent";
        
    
        //使用自定义样式
        public DialogueNodeView(DialogueNodeData nodeData):base(nodeData)
        {
          //  if(nodeData == null) return;
            this.NodeData = nodeData;
            this.title = nodeData.speakerName; //title即name
            this.viewDataKey = nodeData.guid;  //viewDataKey即 guid
          //  this.Q<Label>("title").style.fontSize = 14;
            
            style.left = nodeData.position.x;
            style.top = nodeData.position.y;
            
            //NodeData和NodeView数据绑定
            nodeData.OnChanged += UpdateDialogueContent;


        }
        
        /// <summary>
        /// 显示和数据绑定
        /// </summary>
        public void UpdateDialogueContent()
        {
            
            this.title = NodeData.speakerName;

            //更新节点样式
            if(NodeData.HasCondition())
            {
                styleSheets.Add(Resources.Load<StyleSheet>("DialogueNodeView_HasCondition"));
                
            }
            else
            {
                styleSheets.Add(Resources.Load<StyleSheet>("DialogueNodeView"));
            }
            
            this.Q<TextField>(TextFieldName_DialogueContent).SetValueWithoutNotify((NodeData as DialogueNodeData).dialogueContent);
            foreach (var optionData in NodeData.OptionList)
            {
                var optionInput = this.outputContainer.Q<TextField>(optionData.PortName);
                if (optionInput == null)
                {
                    Debug.Log("没找到option输入框" + optionData.PortName);
                    continue;
                }
                optionInput.SetValueWithoutNotify(optionData.OptionText);
            }
            
            //事件区域
            if (isEventAreaShown)
            {
                this.inputContainer.Q<ObjectField>("EnterEvent").SetValueWithoutNotify(NodeData.OnEnterAction);
                this.inputContainer.Q<ObjectField>("ExitEvent").SetValueWithoutNotify(NodeData.OnExitAction);
            }
            
        }
        
        
        

    }
}