using UnityEngine;
using PlanZ.Events;
using PlanZ.Interaction.Events;
using PlanZ.Player.Input;

namespace PlanZ.Interaction.Components
{
    // Drives the interactable lifecycle from input. Subscribes to input events and queries the
    // Raycaster for the current target. Keeps a reference to the active interactable so it can
    // call Cancel if the target changes mid-interaction (player looks away while holding the key).
    public class InteractionController : MonoBehaviour
    {
        private Interactable _activeInteractable;

        private void OnEnable()
        {
            EventBus.Subscribe<InteractionPressedEvent>(HandlePressed);
            EventBus.Subscribe<InteractionReleasedEvent>(HandleReleased);
            EventBus.Subscribe<InteractionTargetChangedEvent>(HandleTargetChanged);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<InteractionPressedEvent>(HandlePressed);
            EventBus.Unsubscribe<InteractionReleasedEvent>(HandleReleased);
            EventBus.Unsubscribe<InteractionTargetChangedEvent>(HandleTargetChanged);
        }

        private void Update()
        {
            if (_activeInteractable == null) return;

            // Tick is the only per-frame work; the key being held is enough to keep ticking.
            // Releasing the key triggers Cancel via HandleReleased.
            if (PlayerInput.Instance != null && PlayerInput.Instance.InteractHeld)
            {
                bool stillRunning = _activeInteractable.Tick(Time.deltaTime);
                if (!stillRunning) _activeInteractable = null;
            }
        }

        private void HandlePressed()
        {
            if (InteractionRaycaster.Instance == null) return;

            Interactable target = InteractionRaycaster.Instance.CurrentTarget;
            if (target == null) return;

            if (target.Begin())
                _activeInteractable = target;
        }

        private void HandleReleased()
        {
            if (_activeInteractable == null) return;

            _activeInteractable.Cancel();
            _activeInteractable = null;
        }

        // If the player looks away while holding the key, cancel the active interaction. This
        // matches the original behaviour and prevents "interacting from afar" exploits where the
        // player starts an interaction and then turns around to do something else while it ticks.
        private void HandleTargetChanged(InteractionTargetChangedEvent evt)
        {
            if (_activeInteractable == null) return;
            if (evt.Current == _activeInteractable) return;

            _activeInteractable.Cancel();
            _activeInteractable = null;
        }
    }
}
