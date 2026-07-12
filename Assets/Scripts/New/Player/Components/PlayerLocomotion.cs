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
        [SerializeField]  private CharacterController _controller;

        private const float HalfHeightFactor = 0.5f;
        private const float ZeroVerticalVelocity = 0f;
        private const float InitialGroundSnapDistance = 5f;

        private PlayerGroundCheck _groundCheck;

        private Vector2 _currentDir;
        private Vector2 _currentDirVelocity;
        private Vector2 _airSnapshotDir;
        private bool _wasGrounded = true;
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
            bool isGrounded = _groundCheck.IsGrounded;

            UpdateAirSnapshot(isGrounded);
            UpdateHorizontalDirection(input, isGrounded);

            if (isGrounded && !_isJumping)
                _verticalVelocity = ZeroVerticalVelocity;

            Vector3 horizontal = (transform.forward * _currentDir.y + transform.right * _currentDir.x) * (Speed * SlowMultiplier);
            Vector3 motion = horizontal + Vector3.up * _verticalVelocity;
            _controller.Move(motion * Time.deltaTime);

            ApplySlopeForceIfNeeded(input);
        }

        // The snapshot freezes the direction the player had at the instant they left the ground.
        // This is what makes air control "partial": the snapshot keeps existing momentum, while
        // current input only pulls the effective direction toward it by AirControlMultiplier.
        // Without the snapshot, releasing WASD mid-jump would zero out horizontal velocity.
        private void UpdateAirSnapshot(bool isGrounded)
        {
            if (_wasGrounded && !isGrounded)
                _airSnapshotDir = _currentDir;

            _wasGrounded = isGrounded;
        }

        // Ground: smooth toward input (existing inertia behaviour).
        // Air: blend snapshot toward input by the configured multiplier. With multiplier 0 the
        // player keeps strictly the snapshot direction; with 1 the player has full air control
        // (equivalent to the original behaviour). 0.3 is a common middle ground.
        private void UpdateHorizontalDirection(Vector2 input, bool isGrounded)
        {
            if (isGrounded)
            {
                _currentDir = Vector2.SmoothDamp(_currentDir, input, ref _currentDirVelocity,
                    config.MoveSmoothTime);
                return;
            }

            _currentDir = Vector2.Lerp(_airSnapshotDir, input, config.AirControlMultiplier);
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

            // The jump itself starts a new air phase; capture the current direction now so the
            // ground frame doesn't have a chance to smoothdamp it back to input first.
            _airSnapshotDir = _currentDir;
        }

        private void HandleJumpStarted() { /* reserved for animation hooks */ }

        private void HandleLanded() => _isJumping = false;
    }
}