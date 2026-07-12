using UnityEngine;
using PlanZ.Combat.Damage;
using PlanZ.Combat.Pooling;
using PlanZ.Events;
using PlanZ.Player.Input;
using PlanZ.Weapons.Data;
using PlanZ.Weapons.Events;

namespace PlanZ.Weapons.Components
{
    [RequireComponent(typeof(WeaponController))]
    public class WeaponShooter : MonoBehaviour
    {
        [SerializeField] private Camera fireCamera;
        [SerializeField] private Camera traceCamera;

        [SerializeField] private Transform bulletOrigin;
        [SerializeField] private AmmoInventory ammoInventory;

        private const float DecalNormalOffset = 0.01f;

        private WeaponController _controller;
        private WeaponReloader _reloader;
        private float _nextShotTime;
        private bool _firePressedThisFrame;
        private bool _automaticReadyToFire;

        private void Awake()
        {
            _controller = GetComponent<WeaponController>();
            _reloader = GetComponent<WeaponReloader>();
        }

        // OnEnable runs when the weapon is equipped (the GameObject is reactivated by the
        // WeaponSwitcher). Resetting these flags here prevents two annoying behaviours:
        // 1) Semi-auto: clicks made during the equip animation would queue up and fire instantly
        //    when the weapon becomes ready.
        // 2) Automatic: holding Mouse0 during a weapon swap would cause the new weapon to start
        //    spraying the moment the equip finishes. Requiring a release-and-press matches the
        //    convention used by most modern FPS games.
        private void OnEnable()
        {
            EventBus.Subscribe<WeaponFirePressedEvent>(HandleFirePressed);
            EventBus.Subscribe<WeaponEquippedEvent>(HandleEquipped);

            _firePressedThisFrame = false;
            _automaticReadyToFire = false;
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<WeaponFirePressedEvent>(HandleFirePressed);
            EventBus.Unsubscribe<WeaponEquippedEvent>(HandleEquipped);
        }

        // Publishing the ammo state must happen AFTER WeaponEquippedEvent so the HUD has already
        // bound to this weapon. Doing it inside OnEnable would race with the equip flow because
        // OnEnable fires when SetActive(true) is called, but before the controller publishes the
        // equip event in the next line.
        private void HandleEquipped(WeaponEquippedEvent evt)
        {
            if (evt.Data != _controller.Data) return;
            PublishCurrentAmmoState();
        }

        private void Update()
        {
            if (!_controller.IsFullyEquipped)
            {
                _firePressedThisFrame = false;
                return;
            }

            UpdateAutomaticReadiness();

            bool wantsToFire = _controller.Data.ShotType == ShotType.Automatic
                ? PlayerInput.Instance.FireHeld && _automaticReadyToFire
                : _firePressedThisFrame;

            _firePressedThisFrame = false;

            if (wantsToFire) TryFire();
        }

        // Automatic fire is only allowed once the player has released the trigger at least once
        // since the weapon was equipped. This avoids carrying a held button across weapon swaps.
        private void UpdateAutomaticReadiness()
        {
            if (_automaticReadyToFire) return;
            if (!PlayerInput.Instance.FireHeld) _automaticReadyToFire = true;
        }

        private void HandleFirePressed()
        {
            if (!_controller.IsFullyEquipped) return;
            _firePressedThisFrame = true;
        }

        private void TryFire()
        {
            if (_controller.IsLocked || _controller.IsReloading) return;
            if (Time.time < _nextShotTime) return;

            if (_controller.CurrentMagazine <= 0)
            {
                HandleEmptyTrigger();
                return;
            }

            FireOnce();
        }

        private void HandleEmptyTrigger()
        {
            EventBus.Publish(new WeaponEmptyTriggerEvent(_controller.Data));

            if (_controller.Data.AutoReloadOnEmpty && _reloader != null)
                _reloader.TryStartReload();
        }

        private void FireOnce()
        {
            if (!_controller.TryConsumeRound()) return;

            _nextShotTime = Time.time + _controller.Data.FireRate;

            int reserve = ammoInventory != null ? ammoInventory.GetAmount(_controller.Data.AmmoType) : 0;
            _controller.NotifyAmmoChanged(reserve);

            bool didHit = Physics.Raycast(fireCamera.transform.position, fireCamera.transform.forward,
                out RaycastHit hit, _controller.Data.Range, _controller.Data.HitLayers,
                QueryTriggerInteraction.Ignore);

            Vector3 endPoint;
            Vector3 normal = Vector3.up;

            if (didHit)
            {
                ApplyDamage(hit);
                SpawnDecal(hit);
                normal = hit.normal;
            }

            endPoint = traceCamera.transform.position + traceCamera.transform.forward * _controller.Data.Range;

            SpawnTracer(endPoint);
            EventBus.Publish(new WeaponFiredEvent(_controller.Data, didHit, endPoint, normal));
        }

        private void ApplyDamage(RaycastHit hit)
        {
            IDamageable damageable = hit.transform.GetComponentInParent<IDamageable>();
            if (damageable == null) return;

            Vector3 direction = fireCamera.transform.forward;
            DamageInfo info = new DamageInfo(_controller.Data.Damage, transform, hit.point, direction);
            damageable.ApplyDamage(info);
        }

        private void SpawnDecal(RaycastHit hit)
        {
            if (PoolRegistry.Instance == null) return;
            string poolId = _controller.Data.DecalPoolId;
            if (string.IsNullOrEmpty(poolId)) return;

            Vector3 position = hit.point + hit.normal * DecalNormalOffset;
            Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, hit.normal);
            PoolRegistry.Instance.Spawn(poolId, position, rotation);
        }

        private void SpawnTracer(Vector3 endPoint)
        {
            if (PoolRegistry.Instance == null) return;
            string poolId = _controller.Data.TracerPoolId;
            if (string.IsNullOrEmpty(poolId)) return;
            if (bulletOrigin == null) return;

            GameObject instance = PoolRegistry.Instance.Spawn(poolId, bulletOrigin.position, Quaternion.identity);
            if (instance == null) return;

            BulletTracer tracer = instance.GetComponent<BulletTracer>();
            if (tracer != null) tracer.Launch(bulletOrigin.position, endPoint);
        }

        private void PublishCurrentAmmoState()
        {
            int reserve = ammoInventory != null ? ammoInventory.GetAmount(_controller.Data.AmmoType) : 0;
            _controller.NotifyAmmoChanged(reserve);
        }
    }
}