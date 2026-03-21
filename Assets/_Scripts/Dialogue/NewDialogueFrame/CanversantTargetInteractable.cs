using MyNodeEditor.Extension.Dialogue;
using RPGDemo.InteractionSystem;
using UnityEngine;

namespace NewDialogueFrame
{
    [RequireComponent(typeof(CanversantTarget))]
    public class CanversantTargetInteractable:Interactable
    {
        [SerializeField] private DialogueTree dialogueConfig;
        
        public DialogueTree GetDialogueConfig()=>dialogueConfig;
        
        public override void Interact(Interactor interactor)
        {
            base.Interact(interactor);
            Debug.Log("Interacting...");
            var playerCanversant = PlayerCanversant.GetPlayerCanversant();
            //playerCanversant.SetCurrentDialogueTree(dialogueConfig);
           // onInteract?.Invoke();
           playerCanversant.StartDialogue(dialogueConfig,GetComponent<CanversantTarget>());
        }
        
        
    }
}