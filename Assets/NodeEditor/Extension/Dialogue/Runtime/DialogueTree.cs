using System.Collections.Generic;
using UnityEngine;

namespace  MyNodeEditor.Extension.Dialogue
{

    [CreateAssetMenu(menuName = "MyDialogue/DialogueTree", fileName = "New DialogueTree")]
    public class DialogueTree : NodeTree
    {

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
