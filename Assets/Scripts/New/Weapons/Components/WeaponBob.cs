using UnityEngine;
using PlanZ.Events;
using PlanZ.Player.Events;
using PlanZ.Player.Input;
using PlanZ.Weapons.Events;

namespace PlanZ.Weapons.Components
{
    // Sinusoidal position offset that creates the classic FPS weapon bob. Walking drives a strong
    // figure-8 pattern (Y oscillates at full frequency, X at half), idle uses a subtler version
    // for breathing, and sprinting amplifies both amplitude and frequency. Aiming suppresses the
    // entire effect so the player can shoot precisely.
    public class WeaponBob : MonoBehaviour
    {
        [Header("Walk bob")]
        [SerializeField] private float walkAmplitudeX = 0.04f;
        [SerializeField] private float walkAmplitudeY = 0.03f;
        [SerializeField] private float walkFrequency = 8f;

        [Header("Sprint multipliers")]
        [SerializeField] private float sprintAmplitudeMultiplier = 1.5f;
        [SerializeField] private float sprintFrequencyMultiplier = 1.4f;

        [Header("Idle bob (breathing)")]
        [SerializeField] private float idleAmplitudeX = 0.005f;
        [SerializeField] private float idleAmplitudeY = 0.008f;
        [SerializeField] private float idleFrequency = 1.5f;

        [Header("Smoothing")]
        [SerializeField] private float transitionSpeed = 6f;

        [Header("Aim suppression")]
        [SerializeField, Range(0f, 1f)] private float aimedMultiplier = 0f;

        private Vector3 _restPosition;
        private float _phase;
        private Vector3 _currentOffset;
        private bool _isSprinting;
        private bool _isAiming;

        private void Awake() => _restPosition = transform.localPosition;

        private void OnEnable()
        {
            EventBus.Subscribe<PlayerSprintStateChangedEvent>(HandleSprintChanged);
            EventBus.Subscribe<WeaponAimStateChangedEvent>(HandleAimChanged);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<PlayerSprintStateChangedEvent>(HandleSprintChanged);
            EventBus.Unsubscribe<WeaponAimStateChangedEvent>(HandleAimChanged);
        }

        private void Update()
        {
            if (PlayerInput.Instance == null) return;

            Vector3 target = CalculateTargetOffset();

            // Lerping the offset (not the phase) prevents harsh jumps when switching between
            // walk and idle. The phase keeps advancing continuously so the sine wave's position
            // never resets - only its amplitude smoothly transitions.
            _currentOffset = Vector3.Lerp(_currentOffset, target, transitionSpeed * Time.deltaTime);
            transform.localPosition = _restPosition + _currentOffset;
        }

        private Vector3 CalculateTargetOffset()
        {
            bool isMoving = PlayerInput.Instance.MoveAxis.sqrMagnitude > 0.01f;
            float aimMultiplier = _isAiming ? aimedMultiplier : 1f;

            // Aim suppression also freezes the phase so when ADS is released, the bob resumes
            // smoothly from where it would naturally be, not from a stale position.
            if (aimMultiplier <= 0f) return Vector3.zero;

            (float amplitudeX, float amplitudeY, float frequency) = ResolveBobParams(isMoving);

            _phase += frequency * Time.deltaTime;

            // Figure-8 pattern: X frequency is half of Y, so each full vertical oscillation
            // corresponds to one horizontal cycle, matching natural human gait.
            float offsetX = Mathf.Sin(_phase * 0.5f) * amplitudeX;
            float offsetY = Mathf.Sin(_phase) * amplitudeY;

            return new Vector3(offsetX, offsetY, 0f) * aimMultiplier;
        }

        private (float ampX, float ampY, float freq) ResolveBobParams(bool isMoving)
        {
            if (!isMoving)
                return (idleAmplitudeX, idleAmplitudeY, idleFrequency);

            float sprintMul = _isSprinting ? sprintAmplitudeMultiplier : 1f;
            float freqMul = _isSprinting ? sprintFrequencyMultiplier : 1f;

            return (walkAmplitudeX * sprintMul, walkAmplitudeY * sprintMul, walkFrequency * freqMul);
        }

        private void HandleSprintChanged(PlayerSprintStateChangedEvent evt) => _isSprinting = evt.IsSprinting;

        private void HandleAimChanged(WeaponAimStateChangedEvent evt) => _isAiming = evt.IsAiming;
    }
}
