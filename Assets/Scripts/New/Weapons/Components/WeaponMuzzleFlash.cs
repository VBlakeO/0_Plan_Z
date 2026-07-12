using UnityEngine;
using PlanZ.Events;
using PlanZ.Weapons.Events;

namespace PlanZ.Weapons.Components
{
    // Plays muzzle flash, smoke, ejection particles, and any other fire-time VFX whenever the
    // owning weapon fires. Accepts an array so multiple effects on the same weapon can be driven
    // by a single component instead of multiplying listener components.
    [RequireComponent(typeof(WeaponController))]
    public class WeaponMuzzleFlash : MonoBehaviour
    {
        [SerializeField] private ParticleSystem[] effects;
        [SerializeField] private bool activateGameObjectOnFire = true;

        private WeaponController _controller;

        private void Awake() => _controller = GetComponent<WeaponController>();

        private void OnEnable() => EventBus.Subscribe<WeaponFiredEvent>(HandleWeaponFired);

        private void OnDisable() => EventBus.Unsubscribe<WeaponFiredEvent>(HandleWeaponFired);

        private void HandleWeaponFired(WeaponFiredEvent evt)
        {
            if (evt.Data != _controller.Data) return;
            if (effects == null) return;

            for (int i = 0; i < effects.Length; i++)
                PlayEffect(effects[i]);
        }

        private void PlayEffect(ParticleSystem effect)
        {
            if (effect == null) return;

            // Activating the GameObject ensures the effect plays even if a previous shot stopped
            // it and an external system disabled the object. Most muzzle flashes are short-lived
            // particle bursts whose own Duration takes them back to idle automatically.
            if (activateGameObjectOnFire && !effect.gameObject.activeSelf)
                effect.gameObject.SetActive(true);

            effect.Play(true);
        }
    }
}
