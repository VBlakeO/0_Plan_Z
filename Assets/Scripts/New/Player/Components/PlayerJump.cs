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

        private const float NotPending = -1f;
        private const float NeverLeft = -1f;

        private PlayerLocomotion _locomotion;
        private PlayerGroundCheck _groundCheck;
        private PlayerCrouch _crouch;

        private float _cooldownTimer;
        private float _pendingImpulseAt = NotPending;

        // Time when the player last left the ground WITHOUT jumping (i.e. walked off a ledge).
        // A pending impulse is not enough to qualify because we don't want a double-jump from
        // the same input.
        private float _leftGroundAt = NeverLeft;
        private bool _justJumped;

        private void Awake()
        {
            _locomotion = GetComponent<PlayerLocomotion>();
            _groundCheck = GetComponent<PlayerGroundCheck>();
            _crouch = GetComponent<PlayerCrouch>();
        }

        private void OnEnable()
        {
            EventBus.Subscribe<PlayerJumpRequestedEvent>(HandleJumpRequested);
            EventBus.Subscribe<PlayerGroundStateChangedEvent>(HandleGroundStateChanged);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<PlayerJumpRequestedEvent>(HandleJumpRequested);
            EventBus.Unsubscribe<PlayerGroundStateChangedEvent>(HandleGroundStateChanged);
        }

        private void Update()
        {
            if (_cooldownTimer > 0f)
                _cooldownTimer -= Time.deltaTime;

            TryApplyPendingImpulse();
        }

        // The player can jump if grounded OR within the coyote window after walking off a ledge.
        // The _justJumped flag ensures the coyote grace doesn't accidentally enable a second jump
        // when the player has just left the ground BY jumping.
        private void HandleJumpRequested()
        {
            if (_pendingImpulseAt != NotPending) return;
            if (_locomotion.Locks.CantJump) return;
            if (_crouch.IsCrouched) return;
            if (_cooldownTimer > 0f) return;

            if (!CanJumpNow()) return;

            EventBus.Publish(new PlayerJumpStartedEvent());
            _pendingImpulseAt = Time.time + config.JumpAnticipationDelay;
        }

        private bool CanJumpNow()
        {
            if (_groundCheck.IsGrounded) return true;
            if (_justJumped) return false;
            if (_leftGroundAt < 0f) return false;
            return Time.time - _leftGroundAt <= config.CoyoteTime;
        }

        // Track ground transitions to feed coyote time. Becoming airborne while _justJumped is
        // true means it was a jump (skip coyote eligibility); becoming airborne otherwise means
        // the player fell off a ledge (eligible for coyote). Landing always resets the trackers.
        private void HandleGroundStateChanged(PlayerGroundStateChangedEvent evt)
        {
            if (evt.IsGrounded)
            {
                _leftGroundAt = NeverLeft;
                _justJumped = false;
                return;
            }

            if (_justJumped)
            {
                _leftGroundAt = NeverLeft;
                return;
            }

            _leftGroundAt = Time.time;
        }

        // The impulse fires after the anticipation delay so the visible jump animation has time to
        // wind up (knees bending) before the character actually leaves the ground.
        private void TryApplyPendingImpulse()
        {
            if (_pendingImpulseAt != NotPending && Time.time >= _pendingImpulseAt)
                ApplyImpulseNow();
        }

        // The coyote check is repeated here because between the request and the delayed impulse,
        // the player may have fallen out of the grace window. If so, the jump fizzles - a
        // deliberate trade-off favouring tight controls over leniency.
        private void ApplyImpulseNow()
        {
            _pendingImpulseAt = NotPending;

            if (!CanJumpNow()) return;

            _justJumped = true;
            _leftGroundAt = NeverLeft;

            _locomotion.ApplyJumpImpulse(config.JumpForce);
            _cooldownTimer = config.JumpCooldown;
            EventBus.Publish(new PlayerJumpedEvent());
        }
    }
}