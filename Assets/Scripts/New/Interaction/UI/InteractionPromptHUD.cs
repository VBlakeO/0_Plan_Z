using UnityEngine;
using UnityEngine.UI;
using PlanZ.Events;
using PlanZ.Interaction.Components;
using PlanZ.Interaction.Events;
using TMPro;

namespace PlanZ.UI
{
    // Shows/hides the prompt panel and updates its label when the player aims at an Interactable.
    // Listens both to target changes (player looks at a new object) and label changes (the
    // current target updated its DisplayLabel because of a state change like "Full" / "Empty").
    public class InteractionPromptHUD : MonoBehaviour
    {
        [SerializeField] private GameObject promptRoot;
        [SerializeField] private TextMeshProUGUI labelText;

        private Interactable _currentTarget;

        private void OnEnable()
        {
            EventBus.Subscribe<InteractionTargetChangedEvent>(HandleTargetChanged);
            EventBus.Subscribe<InteractableLabelChangedEvent>(HandleLabelChanged);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<InteractionTargetChangedEvent>(HandleTargetChanged);
            EventBus.Unsubscribe<InteractableLabelChangedEvent>(HandleLabelChanged);
        }

        private void Start() => SetVisible(false);

        private void HandleTargetChanged(InteractionTargetChangedEvent evt)
        {
            _currentTarget = evt.Current;

            if (evt.Current == null)
            {
                SetVisible(false);
                return;
            }

            RenderLabel();
            SetVisible(true);
        }

        private void HandleLabelChanged(InteractableLabelChangedEvent evt)
        {
            if (evt.Target != _currentTarget) return;
            RenderLabel();
        }

        private void RenderLabel()
        {
            if (labelText != null && _currentTarget != null)
                labelText.text = "E  " + _currentTarget.DisplayLabel + "...";
        }

        private void SetVisible(bool value)
        {
            if (promptRoot != null) promptRoot.SetActive(value);
        }
    }
}