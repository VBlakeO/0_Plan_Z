using UnityEngine;
using UnityEngine.UI;
using PlanZ.Events;
using PlanZ.Weapons.Data;
using PlanZ.Weapons.Events;

namespace PlanZ.UI
{
    // Renders the crosshair from the equipped weapon's CrosshairData and swaps between the
    // default and aiming variants. The aim variant can be a smaller dot, an empty sprite to
    // hide entirely while ADS, or any other style configured in the SO.
    public class WeaponCrosshairHUD : MonoBehaviour
    {
        [SerializeField] private Image crosshairImage;
        [SerializeField] private bool hideWhenAiming;

        private WeaponData _equippedWeapon;
        private bool _isAiming;

        private void OnEnable()
        {
            EventBus.Subscribe<WeaponEquippedEvent>(HandleEquipped);
            EventBus.Subscribe<WeaponUnequippedEvent>(HandleUnequipped);
            EventBus.Subscribe<WeaponAimStateChangedEvent>(HandleAimChanged);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<WeaponEquippedEvent>(HandleEquipped);
            EventBus.Unsubscribe<WeaponUnequippedEvent>(HandleUnequipped);
            EventBus.Unsubscribe<WeaponAimStateChangedEvent>(HandleAimChanged);
        }

        private void HandleEquipped(WeaponEquippedEvent evt)
        {
            _equippedWeapon = evt.Data;
            Render();
        }

        private void HandleUnequipped(WeaponUnequippedEvent evt)
        {
            if (evt.Data != _equippedWeapon) return;
            _equippedWeapon = null;
            if (crosshairImage != null) crosshairImage.enabled = false;
        }

        private void HandleAimChanged(WeaponAimStateChangedEvent evt)
        {
            if (evt.Data != _equippedWeapon) return;
            _isAiming = evt.IsAiming;
            Render();
        }

        private void Render()
        {
            if (crosshairImage == null || _equippedWeapon == null) return;

            if (hideWhenAiming && _isAiming)
            {
                crosshairImage.enabled = false;
                return;
            }

            CrosshairData data = _isAiming ? _equippedWeapon.AimingCrosshair : _equippedWeapon.DefaultCrosshair;
            if (data == null)
            {
                crosshairImage.enabled = false;
                return;
            }

            crosshairImage.enabled = true;
            crosshairImage.sprite = data.Sprite;
            crosshairImage.color = data.Color;
            crosshairImage.rectTransform.sizeDelta = new Vector2(data.Size, data.Size);
        }
    }
}
