using UnityEngine;
using PlanZ.Events;
using PlanZ.Weapons.Data;
using PlanZ.Weapons.Events;
using TMPro;

namespace PlanZ.UI
{
    public class WeaponAmmoHUD : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI magazineText;

        private WeaponData _equippedWeapon;
        private int _lastMagazine;
        private int _lastReserve;

        private void OnEnable()
        {
            EventBus.Subscribe<WeaponEquippedEvent>(HandleEquipped);
            EventBus.Subscribe<WeaponUnequippedEvent>(HandleUnequipped);
            EventBus.Subscribe<WeaponAmmoChangedEvent>(HandleAmmoChanged);
            EventBus.Subscribe<AmmoReserveChangedEvent>(HandleReserveChanged);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<WeaponEquippedEvent>(HandleEquipped);
            EventBus.Unsubscribe<WeaponUnequippedEvent>(HandleUnequipped);
            EventBus.Unsubscribe<WeaponAmmoChangedEvent>(HandleAmmoChanged);
            EventBus.Unsubscribe<AmmoReserveChangedEvent>(HandleReserveChanged);
        }

        private void HandleEquipped(WeaponEquippedEvent evt)
        {
            _equippedWeapon = evt.Data;
            _lastMagazine = evt.CurrentMagazine;
            Render();
        }

        private void HandleUnequipped(WeaponUnequippedEvent evt)
        {
            if (evt.Data != _equippedWeapon) return;
            _equippedWeapon = null;
        }

        private void HandleAmmoChanged(WeaponAmmoChangedEvent evt)
        {
            if (evt.Data != _equippedWeapon) return;

            _lastMagazine = evt.CurrentMagazine;
            _lastReserve = evt.Reserve;
            Render();
        }

        private void HandleReserveChanged(AmmoReserveChangedEvent evt)
        {
            if (_equippedWeapon == null || evt.AmmoType != _equippedWeapon.AmmoType) return;

            _lastReserve = evt.Amount;
            Render();
        }

        private void Render()
        {
            if (magazineText != null)
            {
                magazineText.text = _lastMagazine.ToString() + "/" +_lastReserve.ToString();
            }
        }
    }
}
