using UnityEngine;
using PlanZ.Events;
using PlanZ.Player.Config;
using PlanZ.Player.Events;
using PlanZ.Player.Input;

namespace PlanZ.Player.Components
{
    public class PlayerZoom : MonoBehaviour
    {
        [SerializeField] private MovementConfig config;
        [SerializeField] private Camera playerCamera;
        [SerializeField] private bool holdToZoom;
        [SerializeField] private bool enableZoom = true;

        private bool _isZoomed;
        private bool _wasZoomHeld;
        private bool _isSprinting;

        private void Awake() => playerCamera.fieldOfView = config.InitialFOV;

        private void OnEnable() => EventBus.Subscribe<PlayerSprintStateChangedEvent>(HandleSprintChanged);

        private void OnDisable() => EventBus.Unsubscribe<PlayerSprintStateChangedEvent>(HandleSprintChanged);

        private void Update()
        {
            if (!enableZoom) return;

            UpdateZoomState();
            UpdateFOV();
        }

        private void UpdateZoomState()
        {
            bool zoomHeld = PlayerInput.Instance.ZoomHeld;
            bool pressedThisFrame = zoomHeld && !_wasZoomHeld;
            bool releasedThisFrame = !zoomHeld && _wasZoomHeld;
            _wasZoomHeld = zoomHeld;

            if (_isSprinting)
            {
                SetZoom(false);
                return;
            }

            if (holdToZoom)
            {
                if (pressedThisFrame) SetZoom(true);
                else if (releasedThisFrame) SetZoom(false);
                return;
            }

            if (pressedThisFrame) SetZoom(!_isZoomed);
        }

        private void UpdateFOV()
        {
            float targetFOV = ResolveTargetFOV();
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFOV, config.FOVLerpSpeed * Time.deltaTime);
        }

        private float ResolveTargetFOV()
        {
            if (_isZoomed) return config.ZoomFOV;
            if (_isSprinting) return config.SprintFOV;
            return config.InitialFOV;
        }

        private void SetZoom(bool value)
        {
            if (_isZoomed == value) return;
            _isZoomed = value;
            EventBus.Publish(new PlayerZoomStateChangedEvent(_isZoomed));
        }

        private void HandleSprintChanged(PlayerSprintStateChangedEvent evt) => _isSprinting = evt.IsSprinting;
    }
}
