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
        [SerializeField] private Animator anim;
        [SerializeField] private PlayerStateLocks locks = new();
        [SerializeField] private CharacterController _controller;

        private const string AnimSpeedX = "SpeedX";
        private const string AnimSpeedY = "SpeedY";
        private const float HalfHeightFactor = 0.5f;
        private const float ZeroVerticalVelocity = 0f;

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

        private void FixedUpdate() => Move();

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
            UpdateAnimator();
        }

        private void ApplySlopeForceIfNeeded(Vector2 input)
        {
            if (input.sqrMagnitude <= 0f) return;
            if (!_groundCheck.IsOnSlope()) return;

            float pushDistance = _controller.height * HalfHeightFactor * config.SlopeForce * Time.deltaTime;
            _controller.Move(Vector3.down * pushDistance);
        }

        private void UpdateAnimator()
        {
            if (anim == null) return;
            anim.SetFloat(AnimSpeedX, _currentDir.x);
            anim.SetFloat(AnimSpeedY, _currentDir.y);
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
