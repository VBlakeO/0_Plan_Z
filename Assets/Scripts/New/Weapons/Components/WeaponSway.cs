using UnityEngine;
using PlanZ.Events;
using PlanZ.Weapons.Events;

namespace PlanZ.Weapons.Components
{
    // Creates the "weapon lag" effect for setups where the weapon viewmodel is rendered by a
    // separate camera and is NOT a child of the Player. This component samples the Player's
    // rotation each frame and translates the angular delta into a counter-rotation on the
    // viewmodel - the weapon visually trails behind the camera and settles back to neutral
    // when rotation stops.
    public class WeaponSway : MonoBehaviour
    {
        [Header("Rotation source")]
        [SerializeField] private Transform rotationSource;

        [Header("Sway intensity (degrees of lag per degree of rotation per frame)")]
        [SerializeField] private float horizontalSwayAmount = 1.5f;
        [SerializeField] private float verticalSwayAmount = 1.5f;

        [Header("Limits (max degrees of visible lag)")]
        [SerializeField] private float maxHorizontalSway = 6f;
        [SerializeField] private float maxVerticalSway = 6f;

        [Header("Smoothing")]
        [SerializeField] private float swayResponseSpeed = 14f;
        [SerializeField] private float returnSpeed = 8f;

        [Header("Aim damping")]
        [SerializeField, Range(0f, 1f)] private float aimedSwayMultiplier = 0.3f;

        private Quaternion _restRotation;
        private Vector2 _currentSway;
        private Vector2 _targetSway;
        private Vector3 _lastSourceEuler;
        private bool _isAiming;
        private bool _hasPriorSample;

        private void Awake() => _restRotation = transform.localRotation;

        private void Start()
        {
            if (rotationSource != null)
            {
                _lastSourceEuler = rotationSource.eulerAngles;
                _hasPriorSample = true;
            }
        }

        private void OnEnable() => EventBus.Subscribe<WeaponAimStateChangedEvent>(HandleAimStateChanged);

        private void OnDisable() => EventBus.Unsubscribe<WeaponAimStateChangedEvent>(HandleAimStateChanged);

        // LateUpdate ensures we sample the rotation source AFTER it has been updated by its own
        // controllers (PlayerLook, AI, etc.) that frame. Sampling in Update would race with them.
        private void LateUpdate()
        {
            if (rotationSource == null) return;

            Vector2 angularDelta = SampleAngularDelta();
            UpdateTargetSway(angularDelta);
            InterpolateCurrentSway();
            ApplyRotation();
        }

        private Vector2 SampleAngularDelta()
        {
            Vector3 currentEuler = rotationSource.eulerAngles;

            if (!_hasPriorSample)
            {
                _lastSourceEuler = currentEuler;
                _hasPriorSample = true;
                return Vector2.zero;
            }

            float deltaYaw = Mathf.DeltaAngle(_lastSourceEuler.y, currentEuler.y);
            float deltaPitch = Mathf.DeltaAngle(_lastSourceEuler.x, currentEuler.x);

            _lastSourceEuler = currentEuler;
            return new Vector2(deltaYaw, deltaPitch);
        }

        // Yaw delta (camera turning right) produces opposite-sign sway on Y (weapon swings left
        // around the world up axis). Pitch delta works the same way on X.
        private void UpdateTargetSway(Vector2 angularDelta)
        {
            float multiplier = _isAiming ? aimedSwayMultiplier : 1f;

            float horizontal = -angularDelta.x * horizontalSwayAmount * multiplier;
            float vertical = angularDelta.y * verticalSwayAmount * multiplier;

            _targetSway += new Vector2(horizontal, vertical);
            _targetSway = new Vector2(
                Mathf.Clamp(_targetSway.x, -maxHorizontalSway, maxHorizontalSway),
                Mathf.Clamp(_targetSway.y, -maxVerticalSway, maxVerticalSway)
            );
        }

        // Asymmetric speeds: respond fast to fresh input, recover gently when input stops. The
        // target decays toward zero every frame so even a held-rotation eventually returns the
        // weapon to neutral instead of leaving it tilted forever.
        private void InterpolateCurrentSway()
        {
            float responseRate = _targetSway.sqrMagnitude > _currentSway.sqrMagnitude
                ? swayResponseSpeed
                : returnSpeed;

            _currentSway = Vector2.Lerp(_currentSway, _targetSway, responseRate * Time.deltaTime);
            _targetSway = Vector2.Lerp(_targetSway, Vector2.zero, returnSpeed * Time.deltaTime);
        }

        private void ApplyRotation()
        {
            Quaternion swayRotation = Quaternion.Euler(_currentSway.y, _currentSway.x, 0f);
            transform.localRotation = _restRotation * swayRotation;
        }

        private void HandleAimStateChanged(WeaponAimStateChangedEvent evt) => _isAiming = evt.IsAiming;
    }
}