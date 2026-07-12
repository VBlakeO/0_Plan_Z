using UnityEngine;
using PlanZ.Events;
using PlanZ.Interaction.Components;
using PlanZ.Interaction.Events;
using PlanZ.Weapons.Components;
using PlanZ.Weapons.Events;

namespace PlanZ.Weapons.Pickups
{
    [RequireComponent(typeof(Interactable))]
    public class AmmoBoxPickup : MonoBehaviour
    {
        [SerializeField] private AmmoBoxData data;
        [SerializeField] private AmmoInventory ammoInventory;
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
            EventBus.Subscribe<AmmoReserveChangedEvent>(HandleReserveChanged);

            RefreshAvailability();
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<InteractionCompletedEvent>(HandleCompleted);
            EventBus.Unsubscribe<AmmoReserveChangedEvent>(HandleReserveChanged);
        }

        private void HandleCompleted(InteractionCompletedEvent evt)
        {
            if (evt.Target != _interactable) return;
            if (data == null || ammoInventory == null) return;

            int added = ammoInventory.Add(data.AmmoType, data.Amount);
            if (added <= 0) return;

            ConsumePickup();
        }

        private void ConsumePickup()
        {
            _interactable.SetEnabled(false);
            visualRoot.SetActive(false);
        }

        private void HandleReserveChanged(AmmoReserveChangedEvent evt)
        {
            if (data == null || evt.AmmoType != data.AmmoType) return;
            RefreshAvailability();
        }

        private void RefreshAvailability()
        {
            if (data == null || ammoInventory == null) return;

            bool isFull = ammoInventory.IsFull(data.AmmoType);
            _interactable.SetInteractionBlocked(isFull);

            if (isFull)
                _interactable.SetOverrideLabel(fullLabel);
            else
                _interactable.ClearOverrideLabel();
        }
    }
}