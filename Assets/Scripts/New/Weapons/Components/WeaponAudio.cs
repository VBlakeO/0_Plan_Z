using UnityEngine;
using PlanZ.Events;
using PlanZ.Weapons.Data;
using PlanZ.Weapons.Events;

namespace PlanZ.Weapons.Components
{
    [RequireComponent(typeof(WeaponController))]
    public class WeaponAudio : MonoBehaviour
    {
        [SerializeField] private AudioSource generalSource;
        [SerializeField] private AudioSource shootSource;

        private const float DefaultPitch = 1f;

        private WeaponController _controller;

        private void Awake() => _controller = GetComponent<WeaponController>();

        private void OnEnable()
        {
            EventBus.Subscribe<WeaponEquippedEvent>(HandleEquipped);
            EventBus.Subscribe<WeaponFiredEvent>(HandleFired);
            EventBus.Subscribe<WeaponEmptyTriggerEvent>(HandleEmpty);
            EventBus.Subscribe<WeaponReloadStartedEvent>(HandleReloadStarted);
            EventBus.Subscribe<WeaponReloadFinishedEvent>(HandleReloadFinished);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<WeaponEquippedEvent>(HandleEquipped);
            EventBus.Unsubscribe<WeaponFiredEvent>(HandleFired);
            EventBus.Unsubscribe<WeaponEmptyTriggerEvent>(HandleEmpty);
            EventBus.Unsubscribe<WeaponReloadStartedEvent>(HandleReloadStarted);
            EventBus.Unsubscribe<WeaponReloadFinishedEvent>(HandleReloadFinished);
        }

        private void HandleEquipped(WeaponEquippedEvent evt)
        {
            if (evt.Data != _controller.Data) return;
            PlayOn(generalSource, _controller.Data.AudioData?.Equip, DefaultPitch);
        }

        // Shooting uses pitch variation so consecutive shots don't sound identical, which would
        // make automatic fire feel artificial. The range is set per-weapon via WeaponAudioData.
        private void HandleFired(WeaponFiredEvent evt)
        {
            if (evt.Data != _controller.Data) return;

            var audioData = _controller.Data.AudioData;
            if (audioData == null) return;

            float pitch = Random.Range(audioData.ShootPitchRange.x, audioData.ShootPitchRange.y);
            PlayOn(shootSource, audioData.Shoot, pitch);
        }

        private void HandleEmpty(WeaponEmptyTriggerEvent evt)
        {
            if (evt.Data != _controller.Data) return;
            PlayOn(generalSource, _controller.Data.AudioData?.EmptyMag, DefaultPitch);
        }

        private void HandleReloadStarted(WeaponReloadStartedEvent evt)
        {
            if (evt.Data != _controller.Data) return;
            PlayOn(generalSource, _controller.Data.AudioData?.ReloadStart, DefaultPitch);
        }

        private void HandleReloadFinished(WeaponReloadFinishedEvent evt)
        {
            if (evt.Data != _controller.Data) return;
            PlayOn(generalSource, _controller.Data.AudioData?.ReloadEnd, DefaultPitch);
        }

        private static void PlayOn(AudioSource source, WeaponAudioData.Clip? clip, float pitch)
        {
            if (source == null || clip == null) return;
            var c = clip.Value;
            if (c.audioClip == null) return;

            source.pitch = pitch;
            source.volume = c.volume;
            source.PlayOneShot(c.audioClip);
        }
    }
}
