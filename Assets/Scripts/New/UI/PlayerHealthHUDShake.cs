using DG.Tweening;
using UnityEngine;
using PlanZ.Events;
using PlanZ.Player.Events;

namespace PlanZ.UI
{
    // Shakes the assigned RectTransform when the player takes damage. Strength scales with
    // damage amount, capping at a configurable maximum to avoid the HUD flying offscreen on
    // big hits. The component snapshots the original anchored position on awake so the
    // shake always settles back exactly where the HUD started, regardless of layout group
    // adjustments at runtime.
    public class PlayerHealthHUDShake : MonoBehaviour
    {
        [SerializeField] private RectTransform shakeTarget;

        [Header("Strength")]
        [SerializeField] private float minStrength = 4f;
        [SerializeField] private float maxStrength = 18f;
        [SerializeField] private float damageReferenceAmount = 30f;

        [Header("Shake parameters")]
        [SerializeField] private float duration = 0.3f;
        [SerializeField] private int vibrato = 14;
        [SerializeField] private float randomness = 90f;

        private Vector2 _restPosition;
        private Tween _shakeTween;

        private void Awake()
        {
            if (shakeTarget != null)
                _restPosition = shakeTarget.anchoredPosition;
        }

        private void OnEnable() => EventBus.Subscribe<PlayerDamagedEvent>(HandleDamaged);

        private void OnDisable()
        {
            EventBus.Unsubscribe<PlayerDamagedEvent>(HandleDamaged);
            _shakeTween?.Kill();

            // Restoring the rest position avoids the HUD staying off-center if the component
            // is disabled mid-shake (e.g. menu opens, scene transitions).
            if (shakeTarget != null) shakeTarget.anchoredPosition = _restPosition;
        }

        private void HandleDamaged(PlayerDamagedEvent evt)
        {
            if (shakeTarget == null) return;

            float t = Mathf.Clamp01(evt.Info.Amount / damageReferenceAmount);
            float strength = Mathf.Lerp(minStrength, maxStrength, t);

            // DOShakeAnchorPos mutates anchoredPosition, so killing the previous tween and
            // resetting to rest before starting a new one prevents drift when hits stack
            // faster than the shake duration.
            _shakeTween?.Kill();
            shakeTarget.anchoredPosition = _restPosition;
            _shakeTween = shakeTarget.DOShakeAnchorPos(duration, strength, vibrato, randomness, false, true)
                .OnComplete(() => shakeTarget.anchoredPosition = _restPosition);
        }
    }
}
