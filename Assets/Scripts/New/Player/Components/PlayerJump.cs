using UnityEngine;
using PlanZ.Events;
using PlanZ.Player.Config;
using PlanZ.Player.Events;

namespace PlanZ.Player.Components
{
    [RequireComponent(typeof(PlayerLocomotion))]
    [RequireComponent(typeof(PlayerGroundCheck))]
    [RequireComponent(typeof(PlayerCrouch))]
    public class PlayerJump : MonoBehaviour
    {
        [SerializeField] private MovementConfig config;

        private PlayerLocomotion _locomotion;
        private PlayerGroundCheck _groundCheck;
        private PlayerCrouch _crouch;

        private float _cooldownTimer;

        private void Awake()
        {
            _locomotion = GetComponent<PlayerLocomotion>();
            _groundCheck = GetComponent<PlayerGroundCheck>();
            _crouch = GetComponent<PlayerCrouch>();
        }

        private void OnEnable() => EventBus.Subscribe<PlayerJumpRequestedEvent>(HandleJumpRequested);

        private void OnDisable() => EventBus.Unsubscribe<PlayerJumpRequestedEvent>(HandleJumpRequested);

        private void Update()
        {
            if (_cooldownTimer > 0f)
                _cooldownTimer -= Time.deltaTime;
        }

        private void HandleJumpRequested()
        {
            if (_locomotion.Locks.CantJump) return;
            if (_crouch.IsCrouched) return;
            if (!_groundCheck.IsGrounded) return;
            if (_cooldownTimer > 0f) return;

            _locomotion.ApplyJumpImpulse(config.JumpForce);
            _cooldownTimer = config.JumpCooldown;
            EventBus.Publish(new PlayerJumpedEvent());
        }
    }
}
