using UnityEngine;
using PlanZ.Events;
using PlanZ.Player.Config;
using PlanZ.Player.Events;

namespace PlanZ.Player.Components
{
    // Owns all Animator parameter writes related to locomotion. Other movement components publish
    // events; this one translates them into Animator state. Keeping it here means PlayerLocomotion
    // stays focused on physics and the Animator is decoupled from physics math.
    [RequireComponent(typeof(PlayerLocomotion))]
    public class PlayerLocomotionAnimator : MonoBehaviour
    {
        [SerializeField] private MovementConfig config;
        [SerializeField] private Animator anim;

        private const string AnimSpeedX = "SpeedX";
        private const string AnimSpeedY = "SpeedY";
        private const string AnimLocomotionSpeed = "LocomotionSpeed";

        private static readonly int SpeedXHash = Animator.StringToHash(AnimSpeedX);
        private static readonly int SpeedYHash = Animator.StringToHash(AnimSpeedY);
        private static readonly int LocomotionSpeedHash = Animator.StringToHash(AnimLocomotionSpeed);

        private PlayerLocomotion _locomotion;

        private bool _isSprinting;
        private bool _isCrouched;
        private float _currentSpeedMultiplier;
        private float _targetSpeedMultiplier;

        private void Awake()
        {
            _locomotion = GetComponent<PlayerLocomotion>();
            _currentSpeedMultiplier = config.WalkAnimationSpeed;
            _targetSpeedMultiplier = config.WalkAnimationSpeed;
        }

        private void OnEnable()
        {
            EventBus.Subscribe<PlayerSprintStateChangedEvent>(HandleSprintChanged);
            EventBus.Subscribe<PlayerCrouchStateChangedEvent>(HandleCrouchChanged);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<PlayerSprintStateChangedEvent>(HandleSprintChanged);
            EventBus.Unsubscribe<PlayerCrouchStateChangedEvent>(HandleCrouchChanged);
        }

        private void Update()
        {
            if (anim == null) return;

            WriteDirection();
            WriteSpeedMultiplier();
        }

        private void WriteDirection()
        {
            Vector2 dir = _locomotion.CurrentDirection;
            anim.SetFloat(SpeedXHash, dir.x);
            anim.SetFloat(SpeedYHash, dir.y);
        }

        // Lerp avoids abrupt jumps when starting or stopping a sprint, which would make the legs
        // visibly snap to a faster cadence mid-stride.
        private void WriteSpeedMultiplier()
        {
            _currentSpeedMultiplier = Mathf.Lerp(_currentSpeedMultiplier, _targetSpeedMultiplier,
                config.AnimationSpeedSmoothing * Time.deltaTime);

            anim.SetFloat(LocomotionSpeedHash, _currentSpeedMultiplier);
        }

        private void HandleSprintChanged(PlayerSprintStateChangedEvent evt)
        {
            _isSprinting = evt.IsSprinting;
            RefreshTargetSpeed();
        }

        private void HandleCrouchChanged(PlayerCrouchStateChangedEvent evt)
        {
            _isCrouched = evt.IsCrouched;
            RefreshTargetSpeed();
        }

        private void RefreshTargetSpeed()
        {
            if (_isCrouched)
            {
                _targetSpeedMultiplier = config.CrouchAnimationSpeed;
                return;
            }

            _targetSpeedMultiplier = _isSprinting ? config.SprintAnimationSpeed : config.WalkAnimationSpeed;
        }
    }
}
