using UnityEngine;
using PlanZ.Events;
using PlanZ.Player.Config;
using PlanZ.Player.Events;
using PlanZ.Player.Input;

namespace PlanZ.Player.Components
{
    [RequireComponent(typeof(PlayerLocomotion))]
    [RequireComponent(typeof(PlayerCrouch))]
    [RequireComponent(typeof(PlayerGroundCheck))]
    public class PlayerSprint : MonoBehaviour
    {
        [SerializeField] private MovementConfig config;

        private const float MoveDeadZoneSqr = 0.01f;

        private PlayerLocomotion _locomotion;
        private PlayerCrouch _crouch;
        private PlayerGroundCheck _groundCheck;
        private bool _isZoomed;

        public bool IsSprinting { get; private set; }

        private void Awake()
        {
            _locomotion = GetComponent<PlayerLocomotion>();
            _crouch = GetComponent<PlayerCrouch>();
            _groundCheck = GetComponent<PlayerGroundCheck>();
        }

        private void OnEnable() => EventBus.Subscribe<PlayerZoomStateChangedEvent>(HandleZoomChanged);

        private void OnDisable() => EventBus.Unsubscribe<PlayerZoomStateChangedEvent>(HandleZoomChanged);

        // Sprint state is locked while the player is airborne: the decision to start or stop
        // sprinting can only be made with feet on the ground. This prevents the player from
        // changing horizontal speed mid-jump by toggling Shift, which would otherwise let them
        // "boost" their air movement after the fact.
        private void Update()
        {
            if (!_groundCheck.IsGrounded) return;

            bool shouldSprint = ResolveShouldSprint();
            if (shouldSprint == IsSprinting) return;

            IsSprinting = shouldSprint;
            _locomotion.Speed = ResolveTargetSpeed();
            EventBus.Publish(new PlayerSprintStateChangedEvent(IsSprinting));
        }

        private bool ResolveShouldSprint()
        {
            if (_locomotion.Locks.CantSprint) return false;
            if (!PlayerInput.Instance.SprintHeld) return false;
            if (_crouch.IsCrouched) return false;
            if (config.CancelSprintWhenZoomed && _isZoomed) return false;
            if (PlayerInput.Instance.MoveAxis.sqrMagnitude <= MoveDeadZoneSqr) return false;
            return true;
        }

        private float ResolveTargetSpeed()
        {
            float baseSpeed = IsSprinting ? config.SprintSpeed : config.WalkSpeed;
            return _crouch.IsCrouched ? baseSpeed * config.CrouchSpeedMultiplier : baseSpeed;
        }

        private void HandleZoomChanged(PlayerZoomStateChangedEvent evt) => _isZoomed = evt.IsZoomed;
    }
}