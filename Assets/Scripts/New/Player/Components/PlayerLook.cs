using UnityEngine;
using PlanZ.Events;
using PlanZ.Player.Config;
using PlanZ.Player.Events;

namespace PlanZ.Player.Components
{
    [RequireComponent(typeof(PlayerLocomotion))]
    public class PlayerLook : MonoBehaviour
    {
        [SerializeField] private MovementConfig config;
        [SerializeField] private Transform spineUpper;
        [SerializeField] private Transform spineMiddle;
        [SerializeField] private Transform spineLower;

        private const float YawAxisLock = 0f;

        private PlayerLocomotion _locomotion;
        private float _pitch;
        private float _yaw;

        private Quaternion _spineUpperRest;
        private Quaternion _spineMiddleRest;
        private Quaternion _spineLowerRest;

        public float Pitch => _pitch;
        public float Yaw => _yaw;

        public float HorizontalRecoil { get; set; }
        public float VerticalRecoil { get; set; }

        private void Awake()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            _locomotion = GetComponent<PlayerLocomotion>();
            _yaw = transform.localEulerAngles.y;
            CacheSpineRestPoses();
        }

        private void OnEnable() => EventBus.Subscribe<PlayerLookInputEvent>(HandleLookInput);

        private void OnDisable() => EventBus.Unsubscribe<PlayerLookInputEvent>(HandleLookInput);

        // LateUpdate runs after the Animator writes the pose, so applying the IK delta here ensures
        // it sits on top of the current animation frame instead of being overwritten.
        private void LateUpdate()
        {
            if (!config.SpineIKEnabled) return;
            ApplyPitchToSpine();
        }

        private void HandleLookInput(PlayerLookInputEvent evt)
        {
            if (_locomotion.Locks.CantLook) return;

            _yaw += (evt.Delta.x + HorizontalRecoil) * config.MouseSensitivity;

            float pitchDelta = config.MouseSensitivity * (evt.Delta.y + VerticalRecoil);
            _pitch += config.InvertCamera ? pitchDelta : -pitchDelta;
            _pitch = Mathf.Clamp(_pitch, config.PitchLimits.x, config.PitchLimits.y);

            transform.localEulerAngles = new Vector3(YawAxisLock, _yaw, YawAxisLock);
        }

        private void CacheSpineRestPoses()
        {
            if (spineUpper != null) _spineUpperRest = spineUpper.localRotation;
            if (spineMiddle != null) _spineMiddleRest = spineMiddle.localRotation;
            if (spineLower != null) _spineLowerRest = spineLower.localRotation;
        }

        private void ApplyPitchToSpine()
        {
            ApplyPitchToBone(spineUpper, _spineUpperRest, config.SpineUpperAxis, config.SpineUpperSign, config.SpineUpperWeight, config.SpineUpperAngleLimits);
            ApplyPitchToBone(spineMiddle, _spineMiddleRest, config.SpineMiddleAxis, config.SpineMiddleSign, config.SpineMiddleWeight, config.SpineMiddleAngleLimits);
            ApplyPitchToBone(spineLower, _spineLowerRest, config.SpineLowerAxis, config.SpineLowerSign, config.SpineLowerWeight, config.SpineLowerAngleLimits);
        }

        // The clamp is applied after weight and sign so the limit reflects the actual angle
        // received by the bone, not the raw camera pitch. This allows asymmetric limits
        // (e.g. bone bends more forward than backward) regardless of how weight is distributed.
        private void ApplyPitchToBone(Transform bone, Quaternion restPose, SpineBoneAxis axis, float sign, float weight, Vector2 angleLimits)
        {
            if (bone == null) return;
            if (weight <= 0f) return;

            float angle = _pitch * weight * sign;
            angle = Mathf.Clamp(angle, angleLimits.x, angleLimits.y);

            Quaternion delta = Quaternion.AngleAxis(angle, AxisToVector(axis));
            bone.localRotation = restPose * delta;
        }

        private static Vector3 AxisToVector(SpineBoneAxis axis)
        {
            switch (axis)
            {
                case SpineBoneAxis.Y: return Vector3.up;
                case SpineBoneAxis.Z: return Vector3.forward;
                default: return Vector3.right;
            }
        }
    }
}