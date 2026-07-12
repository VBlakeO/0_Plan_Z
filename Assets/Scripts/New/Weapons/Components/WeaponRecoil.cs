using UnityEngine;
using PlanZ.Events;
using PlanZ.Player.Components;
using PlanZ.Weapons.Events;

namespace PlanZ.Weapons.Components
{
    [RequireComponent(typeof(WeaponController))]
    [RequireComponent(typeof(WeaponAimer))]
    public class WeaponRecoil : MonoBehaviour
    {
        [SerializeField] private PlayerLook playerLook;

        private const float ZeroRecoil = 0f;
        private const float ResetThresholdSqr = 0.0001f;

        private WeaponController _controller;
        private WeaponAimer _aimer;

        private Vector2 _targetOffset;
        private Vector2 _currentOffset;
        private Vector2 _lastAppliedOffset;
        private int _shotIndex;
        private float _lastShotTime = float.NegativeInfinity;
        private bool _firing;

        private void Awake()
        {
            _controller = GetComponent<WeaponController>();
            _aimer = GetComponent<WeaponAimer>();
        }

        private void OnEnable()
        {
            EventBus.Subscribe<WeaponFiredEvent>(HandleWeaponFired);
            EventBus.Subscribe<WeaponFireReleasedEvent>(HandleFireReleased);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<WeaponFiredEvent>(HandleWeaponFired);
            EventBus.Unsubscribe<WeaponFireReleasedEvent>(HandleFireReleased);
            ResetState();
        }

        private void Update()
        {
            UpdateDecayState();
            InterpolateOffset();
            ApplyOffsetDelta();
        }

        private void HandleWeaponFired(WeaponFiredEvent evt)
        {
            if (evt.Data != _controller.Data) return;
            if (_controller.Data.RecoilPattern == null) return;

            Vector2 kick = _controller.Data.RecoilPattern.GetKick(_shotIndex);
            float multiplier = _controller.Data.RecoilIntensity *
                (_aimer.IsAiming ? _controller.Data.AimedRecoilMultiplier : 1f);

            _targetOffset += kick * multiplier;
            _shotIndex++;
            _lastShotTime = Time.time;
            _firing = true;
        }

        private void HandleFireReleased() => _firing = false;

        // After the reset delay with no firing, the target offset decays to zero. While firing,
        // the target stays where the accumulated kicks placed it, and only the rise speed dictates
        // how fast the camera reaches that target.
        private void UpdateDecayState()
        {
            bool shouldDecay = !_firing && Time.time - _lastShotTime > _controller.Data.RecoilResetDelay;
            if (!shouldDecay) return;

            _targetOffset = Vector2.Lerp(_targetOffset, Vector2.zero,
                _controller.Data.RecoilDecaySpeed * Time.deltaTime);

            if (_targetOffset.sqrMagnitude < ResetThresholdSqr)
            {
                _targetOffset = Vector2.zero;
                _shotIndex = 0;
            }
        }

        private void InterpolateOffset()
        {
            _currentOffset = Vector2.Lerp(_currentOffset, _targetOffset,
                _controller.Data.RecoilRiseSpeed * Time.deltaTime);
        }

        // PlayerLook reads HorizontalRecoil/VerticalRecoil every frame and adds them to the look
        // input multiplied by sensitivity. To avoid the recoil being applied permanently every
        // frame, we feed only the DELTA between frames - the camera moves by the difference, and
        // the next frame the difference will be much smaller as currentOffset approaches target.
        private void ApplyOffsetDelta()
        {
            if (playerLook == null) return;

            Vector2 delta = _currentOffset - _lastAppliedOffset;
            _lastAppliedOffset = _currentOffset;

            playerLook.HorizontalRecoil = delta.x;
            playerLook.VerticalRecoil = delta.y;
        }

        private void ResetState()
        {
            _targetOffset = Vector2.zero;
            _currentOffset = Vector2.zero;
            _lastAppliedOffset = Vector2.zero;
            _shotIndex = 0;
            _firing = false;

            if (playerLook != null)
            {
                playerLook.HorizontalRecoil = ZeroRecoil;
                playerLook.VerticalRecoil = ZeroRecoil;
            }
        }
    }
}