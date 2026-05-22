using UnityEngine;
using PlanZ.Events;
using PlanZ.Player.Config;
using PlanZ.Player.Events;

namespace PlanZ.Player.Components
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerGroundCheck : MonoBehaviour
    {
        [SerializeField] private MovementConfig config;
        [SerializeField] private bool drawGizmos = true;

        private const float HalfHeight = 0.5f;
        private static readonly Color GizmoGroundedColor = new(0f, 1f, 0f, 0.4f);
        private static readonly Color GizmoAirborneColor = new(1f, 0f, 0f, 0.4f);

        private CharacterController _controller;
        private float _currentCheckRange;
        private bool _wasGrounded;

        public bool IsGrounded { get; private set; }

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _currentCheckRange = config.GroundCheckRange;

            // Prime the grounded state before any FixedUpdate runs so locomotion sees the correct
            // value on the first physics step. Without this, IsGrounded defaults to false and the
            // first frames of gravity accumulate, causing a visible drop before the ground check
            // catches up.
            IsGrounded = SphereCastDown();
            _wasGrounded = IsGrounded;
        }

        private void OnEnable() => EventBus.Subscribe<PlayerCrouchStateChangedEvent>(HandleCrouchChanged);

        private void OnDisable() => EventBus.Unsubscribe<PlayerCrouchStateChangedEvent>(HandleCrouchChanged);

        private void Update()
        {
            IsGrounded = SphereCastDown();
            if (IsGrounded == _wasGrounded) return;

            _wasGrounded = IsGrounded;
            EventBus.Publish(new PlayerGroundStateChangedEvent(IsGrounded));

            if (IsGrounded)
                EventBus.Publish(new PlayerLandedEvent());
        }

        public bool IsOnSlope()
        {
            Vector3 origin = GetCastOrigin();
            if (!Physics.Raycast(origin, Vector3.down, out RaycastHit hit,
                config.SlopeRayLength, config.GroundLayers, QueryTriggerInteraction.Ignore))
                return false;

            return hit.normal != Vector3.up;
        }

        private bool SphereCastDown()
        {
            Vector3 origin = GetCastOrigin();
            return Physics.SphereCast(origin, config.GroundCheckRadius, -transform.up,
                out _, _currentCheckRange, config.GroundLayers, QueryTriggerInteraction.Ignore);
        }

        // The cast originates slightly above the controller's bottom so the sphere is fully outside
        // the controller's own collider at the start. Without this offset, the cast can start
        // already overlapping the floor and behave inconsistently across slopes.
        private Vector3 GetCastOrigin()
        {
            Vector3 bottomLocal = _controller.center - Vector3.up * (_controller.height * config.GroundCheckRange);//HalfHeight);
            Vector3 bottomWorld = transform.position + transform.TransformVector(bottomLocal);
            return bottomWorld + transform.up * config.GroundCheckRadius;
        }

        private void HandleCrouchChanged(PlayerCrouchStateChangedEvent evt)
        {
            _currentCheckRange = evt.IsCrouched ? config.CrouchGroundCheckRange : config.GroundCheckRange;
        }

        private void OnDrawGizmosSelected()
        {
            if (!drawGizmos) return;
            if (config == null) return;

            if (_controller == null) _controller = GetComponent<CharacterController>();
            if (_controller == null) return;

            float range = Application.isPlaying ? _currentCheckRange : config.GroundCheckRange;
            Gizmos.color = Application.isPlaying && IsGrounded ? GizmoGroundedColor : GizmoAirborneColor;

            Vector3 origin = GetCastOrigin();
            Vector3 endpoint = origin - transform.up * range;

            Gizmos.DrawWireSphere(origin, config.GroundCheckRadius);
            Gizmos.DrawWireSphere(endpoint, config.GroundCheckRadius);
            Gizmos.DrawLine(origin, endpoint);
        }
    }
}