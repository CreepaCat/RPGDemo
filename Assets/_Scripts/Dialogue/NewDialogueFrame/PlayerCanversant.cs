using System;
using System.Collections.Generic;
using MyNodeEditor.Extension.Dialogue;
using RPGDemo.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NewDialogueFrame
{
    /// <summary>
    /// 角色对话交互管理脚本
    /// </summary>
    public class PlayerCanversant : MonoBehaviour
    {
        [SerializeField] DialogueTree currentDialogueTree;

        public event Action<DialogueNodeData> OnDialogueTreeUpdated;
        public event Action OnDialogueBegun;
        public event Action OnDialogueEnded;
        

        DialogueNodeData _currentNodeData = null;
        CanversantTarget currentDialogueTarget = null;
        

        private DialogueNodeData CurrentNodeData
        {
            set
            {
                TriggerExitEvent();
                _currentNodeData = value;
                TriggerEnterEvent();
            }
            get { return _currentNodeData; }
        }
        
        private void Awake()
        {
            Init();
        }

      


        void Update()
        {

            //只有当树不为空时才执行其Update方法
            if (currentDialogueTree != null)
            {
                currentDialogueTree.Update();
            }
        }

        public static PlayerCanversant GetPlayerCanversant()
        {
            return GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCanversant>();
        }

   
        /// <summary>
        /// 发布更新UI事件
        /// </summary>
        /// <param name="runningNodeData"></param>
        public void UpdateDialogue(DialogueNodeData runningNodeData)
        {
            if (runningNodeData == null) return;
            CurrentNodeData = runningNodeData;
            
            OnDialogueTreeUpdated?.Invoke(runningNodeData);

        }

        /// <summary>
        /// 控制显示和关闭对话面板
        /// </summary>
        /// <param name="shouldShow"></param>
        public void StartDialogue(DialogueTree newDialoguTree,CanversantTarget newTarget)
        {
            if (currentDialogueTree!=null 
                && currentDialogueTree.treeState == NodeData.State.Running)
            {
                Debug.Log("已经有在进行中的对话");
                return;
            }
            SetCurrentDialogueTree(newDialoguTree);
            //禁用角色移动输入
            GetComponent<Player>().DisableInput();
            currentDialogueTarget = newTarget;
            
            Debug.Log("Starting Dialogue");
            currentDialogueTree.OnTreeStart();
            OnDialogueBegun?.Invoke();
           
        }


        public void EndDialogue()
        {
            //恢复角色移动输入
            GetComponent<Player>().EnableInput();
            currentDialogueTarget = null;
            
            Debug.Log("Ending Dialogue");
            currentDialogueTree.OnTreeEnd();
            currentDialogueTree = null;
            OnDialogueEnded?.Invoke();
        }
    

        public void OnChoose(int chooseIndex)
        {
            _currentNodeData.Choose(chooseIndex);
        }
        
          //获取角色身上的所有的条件判断器
        public IEnumerable<IPredicateEvaluator> GetEvaluators()
        {
          //  return GetComponents<IPredicateEvaluator>();
          return ConditionHandler.GetInstance().GetEvaluators();
        }

        
        //PRIAVTE
        private void Init()
        {
            currentDialogueTree = null;
            currentDialogueTarget = null;
            _currentNodeData = null;
        }
        
        private void SetCurrentDialogueTree(DialogueTree newDialogueTree)
        {
            currentDialogueTree = newDialogueTree;
            currentDialogueTree.treeState = NodeData.State.Waiting;
        }


        #region 对话事件

        //对话事件触发
        private void TriggerEnterEvent()
        {
            if (_currentNodeData != null && _currentNodeData.OnEnterAction != null)
            {
                if(currentDialogueTarget == null) return;
                DialogueEventTrigger[] eventTriggers =
                    currentDialogueTarget.GetComponentsInChildren<DialogueEventTrigger>();
                foreach (DialogueEventTrigger trigger in eventTriggers)
                {
                    if(!ReferenceEquals(trigger.GetDialogueEvent(),_currentNodeData.OnEnterAction))
                        //  if((DialogueNodeEnterEvent)trigger.GetDialogueEvent() != _currentNodeData.OnEnterAction) 
                        continue;
                    trigger.TriggerEvent();
                }
            }
        }

        private void TriggerExitEvent()
        {
            if (_currentNodeData != null && _currentNodeData.OnExitAction != null)
            {
                if(currentDialogueTarget == null) return;
                DialogueEventTrigger[] eventTriggers =
                    currentDialogueTarget.GetComponentsInChildren<DialogueEventTrigger>();
                foreach (DialogueEventTrigger trigger in eventTriggers)
                {
                    if(!ReferenceEquals(trigger.GetDialogueEvent(),_currentNodeData.OnExitAction))
                        //  if((DialogueNodeEnterEvent)trigger.GetDialogueEvent() != _currentNodeData.OnEnterAction) 
                        continue;
                    trigger.TriggerEvent();
                }
            }
        }
    #endregion
  


    }
}