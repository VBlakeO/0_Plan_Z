using UnityEngine;
using PlanZ.Events;
using PlanZ.Weapons.Events;

namespace PlanZ.Weapons.Components
{
    [RequireComponent(typeof(WeaponController))]
    public class WeaponAnimator : MonoBehaviour
    {
        [SerializeField] private Animator anim;

        private const string AnimDraw = "DrawWeapon";
        private const string AnimUnequip = "UnequipWeapon";
        private const string AnimFire = "Fire";
        private const string AnimAimFire = "AimFire";
        private const string AnimReload = "Reload";
        private const string AnimAiming = "Aiming";

        private static readonly int DrawHash = Animator.StringToHash(AnimDraw);
        private static readonly int UnequipHash = Animator.StringToHash(AnimUnequip);
        private static readonly int FireHash = Animator.StringToHash(AnimFire);
        private static readonly int AimFireHash = Animator.StringToHash(AnimAimFire);
        private static readonly int ReloadHash = Animator.StringToHash(AnimReload);
        private static readonly int AimingHash = Animator.StringToHash(AnimAiming);

        private WeaponController _controller;
        private WeaponAimer _aimer;

        private void Awake()
        {
            _controller = GetComponent<WeaponController>();
            _aimer = GetComponent<WeaponAimer>();
        }

        private void OnEnable()
        {
            EventBus.Subscribe<WeaponEquippedEvent>(HandleEquipped);
            EventBus.Subscribe<WeaponUnequippedEvent>(HandleUnequipped);
            EventBus.Subscribe<WeaponFiredEvent>(HandleFired);
            EventBus.Subscribe<WeaponReloadStartedEvent>(HandleReloadStarted);
            EventBus.Subscribe<WeaponAimStateChangedEvent>(HandleAimStateChanged);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<WeaponEquippedEvent>(HandleEquipped);
            EventBus.Unsubscribe<WeaponUnequippedEvent>(HandleUnequipped);
            EventBus.Unsubscribe<WeaponFiredEvent>(HandleFired);
            EventBus.Unsubscribe<WeaponReloadStartedEvent>(HandleReloadStarted);
            EventBus.Unsubscribe<WeaponAimStateChangedEvent>(HandleAimStateChanged);
        }

        private void HandleEquipped(WeaponEquippedEvent evt)
        {
            if (evt.Data != _controller.Data || anim == null) return;
            anim.Play(DrawHash, 0, 0);
        }

        private void HandleUnequipped(WeaponUnequippedEvent evt)
        {
            if (evt.Data != _controller.Data || anim == null) return;
            anim.Play(UnequipHash, 0, 0);
        }

        private void HandleFired(WeaponFiredEvent evt)
        {
            if (evt.Data != _controller.Data || anim == null) return;

            int hash = _aimer != null && _aimer.IsAiming ? AimFireHash : FireHash;
            anim.Play(hash, 0, 0);
        }

        private void HandleReloadStarted(WeaponReloadStartedEvent evt)
        {
            if (evt.Data != _controller.Data || anim == null) return;
            anim.Play(ReloadHash, 0, 0);
        }

        private void HandleAimStateChanged(WeaponAimStateChangedEvent evt)
        {
            if (evt.Data != _controller.Data || anim == null) return;
            anim.SetBool(AimingHash, evt.IsAiming);
        }
    }
}
