using UnityEngine;
using PlanZ.Events;
using PlanZ.Player.Config;
using PlanZ.Player.Events;
using PlanZ.Player.Input;

namespace PlanZ.Player.Components
{
    [RequireComponent(typeof(PlayerLocomotion))]
    [RequireComponent(typeof(PlayerCrouch))]
    public class PlayerSprint : MonoBehaviour
    {
        [SerializeField] private MovementConfig config;

        private const float ForwardInputThreshold = 0.1f;

        private PlayerLocomotion _locomotion;
        private PlayerCrouch _crouch;
        private bool _isZoomed;

        public bool IsSprinting { get; private set; }

        private void Awake()
        {
            _locomotion = GetComponent<PlayerLocomotion>();
            _crouch = GetComponent<PlayerCrouch>();
        }

        private void OnEnable() => EventBus.Subscribe<PlayerZoomStateChangedEvent>(HandleZoomChanged);

        private void OnDisable() => EventBus.Unsubscribe<PlayerZoomStateChangedEvent>(HandleZoomChanged);

        private void Update()
        {
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
            if (config.RequireForwardInputToSprint && PlayerInput.Instance.MoveAxis.y <= ForwardInputThreshold) return false;
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
