using System.Collections.Generic;
using RPGDemo.Core;
using UnityEngine;

namespace MyNodeEditor.Extension.Dialogue
{

    [CreateAssetMenu(menuName = "MyDialogue/DialogueTree", fileName = "New DialogueTree")]
    public class DialogueTree : NodeTree
    {

        [SerializeField] public string displayName;
        [SerializeField] ConditionSO condition;

        public bool CanEnter()
        {
            if (condition == null) return true;
            return condition.Check();
        }

        /// <summary>
        /// 开始执行树
        /// </summary>
        public override void OnTreeStart()
        {
            base.OnTreeStart();
            // DialogueManager dialogueManager = FindFirstObjectByType<DialogueManager>();
            // dialogueManager.ShowDialoguePanel(true);

        }
        public override void OnTreeEnd()
        {
            base.OnTreeEnd();
            // DialogueManager dialogueManager = FindFirstObjectByType<DialogueManager>();
            // dialogueManager.ShowDialoguePanel(false);
        }
    }
}
