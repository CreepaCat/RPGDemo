using UnityEngine;
using UnityEngine.InputSystem;

namespace RPGDemo.InteractionSystem
{

    public class Interactor : MonoBehaviour
    {
        [SerializeField] float radius = 0.2f;
        [SerializeField] private InteractPomptUI interactPomptUI;
        [SerializeField] LayerMask interactableLayers;
        //[SerializeField] Key interactKey = Key.F;

        private Collider[] interactableBuffer = new Collider[32];
        private IInteractable focused;





        private void Update()
        {
            IInteractable neareast = FindNearestInteractable();
            UpdateFocused(neareast);


        }

        public void DoInteract()
        {
            if (focused != null && focused.CanInteract())
            {
                focused?.Interact(this);
                interactPomptUI.Hide();

            }


        }

        private IInteractable FindNearestInteractable()
        {
            int count = Physics.OverlapSphereNonAlloc(transform.position, radius, interactableBuffer, interactableLayers, QueryTriggerInteraction.Collide);
            IInteractable neareast = null;
            float minDistance = float.MaxValue;

            for (int i = 0; i < count; i++)
            {
                Collider other = interactableBuffer[i];
                if (other == null) continue;
                IInteractable interactable = other.GetComponentInParent<IInteractable>();
                if (interactable == null) continue;
                float distance = Vector3.Distance(transform.position, other.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    neareast = interactable;
                }
            }
            return neareast;
        }

        private void UpdateFocused(IInteractable nearest)
        {
            if (ReferenceEquals(nearest, focused)) return;

            focused?.OnFocusLost();
            focused = nearest;
            if (focused != null)
            {
                focused.OnFocusGained();
                interactPomptUI.Show(focused);
            }
            else
            {
                interactPomptUI.Hide();
            }

        }



        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}
