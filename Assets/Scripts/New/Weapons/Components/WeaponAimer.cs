using UnityEngine;
using PlanZ.Events;
using PlanZ.Player.Events;
using PlanZ.Player.Input;
using PlanZ.Weapons.Events;

namespace PlanZ.Weapons.Components
{
    [RequireComponent(typeof(WeaponController))]
    public class WeaponAimer : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Camera weaponCamera;
        [SerializeField] private Transform hipBulletPoint;
        [SerializeField] private Transform aimedBulletPoint;
        [SerializeField] private Transform bulletPoint;

        private const float FOVEpsilon = 0.5f;

        private WeaponController _controller;
        private bool _isSprinting;
        private bool _isAiming;

        public bool IsAiming => _isAiming;

        private void Awake() => _controller = GetComponent<WeaponController>();

        private void OnEnable() => EventBus.Subscribe<PlayerSprintStateChangedEvent>(HandleSprintChanged);

        private void OnDisable() => EventBus.Unsubscribe<PlayerSprintStateChangedEvent>(HandleSprintChanged);

        private void Update()
        {
            if (!_controller.IsEquipped) return;

            UpdateAimingState();
            UpdateFOV();
            UpdateBulletPoint();
        }

        private void UpdateAimingState()
        {
            bool shouldAim = PlayerInput.Instance.AimHeld && !_controller.IsReloading && !_isSprinting;
            if (shouldAim == _isAiming) return;

            _isAiming = shouldAim;
            EventBus.Publish(new WeaponAimStateChangedEvent(_controller.Data, _isAiming));
        }

        // Each camera lerps toward its target FOV (aimed vs hip). Two cameras are used because
        // weapon and world tend to render with different FOVs to avoid weapon clipping artifacts.
        private void UpdateFOV()
        {
            float targetMainFOV = _isAiming ? _controller.Data.AimedFOV : _controller.Data.HipFOV;
            float targetWeaponFOV = _isAiming ? _controller.Data.AimedFOV : _controller.Data.HipFOV;

            if (mainCamera != null && Mathf.Abs(mainCamera.fieldOfView - targetMainFOV) > FOVEpsilon)
                mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, targetMainFOV,
                    Time.deltaTime * _controller.Data.AimLerpSpeed);

            if (weaponCamera != null && Mathf.Abs(weaponCamera.fieldOfView - targetWeaponFOV) > FOVEpsilon)
                weaponCamera.fieldOfView = Mathf.Lerp(weaponCamera.fieldOfView, targetWeaponFOV,
                    Time.deltaTime * _controller.Data.AimLerpSpeed);
        }

        private void UpdateBulletPoint()
        {
            if (bulletPoint == null) return;

            Transform target = _isAiming ? aimedBulletPoint : hipBulletPoint;
            if (target != null) bulletPoint.position = target.position;
        }

        private void HandleSprintChanged(PlayerSprintStateChangedEvent evt) => _isSprinting = evt.IsSprinting;
    }
}
