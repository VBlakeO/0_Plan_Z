using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using PlanZ.Events;
using PlanZ.Player.Events;

namespace PlanZ.UI
{

    public class PlayerHealthBarHUD : MonoBehaviour
    {
        [Header("Foreground (main bar)")]
        [SerializeField] private Image foregroundFill;

        [Header("Background (delayed fill, Overwatch-style)")]
        [SerializeField] private Image backgroundFill;
        [SerializeField] private float damageDelay = 0.3f;
        [SerializeField] private float damageCatchupDuration = 0.5f;
        [SerializeField] private Ease damageCatchupEase = Ease.OutQuad;

        [Header("Foreground motion")]
        [SerializeField] private float foregroundTweenDuration = 0.15f;
        [SerializeField] private Ease foregroundEase = Ease.OutQuad;

        [Header("Numeric label")]
        [SerializeField] private TextMeshProUGUI healthLabel;

        private float _currentNormalized = 1f;
        private Tween _foregroundTween;
        private Tween _backgroundTween;

        private void OnEnable() => EventBus.Subscribe<PlayerHealthChangedEvent>(HandleHealthChanged);

        private void OnDisable()
        {
            EventBus.Unsubscribe<PlayerHealthChangedEvent>(HandleHealthChanged);
            KillTweens();
        }

        private void HandleHealthChanged(PlayerHealthChangedEvent evt)
        {
            UpdateLabel(evt.Current, evt.Max);

            bool isDamage = evt.Normalized < _currentNormalized;
            _currentNormalized = evt.Normalized;

            AnimateForeground(evt.Normalized);

            if (isDamage)
                AnimateBackgroundOnDamage(evt.Normalized);
            else
                AnimateBackgroundOnHeal(evt.Normalized);
        }

        private void UpdateLabel(float current, float max)
        {
            if (healthLabel == null) return;
            healthLabel.text = $"{ (int)((current/max)*100) }%";
        }

        private void AnimateForeground(float target)
        {
            if (foregroundFill == null) return;

            _foregroundTween?.Kill();
            _foregroundTween = foregroundFill.DOFillAmount(target, foregroundTweenDuration)
                .SetEase(foregroundEase);
        }

        // On damage: stay at the previous value for damageDelay seconds, then catch up. The
        // killed tween ensures consecutive hits restart the delay timer (each new hit gives
        // the player a fresh window to see what's happening).
        private void AnimateBackgroundOnDamage(float target)
        {
            if (backgroundFill == null) return;

            _backgroundTween?.Kill();
            _backgroundTween = DOVirtual.DelayedCall(damageDelay, () =>
            {
                _backgroundTween = backgroundFill.DOFillAmount(target, damageCatchupDuration)
                    .SetEase(damageCatchupEase);
            });
        }

        // On heal, the ghost should catch up immediately so the player sees the gain reflected
        // on both bars at once.
        private void AnimateBackgroundOnHeal(float target)
        {
            if (backgroundFill == null) return;

            _backgroundTween?.Kill();
            _backgroundTween = backgroundFill.DOFillAmount(target, foregroundTweenDuration)
                .SetEase(foregroundEase);
        }

        private void KillTweens()
        {
            _foregroundTween?.Kill();
            _backgroundTween?.Kill();
        }
    }
}
