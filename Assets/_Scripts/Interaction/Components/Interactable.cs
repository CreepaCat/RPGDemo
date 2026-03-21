using UnityEngine;
using UnityEngine.Events;

namespace RPGDemo.InteractionSystem
{
    
    /// <summary>
    /// interactable物品需要设置为Interactor对应的LayerMask
    /// </summary>
    [RequireComponent(typeof(Outliner))]
    [RequireComponent(typeof(Collider))]
    public class Interactable : MonoBehaviour, IInteractable
    {
        [SerializeField] protected string displayName = "Interact";
        [SerializeField] protected string description = "Description";

      
        [SerializeField] protected bool isEnabled = true;
        [SerializeField] protected UnityEvent onInteract;
        public string DisplayName =>displayName;
        public string Description => description;
        
        private Outliner _outliner;

        protected virtual void Awake()
        {
            _outliner =  GetComponent<Outliner>();
        }

        public virtual bool CanInteract()=> isEnabled;
      
        

        //用事件解耦交互者和交互对象
        public virtual void Interact(Interactor interactor)
        {
            Debug.Log("Interacting...");
          onInteract?.Invoke();
        }

        public virtual void OnFocusGained()
        {
           // Debug.Log("OnFocusGained");
            _outliner.ShowOutline();

        }

        public virtual void OnFocusLost()
        {
           // Debug.Log("OnFocusLost");
          _outliner.HideOutline();
        }
    }
}
