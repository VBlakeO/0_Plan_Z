using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using PlanZ.Events;
using PlanZ.Healing.Events;

namespace PlanZ.UI
{
    // Shows a fill bar while the player holds the heal key. Hides on completion and on
    // cancel. Uses DOTween for a brief tween on hide so the bar doesn't snap out abruptly
    // when the player finishes or releases the key.
    public class HealProgressHUD : MonoBehaviour
    {
        [SerializeField] private GameObject barRoot;
        [SerializeField] private Image fillImage;
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Fade")]
        [SerializeField] private float fadeInDuration = 0.1f;
        [SerializeField] private float fadeOutDuration = 0.25f;

        private Tween _fadeTween;

        private void OnEnable()
        {
            EventBus.Subscribe<HealStartedEvent>(HandleStarted);
            EventBus.Subscribe<HealProgressedEvent>(HandleProgressed);
            EventBus.Subscribe<HealCancelledEvent>(HandleCancelled);
            EventBus.Subscribe<HealCompletedEvent>(HandleCompleted);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<HealStartedEvent>(HandleStarted);
            EventBus.Unsubscribe<HealProgressedEvent>(HandleProgressed);
            EventBus.Unsubscribe<HealCancelledEvent>(HandleCancelled);
            EventBus.Unsubscribe<HealCompletedEvent>(HandleCompleted);

            _fadeTween?.Kill();
        }

        private void Start() => HideImmediate();

        private void HandleStarted(HealStartedEvent evt)
        {
            SetFill(0f);
            FadeIn();
        }

        private void HandleProgressed(HealProgressedEvent evt) => SetFill(evt.Progress);

        private void HandleCancelled(HealCancelledEvent evt) => FadeOut();

        private void HandleCompleted(HealCompletedEvent evt)
        {
            SetFill(1f);
            FadeOut();
        }

        private void FadeIn()
        {
            if (barRoot != null) barRoot.SetActive(true);
            if (canvasGroup == null) return;

            _fadeTween?.Kill();
            _fadeTween = canvasGroup.DOFade(1f, fadeInDuration);
        }

        // The bar root is deactivated only after the fade completes, so the player still sees
        // the bar fading out instead of it disappearing instantly on release.
        private void FadeOut()
        {
            if (canvasGroup == null)
            {
                if (barRoot != null) barRoot.SetActive(false);
                return;
            }

            _fadeTween?.Kill();
            _fadeTween = canvasGroup.DOFade(0f, fadeOutDuration)
                .OnComplete(() =>
                {
                    if (barRoot != null) barRoot.SetActive(false);
                });
        }

        private void HideImmediate()
        {
            if (canvasGroup != null) canvasGroup.alpha = 0f;
            if (barRoot != null) barRoot.SetActive(false);
        }

        private void SetFill(float value)
        {
            if (fillImage != null) fillImage.fillAmount = value;
        }
    }
}
