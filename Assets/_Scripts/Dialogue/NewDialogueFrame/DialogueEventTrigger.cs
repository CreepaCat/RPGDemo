using MyNodeEditor.Extension.Dialogue;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace NewDialogueFrame
{
    /// <summary>
    ///用于对话事件的触发 
    /// </summary>
    public class DialogueEventTrigger:MonoBehaviour
    {
        [SerializeField] DialogueNodeEventSO dialogueEvent;
        [SerializeField] private UnityEvent onTrigger;

        public DialogueNodeEventSO GetDialogueEvent() => dialogueEvent;

        public void TriggerEvent()
        {
            Debug.Log("触发对话事件:" + dialogueEvent.description);
            onTrigger.Invoke();
        }
    }
}