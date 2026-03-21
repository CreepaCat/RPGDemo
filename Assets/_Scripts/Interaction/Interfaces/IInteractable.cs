using UnityEngine;

namespace RPGDemo.InteractionSystem
{
    
      public interface IInteractable
      {
          
          public Transform transform { get; } 
          public string DisplayName { get;  }
          public string Description { get; }
           
          bool CanInteract();
          void Interact(Interactor interactor);

          void OnFocusGained();
          void OnFocusLost();

      }
}
