using UnityEngine;
using PlanZ.Events;
using PlanZ.Player.Input;
using PlanZ.Weapons.Events;

namespace PlanZ.Weapons.Components
{
    // Adds a subtle roll (Z-axis tilt) to the viewmodel based on strafe input. Sidestep right and
    // the weapon rolls slightly left, like a real object responding to lateral acceleration.
    // Lives on a separate pivot from WeaponSway so the two effects compose instead of overwriting
    // each other's localRotation.
    public class WeaponMovementTilt : MonoBehaviour
    {
        [Header("Tilt amount (degrees of roll at full strafe)")]
        [SerializeField] private float maxTiltAngle = 4f;

        [Header("Smoothing")]
        [SerializeField] private float tiltResponseSpeed = 10f;
        [SerializeField] private float returnSpeed = 8f;

        [Header("Aim damping")]
        [SerializeField, Range(0f, 1f)] private float aimedTiltMultiplier = 0.3f;

        [Header("Direction")]
        [SerializeField] private bool invertDirection;

        private Quaternion _restRotation;
        private float _currentTilt;
        private float _targetTilt;
        private bool _isAiming;

        private void Awake() => _restRotation = transform.localRotation;

        private void OnEnable() => EventBus.Subscribe<WeaponAimStateChangedEvent>(HandleAimStateChanged);

        private void OnDisable() => EventBus.Unsubscribe<WeaponAimStateChangedEvent>(HandleAimStateChanged);

        // LateUpdate so this runs after PlayerLook and any other rotation writers, keeping the
        // tilt as the final composed rotation on this pivot.
        private void LateUpdate()
        {
            if (PlayerInput.Instance == null) return;

            ResolveTargetTilt();
            InterpolateCurrentTilt();
            ApplyRotation();
        }

        private void ResolveTargetTilt()
        {
            float strafe = PlayerInput.Instance.MoveAxis.x;
            float multiplier = _isAiming ? aimedTiltMultiplier : 1f;
            float direction = invertDirection ? 1f : -1f;

            _targetTilt = strafe * maxTiltAngle * multiplier * direction;
        }

        // Asymmetric speeds: react quickly to fresh strafe input, recover gently when input stops.
        // The recovery feels lighter than the engagement, matching the perceived weight of holding
        // a real weapon during lateral movement.
        private void InterpolateCurrentTilt()
        {
            float responseRate = Mathf.Abs(_targetTilt) > Mathf.Abs(_currentTilt)
                ? tiltResponseSpeed
                : returnSpeed;

            _currentTilt = Mathf.Lerp(_currentTilt, _targetTilt, responseRate * Time.deltaTime);
        }

        private void ApplyRotation()
        {
            Quaternion tiltRotation = Quaternion.Euler(0f, 0f, _currentTilt);
            transform.localRotation = _restRotation * tiltRotation;
        }

        private void HandleAimStateChanged(WeaponAimStateChangedEvent evt) => _isAiming = evt.IsAiming;
    }
}
