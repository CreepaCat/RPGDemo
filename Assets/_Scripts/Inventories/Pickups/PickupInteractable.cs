using RPGDemo.InteractionSystem;
using UnityEngine;

namespace RPGDemo.Inventories.Pickups
{
    [RequireComponent(typeof(Pickup))]
    public class PickupInteractable : Interactable
    {
        private void Start()
        {
            displayName = GetComponent<Pickup>().GetItem().GetDisplayName();
            description = GetComponent<Pickup>().GetItem().GetDescription();
        }

        public override void Interact(Interactor interactor)
        {
            base.Interact(interactor);
            GetComponent<Pickup>()?.PickupItem();
        }
    }
}