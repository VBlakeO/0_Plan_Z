using UnityEngine;
using PlanZ.Core;
using PlanZ.Events;
using PlanZ.Interaction.Events;

namespace PlanZ.Interaction.Components
{
    // Casts a ray from the player's aim camera every frame and tracks the current Interactable
    // under the crosshair. Publishes target-changed events so HUD and controller can react
    // without each doing their own raycasts. The raycast itself lives here so the cost is paid
    // exactly once per frame regardless of how many systems care about the hover state.
    public class InteractionRaycaster : SingletonMonoBehaviour<InteractionRaycaster>
    {
        [SerializeField] private Camera aimCamera;
        [SerializeField] private float range = 3f;
        [SerializeField] private LayerMask interactableLayers = ~0;

        public Interactable CurrentTarget { get; private set; }

        private void Update()
        {
            if (aimCamera == null) return;

            Interactable next = ResolveHoveredInteractable();
            if (next == CurrentTarget) return;

            Interactable previous = CurrentTarget;
            CurrentTarget = next;
            EventBus.Publish(new InteractionTargetChangedEvent(previous, next));
        }

        // GetComponentInParent so a collider on a child mesh still resolves to the Interactable
        // on the root - common pattern when the visual model is one object and the gameplay
        // component sits on a parent that owns colliders, audio, etc.
        private Interactable ResolveHoveredInteractable()
        {
            if (!Physics.Raycast(aimCamera.transform.position, aimCamera.transform.forward,
                out RaycastHit hit, range, interactableLayers, QueryTriggerInteraction.Ignore))
                return null;

            Interactable candidate = hit.transform.GetComponentInParent<Interactable>();
            if (candidate == null) return null;
            if (!candidate.IsDetectable) return null;

            return candidate;
        }
    }
}