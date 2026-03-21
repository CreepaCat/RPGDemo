using System;
using System.Collections.Generic;

using NewDialogueFrame;
using RPGDemo.Core;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MyNodeEditor.Extension.Dialogue
{

    [Serializable]
    public class DialogueNodeData : NodeData
    {
        public string speakerName;
        public Sprite avatar;
        public DialogueRole dialogueRole;
        [TextArea] public string dialogueContent;

        public Action OnChanged;


        public DialogueNodeEventSO OnEnterAction;
        public DialogueNodeEventSO OnExitAction;

        public ConditionSO condition;

        private void OnValidate()
        {
            if (dialogueRole != null)
            {
                speakerName = dialogueRole.speakerName;
                avatar = dialogueRole.avatar;
            }
            else
            {
                speakerName = "";
                avatar = null;
            }

            OnChanged?.Invoke();
        }

        public bool HasCondition() => condition != null;

        // [HideInInspector]
        public List<OptionData> OptionList = new List<OptionData>();

        /// <summary>
        /// 用来便于保存和读取多选项端口和其文本
        /// </summary>
        [Serializable]
        public class OptionData
        {
            public string OptionText;
            [HideInInspector]
            public string PortName;
            [HideInInspector]
            public string ChildGuid;
            [HideInInspector]
            public int OptionIndex = -1;
        }

        #region editor output端口数据增删

        public void AddPort(OptionData optionData)
        {
            OptionList.Add(optionData);
            UpdateOptionIndex();
        }


        public void RemovePortByName(string portName)
        {

            for (int i = 0; i < OptionList.Count; i++)
            {
                if (OptionList[i].PortName == portName)
                {
                    OptionList.RemoveAt(i);
                    break;
                }
            }

            RemoveNullOption();

        }
        #endregion
        #region 连接和移除子节点时调用
        public override void DisconnectChild(string childGuid)
        {
            children.Remove(tree.GetNodeData(childGuid));

            for (int i = 0; i < OptionList.Count; i++)
            {
                if (OptionList[i].ChildGuid == childGuid)
                {
                    OptionList.RemoveAt(i);
                    break;
                }
            }
            RemoveNullOption();
        }
        public override void LinkChild(string childGuid, string portName)
        {
            children.Add(tree.GetNodeData(childGuid));

            for (int i = 0; i < OptionList.Count; i++)
            {
                if (OptionList[i].PortName == portName)
                {
                    OptionList[i].ChildGuid = childGuid;
                    break;
                }
            }

        }
        #endregion

        private void RemoveNullOption()
        {
            //每次移除后，都要消除空对象
            List<OptionData> tempOptionList = new();
            for (int i = 0; i < OptionList.Count; i++)
            {
                if (OptionList[i] == null) continue;
                tempOptionList.Add(OptionList[i]);
            }
            OptionList = tempOptionList;
            UpdateOptionIndex();
        }

        private void UpdateOptionIndex()
        {
            for (int i = 0; i < OptionList.Count; i++)
            {
                OptionList[i].OptionIndex = i;
            }
        }


        #region runtime逻辑

        [HideInInspector] public int nextDialogueIndex = 0;
        [HideInInspector] public bool nexDialogueStart = false;


        /// <summary>
        /// 选择选项
        /// </summary>
        /// <param name="chosenIndex"></param>
        public void Choose(int chosenIndex)
        {
            nextDialogueIndex = chosenIndex;
            nexDialogueStart = true;
        }

        public void RestNode()
        {
            nextDialogueIndex = 0;
            nexDialogueStart = false;
        }



        public override NodeData OnLogicUpdate()
        {
            if (nexDialogueStart)
            {
                state = State.Waiting;
                //判断GetValidateOptions()里是否包含对应的index
                foreach (var option in GetValidateOptions())
                {
                    if (option.OptionIndex != nextDialogueIndex)
                    {
                        continue;
                    }
                    Debug.Log($"选项{nexDialogueStart}对应" + option.OptionText + "被选择了");
                    NodeData nodeData = tree.GetNodeData(option.ChildGuid);
                    //  children[nextDialogueIndex].state = State.Running;

                    if (!children.Contains(nodeData))
                    {
                        Debug.Log("子节点不在选项中");
                        return this;
                    }

                    nodeData.state = State.Running;
                    return nodeData;
                    // return children[nextDialogueIndex];
                }
            }
            return this;

        }


        public override void OnStart()
        {
            //进入时重置节点
            RestNode();

            PlayerCanversant.GetPlayerCanversant().UpdateDialogue(this);

            // throw new System.NotImplementedException();
            //Debug.Log(dialogueContent);

        }

        public override void OnStop()
        {
            //throw new System.NotImplementedException();
            Debug.Log("OnStop");
            //如果没有后继对话，关闭对话面板
            if (GetValidateOptions().Count < 1)
            {
                PlayerCanversant.GetPlayerCanversant().EndDialogue();

            }
        }

        #endregion

        #region 引入条件判断框架
        /// <summary>
        /// 引入条件判断框架,获得合格选项
        /// </summary>
        public bool CheckConditions(IEnumerable<IPredicateEvaluator> evaluators)
        {
            if (condition == null || condition.GetParameters() == null) return true;
            return condition.Check();
        }

        /// <summary>
        /// 用于条件对话时，获得所有目前角色可选择的选项
        /// </summary>
        /// <param name="evaluators"></param>
        /// <returns></returns>
        public List<OptionData> GetValidateOptions()
        {
            List<OptionData> result = new List<OptionData>();

            foreach (var option in OptionList)
            {
                if (string.IsNullOrEmpty(option.ChildGuid)) continue; //如果选项没有连接到子节点，跳过这个选项
                var nodeData = tree.GetNodeData(option.ChildGuid);
                if (!children.Contains(nodeData)) continue; //如果子节点不在children里，说明不是当前节点的后继节点，跳过这个选项
                DialogueNodeData diaNodeData = nodeData as DialogueNodeData;
                if (diaNodeData.CheckConditions(ConditionHandler.GetInstance().GetEvaluators()))
                {
                    result.Add(option);

                }
            }
            return result;
        }


        #endregion
    }
}
