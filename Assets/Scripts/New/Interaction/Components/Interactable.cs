using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using PlanZ.Events;
using PlanZ.Interaction.Data;
using PlanZ.Interaction.Events;

namespace PlanZ.Interaction.Components
{
    public class Interactable : MonoBehaviour
    {
        [SerializeField] private InteractionData data;
        [SerializeField] private bool enabledAtStart = true;

        [Header("Inspector hook (runs once per completion)")]
        [SerializeField] private UnityEvent onCompletedInspectorHook;

        private float _progress;
        private bool _isActive;
        private bool _inCooldown;
        private bool _exhausted;
        private bool _interactionBlocked;
        private string _overrideLabel;
        private Coroutine _cooldownRoutine;
        private Coroutine _delayedEventRoutine;

        public InteractionData Data => data;
        public float Progress => _progress;
        public bool IsActive => _isActive;

        // The raycaster uses this to decide whether to surface the object as the hover target.
        // Even when CanInteract is false (e.g. ammo box at full capacity), the object can still
        // be detectable so the HUD can explain WHY interaction is unavailable.
        public bool IsDetectable => !_exhausted && enabledAtStart;

        // The controller uses this to gate the actual interaction. A detectable but non-interactable
        // object shows a prompt (often with an OverrideLabel like "Full" or "Locked") but rejects
        // any attempt to begin the interaction.
        public bool CanInteract => IsDetectable && !_inCooldown && !_interactionBlocked;

        // Allows the data-driven prompt label to be overridden at runtime. Used by pickup scripts
        // to show contextual messages without duplicating InteractionData assets per state.
        public string DisplayLabel => string.IsNullOrEmpty(_overrideLabel) ? data.PromptLabel : _overrideLabel;

        public void SetEnabled(bool value) => enabledAtStart = value;
        public void ResetExhaustion() => _exhausted = false;

        // External systems call this to make the object detectable but not actionable. The HUD
        // will show DisplayLabel; pressing the key has no effect. Pair with SetOverrideLabel
        // to communicate the reason.
        public void SetInteractionBlocked(bool value) => _interactionBlocked = value;

        public void SetOverrideLabel(string label)
        {
            if (_overrideLabel == label) return;
            _overrideLabel = label;
            EventBus.Publish(new InteractableLabelChangedEvent(this));
        }

        public void ClearOverrideLabel()
        {
            if (string.IsNullOrEmpty(_overrideLabel)) return;
            _overrideLabel = null;
            EventBus.Publish(new InteractableLabelChangedEvent(this));
        }

        public bool Begin()
        {
            if (!CanInteract) return false;
            if (_isActive) return true;

            if (data.Mode == InteractionMode.Quick)
            {
                CompleteImmediately();
                return false;
            }

            _isActive = true;
            EventBus.Publish(new InteractionStartedEvent(this));
            return true;
        }

        public bool Tick(float deltaTime)
        {
            if (!_isActive) return false;
            if (data.Mode == InteractionMode.Quick) return false;

            _progress = Mathf.Clamp01(_progress + deltaTime / data.Duration);
            EventBus.Publish(new InteractionProgressedEvent(this, _progress));

            if (_progress >= 1f)
            {
                CompleteFromProgress();
                return false;
            }

            return true;
        }

        public void Cancel()
        {
            if (!_isActive) return;

            _isActive = false;

            if (data.Mode == InteractionMode.Hold)
                _progress = 0f;

            EventBus.Publish(new InteractionCancelledEvent(this));
        }

        private void CompleteImmediately()
        {
            EventBus.Publish(new InteractionStartedEvent(this));
            FinalizeCompletion();
        }

        private void CompleteFromProgress()
        {
            _isActive = false;
            _progress = 0f;
            FinalizeCompletion();
        }

        private void FinalizeCompletion()
        {
            EventBus.Publish(new InteractionCompletedEvent(this));
            onCompletedInspectorHook?.Invoke();

            if (data.SingleUse) _exhausted = true;

            StartCooldown();
            StartDelayedEvent();
        }

        private void StartCooldown()
        {
            if (data.CooldownAfterComplete <= 0f) return;

            if (_cooldownRoutine != null) StopCoroutine(_cooldownRoutine);
            _cooldownRoutine = StartCoroutine(CooldownRoutine());
        }

        private IEnumerator CooldownRoutine()
        {
            _inCooldown = true;
            yield return new WaitForSeconds(data.CooldownAfterComplete);
            _inCooldown = false;
            _cooldownRoutine = null;
        }

        private void StartDelayedEvent()
        {
            if (data.CompletedEventDelay <= 0f) return;

            if (_delayedEventRoutine != null) StopCoroutine(_delayedEventRoutine);
            _delayedEventRoutine = StartCoroutine(DelayedEventRoutine());
        }

        private IEnumerator DelayedEventRoutine()
        {
            yield return new WaitForSeconds(data.CompletedEventDelay);
            EventBus.Publish(new InteractionCompletedEvent(this));
            _delayedEventRoutine = null;
        }

        private void OnDisable()
        {
            if (_isActive) Cancel();
            if (_cooldownRoutine != null) StopCoroutine(_cooldownRoutine);
            if (_delayedEventRoutine != null) StopCoroutine(_delayedEventRoutine);
        }
    }
}