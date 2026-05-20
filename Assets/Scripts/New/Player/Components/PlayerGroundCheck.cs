using UnityEngine;
using PlanZ.Events;
using PlanZ.Player.Config;
using PlanZ.Player.Events;

namespace PlanZ.Player.Components
{
    public class PlayerGroundCheck : MonoBehaviour
    {
        [SerializeField] private MovementConfig config;

        private float _currentCheckRange;
        private bool _wasGrounded;

        public bool IsGrounded { get; private set; }

        private void Awake() => _currentCheckRange = config.GroundCheckRange;

        private void OnEnable() => EventBus.Subscribe<PlayerCrouchStateChangedEvent>(HandleCrouchChanged);

        private void OnDisable() => EventBus.Unsubscribe<PlayerCrouchStateChangedEvent>(HandleCrouchChanged);

        private void Update()
        {
            IsGrounded = SphereCastDown();
            if (IsGrounded == _wasGrounded) return;

            _wasGrounded = IsGrounded;
            EventBus.Publish(new PlayerGroundStateChangedEvent(IsGrounded));
            EventBus.Publish(IsGrounded ? (IEvent)new PlayerLandedEvent() : new PlayerJumpedEvent());
        }

        public bool IsOnSlope()
        {
            if (!Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit,
                config.SlopeRayLength, config.GroundLayers, QueryTriggerInteraction.Ignore))
                return false;

            return hit.normal != Vector3.up;
        }

        private bool SphereCastDown()
        {
            return Physics.SphereCast(transform.position, config.GroundCheckRadius, -transform.up,
                out _, _currentCheckRange, config.GroundLayers, QueryTriggerInteraction.Ignore);
        }

        private void HandleCrouchChanged(PlayerCrouchStateChangedEvent evt)
        {
            _currentCheckRange = evt.IsCrouched ? config.CrouchGroundCheckRange : config.GroundCheckRange;
        }
    }
}
