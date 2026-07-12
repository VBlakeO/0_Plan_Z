using UnityEngine;
using UnityEngine.UI;
using PlanZ.Events;
using PlanZ.Interaction.Components;
using PlanZ.Interaction.Events;

namespace PlanZ.UI
{
    // Renders a fillable Image (the Unity standard for radial/linear bars). The image's Fill
    // Amount is driven from 0 to 1 by progress events. When the player cancels or completes,
    // the bar resets and hides until the next interaction begins.
    public class InteractionProgressHUD : MonoBehaviour
    {
        [SerializeField] private GameObject barRoot;
        [SerializeField] private Image fillImage;

        private Interactable _activeInteractable;

        private void OnEnable()
        {
            EventBus.Subscribe<InteractionStartedEvent>(HandleStarted);
            EventBus.Subscribe<InteractionProgressedEvent>(HandleProgressed);
            EventBus.Subscribe<InteractionCancelledEvent>(HandleCancelled);
            EventBus.Subscribe<InteractionCompletedEvent>(HandleCompleted);
            EventBus.Subscribe<InteractionTargetChangedEvent>(HandleTargetChanged);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<InteractionStartedEvent>(HandleStarted);
            EventBus.Unsubscribe<InteractionProgressedEvent>(HandleProgressed);
            EventBus.Unsubscribe<InteractionCancelledEvent>(HandleCancelled);
            EventBus.Unsubscribe<InteractionCompletedEvent>(HandleCompleted);
            EventBus.Unsubscribe<InteractionTargetChangedEvent>(HandleTargetChanged);
        }

        private void Start() => SetVisible(false);

        // For continuous-mode interactions the progress can be non-zero when the player resumes
        // looking at the object, so we also react to TargetChanged to surface the existing
        // progress immediately without waiting for the next Tick event.
        private void HandleTargetChanged(InteractionTargetChangedEvent evt)
        {
            if (evt.Current == null || evt.Current.Progress <= 0f)
            {
                SetVisible(false);
                return;
            }

            SetFill(evt.Current.Progress);
            SetVisible(true);
        }

        private void HandleStarted(InteractionStartedEvent evt)
        {
            _activeInteractable = evt.Target;
            SetFill(evt.Target.Progress);
            SetVisible(true);
        }

        private void HandleProgressed(InteractionProgressedEvent evt)
        {
            if (evt.Target != _activeInteractable) return;
            SetFill(evt.Progress);
        }

        private void HandleCancelled(InteractionCancelledEvent evt)
        {
            if (evt.Target != _activeInteractable) return;

            _activeInteractable = null;
            SetFill(evt.Target.Progress);

            // Hide when fully reset (Hold mode); leave visible for partial progress (Continuous).
            if (evt.Target.Progress <= 0f) SetVisible(false);
        }

        private void HandleCompleted(InteractionCompletedEvent evt)
        {
            if (evt.Target != _activeInteractable) return;

            _activeInteractable = null;
            SetFill(0f);
            SetVisible(false);
        }

        private void SetFill(float value)
        {
            if (fillImage != null) fillImage.fillAmount = value;
        }

        private void SetVisible(bool value)
        {
            if (barRoot != null) barRoot.SetActive(value);
        }
    }
}
