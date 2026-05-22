using UnityEngine;
using PlanZ.Events;
using PlanZ.Player.Config;
using PlanZ.Player.Events;
using PlanZ.Player.Input;

namespace PlanZ.Player.Components
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerGroundCheck))]
    public class PlayerLocomotion : BaseMovementSystem
    {
        [SerializeField] private MovementConfig config;
        [SerializeField] private PlayerStateLocks locks = new();
        [SerializeField] private CharacterController _controller;

        private const float HalfHeightFactor = 0.5f;
        private const float ZeroVerticalVelocity = 0f;
        private const float InitialGroundSnapDistance = 5f;

        private PlayerGroundCheck _groundCheck;

        private Vector2 _currentDir;
        private Vector2 _currentDirVelocity;
        private float _verticalVelocity;
        private bool _isJumping;

        public Vector2 CurrentDirection => _currentDir;
        public float VerticalVelocity => _verticalVelocity;
        public PlayerStateLocks Locks => locks;
        public CharacterController Controller => _controller;

        private void Awake()
        {
            _groundCheck = GetComponent<PlayerGroundCheck>();
            Speed = config.WalkSpeed;
        }

        // Snap to ground on Start so the character begins resting on the surface instead of
        // dropping the first few frames. CharacterController.Move with a large downward delta
        // resolves against the floor in a single physics step.
        private void Start() => SnapToGround();

        private void OnEnable()
        {
            EventBus.Subscribe<PlayerJumpedEvent>(HandleJumpStarted);
            EventBus.Subscribe<PlayerLandedEvent>(HandleLanded);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<PlayerJumpedEvent>(HandleJumpStarted);
            EventBus.Unsubscribe<PlayerLandedEvent>(HandleLanded);
        }

        // CharacterController.Move resolves collisions immediately and doesn't depend on physics
        // dynamics, so it runs in Update. This keeps movement in lockstep with the camera (which
        // also updates per-frame), preventing the visible jitter that appears when one runs at
        // the FixedUpdate rate and the other at the render rate.
        private void Update() => Move();

        private void SnapToGround()
        {
            _controller.Move(Vector3.down * InitialGroundSnapDistance);
        }

        private void Move()
        {
            _verticalVelocity += config.Gravity * Time.deltaTime;

            if (locks.CantMove)
            {
                _controller.Move(Vector3.up * _verticalVelocity * Time.deltaTime);
                return;
            }

            Vector2 input = PlayerInput.Instance.MoveAxis.normalized;
            _currentDir = Vector2.SmoothDamp(_currentDir, input, ref _currentDirVelocity, config.MoveSmoothTime);

            if (_groundCheck.IsGrounded && !_isJumping)
                _verticalVelocity = ZeroVerticalVelocity;

            Vector3 horizontal = (transform.forward * _currentDir.y + transform.right * _currentDir.x) * (Speed * SlowMultiplier);
            Vector3 motion = horizontal + Vector3.up * _verticalVelocity;
            _controller.Move(motion * Time.deltaTime);

            ApplySlopeForceIfNeeded(input);
        }

        private void ApplySlopeForceIfNeeded(Vector2 input)
        {
            if (input.sqrMagnitude <= 0f) return;
            if (!_groundCheck.IsOnSlope()) return;

            float pushDistance = _controller.height * HalfHeightFactor * config.SlopeForce * Time.deltaTime;
            _controller.Move(Vector3.down * pushDistance);
        }

        public void ApplyJumpImpulse(float force)
        {
            _verticalVelocity = force;
            _isJumping = true;
        }

        private void HandleJumpStarted() { /* reserved for animation hooks */ }

        private void HandleLanded() => _isJumping = false;
    }
}