using System.Collections;
using UnityEngine;
using PlanZ.Events;
using PlanZ.Weapons.Events;

namespace PlanZ.Weapons.Components
{
    [RequireComponent(typeof(WeaponController))]
    public class WeaponReloader : MonoBehaviour
    {
        [SerializeField] private AmmoInventory ammoInventory;

        private WeaponController _controller;
        private Coroutine _activeReload;

        private void Awake() => _controller = GetComponent<WeaponController>();

        private void OnEnable() => EventBus.Subscribe<WeaponReloadPressedEvent>(HandleReloadPressed);

        private void OnDisable()
        {
            EventBus.Unsubscribe<WeaponReloadPressedEvent>(HandleReloadPressed);
            CancelActiveReload();
        }

        private void HandleReloadPressed() => TryStartReload();

        public void TryStartReload()
        {
            if (!_controller.IsFullyEquipped) return;
            if (_controller.IsReloading) return;
            if (_controller.GetMissingFromMagazine() <= 0) return;
            if (ammoInventory != null && ammoInventory.GetAmount(_controller.Data.AmmoType) <= 0) return;

            _activeReload = StartCoroutine(ReloadRoutine());
        }

        private IEnumerator ReloadRoutine()
        {
            _controller.IsReloading = true;
            EventBus.Publish(new WeaponReloadStartedEvent(_controller.Data));

            yield return new WaitForSeconds(_controller.Data.ReloadDuration);

            int missing = _controller.GetMissingFromMagazine();
            int taken = ammoInventory != null
                ? ammoInventory.TakeFromReserve(_controller.Data.AmmoType, missing)
                : missing;

            _controller.RefillMagazine(taken);

            int reserve = ammoInventory != null ? ammoInventory.GetAmount(_controller.Data.AmmoType) : 0;
            _controller.NotifyAmmoChanged(reserve);

            _controller.IsReloading = false;
            _activeReload = null;
            EventBus.Publish(new WeaponReloadFinishedEvent(_controller.Data));
        }

        private void CancelActiveReload()
        {
            if (_activeReload == null) return;

            StopCoroutine(_activeReload);
            _activeReload = null;
            _controller.IsReloading = false;
        }
    }
}
