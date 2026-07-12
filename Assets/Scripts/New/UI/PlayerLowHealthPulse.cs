using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using PlanZ.Events;
using PlanZ.Player.Events;

namespace PlanZ.UI
{
    // Pulses a target Image (typically a red overlay separate from the damage vignette) when
    // player HP falls below a threshold. The pulse intensifies as HP gets lower - at the
    // threshold the alpha barely visible, near death it's fully visible. Stops cleanly when
    // health recovers above the threshold.
    public class PlayerLowHealthPulse : MonoBehaviour
    {
        [SerializeField] private Image pulseImage;

        [Header("Activation")]
        [SerializeField, Range(0f, 1f)] private float activationThreshold = 0.3f;

        [Header("Intensity")]
        [SerializeField] private float minAlpha = 0.1f;
        [SerializeField] private float maxAlphaAtZero = 0.6f;

        [Header("Pulse rhythm")]
        [SerializeField] private float pulseDuration = 0.7f;
        [SerializeField] private Ease pulseEase = Ease.InOutSine;

        private Tween _pulseTween;
        private bool _isActive;
        private float _currentPeakAlpha;

        private const float PeakChangeThreshold = 0.05f;

        private void Awake() => SetAlpha(0f);

        private void OnEnable() => EventBus.Subscribe<PlayerHealthChangedEvent>(HandleHealthChanged);

        private void OnDisable()
        {
            EventBus.Unsubscribe<PlayerHealthChangedEvent>(HandleHealthChanged);
            StopPulse();
        }

        private void HandleHealthChanged(PlayerHealthChangedEvent evt)
        {
            bool shouldBeActive = evt.Normalized > 0f && evt.Normalized <= activationThreshold;

            if (shouldBeActive)
                UpdatePulseIntensity(evt.Normalized);
            else
                StopPulse();
        }

        private void UpdatePulseIntensity(float normalized)
        {
            float dangerLevel = 1f - (normalized / activationThreshold);
            float peakAlpha = Mathf.Lerp(minAlpha, maxAlphaAtZero, dangerLevel);

            // Only restart the tween when the peak changes meaningfully. Otherwise consecutive
            // tiny damage events (DOT, multiple zombies tickling) would constantly restart the
            // pulse and ruin its rhythm.
            if (_isActive && Mathf.Abs(peakAlpha - _currentPeakAlpha) < PeakChangeThreshold)
                return;

            StartPulse(peakAlpha);
        }

        private void StartPulse(float peakAlpha)
        {
            if (pulseImage == null) return;

            _pulseTween?.Kill();
            _pulseTween = pulseImage.DOFade(peakAlpha, pulseDuration)
                .SetEase(pulseEase)
                .SetLoops(-1, LoopType.Yoyo);

            _isActive = true;
            _currentPeakAlpha = peakAlpha;
        }

        private void StopPulse()
        {
            if (!_isActive) return;

            _pulseTween?.Kill();
            SetAlpha(0f);
            _isActive = false;
        }

        private void SetAlpha(float alpha)
        {
            if (pulseImage == null) return;
            Color c = pulseImage.color;
            c.a = alpha;
            pulseImage.color = c;
        }
    }
}
