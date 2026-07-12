using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using PlanZ.Events;
using PlanZ.Player.Events;

namespace PlanZ.UI
{
    // Red screen vignette that pulses when the player takes damage. Stronger hits produce a
    // more opaque flash; the vignette then fades back to transparent. Intensity scales with
    // the damage amount so a paper cut feels different from a shotgun blast.
    public class PlayerDamageVignette : MonoBehaviour
    {
        [SerializeField] private Image vignetteImage;

        [Header("Intensity")]
        [SerializeField] private float minAlpha = 0.2f;
        [SerializeField] private float maxAlpha = 0.7f;
        [SerializeField] private float damageReferenceAmount = 30f;

        [Header("Timing")]
        [SerializeField] private float fadeInDuration = 0.08f;
        [SerializeField] private float fadeOutDuration = 0.6f;
        [SerializeField] private Ease fadeOutEase = Ease.OutQuad;

        private Tween _vignetteTween;

        private void Awake() => SetAlpha(0f);

        private void OnEnable() => EventBus.Subscribe<PlayerDamagedEvent>(HandleDamaged);

        private void OnDisable()
        {
            EventBus.Unsubscribe<PlayerDamagedEvent>(HandleDamaged);
            _vignetteTween?.Kill();
        }

        // Alpha is a function of damage relative to a reference value. A hit equal to
        // damageReferenceAmount produces maxAlpha; smaller hits land between min and max
        // proportionally. Clamping prevents exotic damage spikes from going invisible at
        // very low values or saturating instantly.
        private void HandleDamaged(PlayerDamagedEvent evt)
        {
            if (vignetteImage == null) return;

            float t = Mathf.Clamp01(evt.Info.Amount / damageReferenceAmount);
            float targetAlpha = Mathf.Lerp(minAlpha, maxAlpha, t);

            _vignetteTween?.Kill();
            _vignetteTween = DOTween.Sequence()
                .Append(vignetteImage.DOFade(targetAlpha, fadeInDuration))
                .Append(vignetteImage.DOFade(0f, fadeOutDuration).SetEase(fadeOutEase));
        }

        private void SetAlpha(float alpha)
        {
            if (vignetteImage == null) return;
            Color c = vignetteImage.color;
            c.a = alpha;
            vignetteImage.color = c;
        }
    }
}
