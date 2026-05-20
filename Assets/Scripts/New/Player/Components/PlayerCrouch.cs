using UnityEngine;
using PlanZ.Events;
using PlanZ.Player.Config;
using PlanZ.Player.Events;
using PlanZ.Player.Input;

namespace PlanZ.Player.Components
{
    [RequireComponent(typeof(PlayerLocomotion))]
    public class PlayerCrouch : MonoBehaviour
    {
        [SerializeField] private MovementConfig config;

        private PlayerLocomotion _locomotion;
        private CharacterController _controller;
        private float _originalHeight;

        public bool IsCrouched { get; private set; }

        private void Awake()
        {
            _locomotion = GetComponent<PlayerLocomotion>();
            _controller = _locomotion.Controller;
            _originalHeight = _controller.height;
        }

        private void FixedUpdate()
        {
            if (_locomotion.Locks.CantCrouch) return;

            bool wantsToCrouch = PlayerInput.Instance.CrouchHeld;

            if (wantsToCrouch && !IsCrouched)
                EnterCrouch();
            else if (!wantsToCrouch && IsCrouched && !HasCeilingAbove())
                ExitCrouch();
        }

        private void EnterCrouch()
        {
            _controller.height = config.CrouchHeight;
            IsCrouched = true;
            EventBus.Publish(new PlayerCrouchStateChangedEvent(true));
        }

        private void ExitCrouch()
        {
            _controller.height = _originalHeight;
            _controller.Move(Vector3.down * config.CrouchExitDownNudge);
            IsCrouched = false;
            EventBus.Publish(new PlayerCrouchStateChangedEvent(false));
        }

        private bool HasCeilingAbove()
        {
            return Physics.SphereCast(transform.position, config.CeilingCheckRadius, transform.up,
                out _, config.CeilingCheckDistance, config.GroundLayers, QueryTriggerInteraction.Ignore);
        }
    }
}
