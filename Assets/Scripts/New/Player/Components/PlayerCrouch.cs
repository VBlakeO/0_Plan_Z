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
        [SerializeField] private Animator anim;


        private const float HalfHeight = 0.5f;
        private const float HeightSnapEpsilon = 0.001f;

        private PlayerLocomotion _locomotion;
        private CharacterController _controller;

        private float _standingHeight;
        private Vector3 _standingCenter;
        private float _targetHeight;
        private bool _hasPublishedState;

        public bool IsCrouched { get; private set; }

        private void Awake()
        {
            _locomotion = GetComponent<PlayerLocomotion>();
            _controller = _locomotion.Controller;
            _standingHeight = _controller.height;
            _standingCenter = _controller.center;
            _targetHeight = _standingHeight;
        }

        private void Update()
        {
            if (_locomotion.Locks.CantCrouch) return;

            anim.SetBool("IsCrouch", IsCrouched);

            ResolveDesiredState();
            InterpolateHeight();

        }

        // Splits intent from execution: ResolveDesiredState only flips IsCrouched and publishes the
        // state event; InterpolateHeight runs every frame to smoothly animate the controller's
        // dimensions toward the current target.
        private void ResolveDesiredState()
        {
            bool wantsToCrouch = PlayerInput.Instance.CrouchHeld;

            if (wantsToCrouch && !IsCrouched)
            {
                IsCrouched = true;
                _targetHeight = config.CrouchHeight;
                EventBus.Publish(new PlayerCrouchStateChangedEvent(true));
                return;
            }

            if (!wantsToCrouch && IsCrouched && !HasCeilingAbove())
            {
                IsCrouched = false;
                _targetHeight = _standingHeight;
                EventBus.Publish(new PlayerCrouchStateChangedEvent(false));
            }
        }

        private void InterpolateHeight()
        {
            if (Mathf.Approximately(_controller.height, _targetHeight)) return;

            float newHeight = Mathf.MoveTowards(_controller.height, _targetHeight,
                config.CrouchTransitionSpeed * Time.deltaTime);

            ApplyHeight(newHeight);

            if (Mathf.Abs(newHeight - _targetHeight) < HeightSnapEpsilon)
                ApplyHeight(_targetHeight);
        }

        // Adjusts both height and center so the bottom of the capsule stays at the same world
        // position. Without compensating the center, lowering the height makes the capsule shrink
        // around its middle (head and feet pull together) instead of "ducking down".
        private void ApplyHeight(float height)
        {
            float standingBottomOffset = _standingCenter.y - _standingHeight * HalfHeight;
            float newCenterY = standingBottomOffset + height * HalfHeight;

            _controller.height = height;
            _controller.center = new Vector3(_standingCenter.x, newCenterY, _standingCenter.z);
        }

        private bool HasCeilingAbove()
        {
            return Physics.SphereCast(transform.position, config.CeilingCheckRadius, transform.up,
                out _, config.CeilingCheckDistance, config.GroundLayers, QueryTriggerInteraction.Ignore);
        }
    }
}