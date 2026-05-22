using UnityEngine;
using PlanZ.Events;

namespace PlanZ.Player.Events
{
    public readonly struct PlayerMoveInputEvent : IEvent
    {
        public Vector2 Direction { get; }
        public PlayerMoveInputEvent(Vector2 direction) => Direction = direction;
    }

    public readonly struct PlayerLookInputEvent : IEvent
    {
        public Vector2 Delta { get; }
        public PlayerLookInputEvent(Vector2 delta) => Delta = delta;
    }

    public readonly struct PlayerScrollInputEvent : IEvent
    {
        public int Direction { get; }
        public PlayerScrollInputEvent(int direction) => Direction = direction;
    }

    public readonly struct PlayerJumpRequestedEvent : IEvent { }

    public readonly struct PlayerJumpStartedEvent : IEvent { }

    public readonly struct PlayerCrouchStateChangedEvent : IEvent
    {
        public bool IsCrouched { get; }
        public PlayerCrouchStateChangedEvent(bool isCrouched) => IsCrouched = isCrouched;
    }

    public readonly struct PlayerSprintStateChangedEvent : IEvent
    {
        public bool IsSprinting { get; }
        public PlayerSprintStateChangedEvent(bool isSprinting) => IsSprinting = isSprinting;
    }

    public readonly struct PlayerZoomStateChangedEvent : IEvent
    {
        public bool IsZoomed { get; }
        public PlayerZoomStateChangedEvent(bool isZoomed) => IsZoomed = isZoomed;
    }

    public readonly struct PlayerGroundStateChangedEvent : IEvent
    {
        public bool IsGrounded { get; }
        public PlayerGroundStateChangedEvent(bool isGrounded) => IsGrounded = isGrounded;
    }

    public readonly struct PlayerJumpedEvent : IEvent { }

    public readonly struct PlayerLandedEvent : IEvent { }

    public readonly struct PlayerLockChangedEvent : IEvent { }
}