using PlanZ.Events;
using PlanZ.Healing.Components;
using PlanZ.Healing.Events;
using PlanZ.Interaction.Components;
using PlanZ.Interaction.Events;
using UnityEngine;

namespace PlanZ.Healing.Pickups
{
    [RequireComponent(typeof(Interactable))]
    public class MedkitPickup : MonoBehaviour
    {
        [SerializeField] private MedkitInventory medkitInventory;
        [SerializeField] private GameObject visualRoot;
        [SerializeField] private string fullLabel = "Cheio";

        private Interactable _interactable;

        private void Awake()
        {
            _interactable = GetComponent<Interactable>();
            if (visualRoot == null) visualRoot = gameObject;
        }

        private void OnEnable()
        {
            EventBus.Subscribe<InteractionCompletedEvent>(HandleCompleted);
            EventBus.Subscribe<MedkitInventoryChangedEvent>(HandleReserveChanged);
            RefreshAvailability();
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<InteractionCompletedEvent>(HandleCompleted);
            EventBus.Subscribe<MedkitInventoryChangedEvent>(HandleReserveChanged);
        }

        private void HandleCompleted(InteractionCompletedEvent evt)
        {
            if (evt.Target != _interactable) return;
            if (medkitInventory == null) return;

            int added = medkitInventory.Add(1);
            if (added <= 0) return;

            ConsumePickup();
        }

        private void HandleReserveChanged(MedkitInventoryChangedEvent evt)
        {
            RefreshAvailability();
        }

        private void ConsumePickup()
        {
            _interactable.SetEnabled(false);
            visualRoot.SetActive(false);
        }

        private void RefreshAvailability()
        {
            if (medkitInventory == null) return;

            bool isFull = medkitInventory.IsFull;
            _interactable.SetInteractionBlocked(isFull);

            if (isFull)
                _interactable.SetOverrideLabel(fullLabel);
            else
                _interactable.ClearOverrideLabel();
        }
    }
}
