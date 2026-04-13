using System.Collections.Generic;
using MyNodeEditor.Extension.Dialogue;
using RPGDemo.InteractionSystem;
using UnityEngine;

namespace NewDialogueFrame
{
    [RequireComponent(typeof(CanversantTarget))]
    public class CanversantTargetInteractable : Interactable
    {
        [SerializeField] private DialogueTree defaultDialogue = null;

        public List<DialogueTree> dialogues = new();

        private void Start()
        {
            dialogues.Add(defaultDialogue);
        }

        //todo:交互时显示所有可选的对话分支UI，根据选择的分支加载对应的对话树
        //用对话Ui面板来显示所有可用对话树选项

        public IEnumerable<DialogueTree> GetVildaDialogues()
        {
            foreach (var dialogue in dialogues)
            {

                if (dialogue != null && dialogue.CanEnter())
                {
                    yield return dialogue;
                }
            }
        }

        public void AddDialogue(DialogueTree newDialogue)
        {
            if (newDialogue == null || dialogues.Contains(newDialogue)) return;
            dialogues.Add(newDialogue);
        }

        public override void Interact(Interactor interactor)
        {
            base.Interact(interactor);
            Debug.Log("Interacting...");
            var playerCanversant = PlayerCanversant.GetPlayerCanversant();
            //todo：显示对话选择面板
            playerCanversant.ChooseDialogue(GetComponent<CanversantTarget>());
            // playerCanversant.StartDialogue(dialogueConfig, GetComponent<CanversantTarget>());
        }


    }
}
