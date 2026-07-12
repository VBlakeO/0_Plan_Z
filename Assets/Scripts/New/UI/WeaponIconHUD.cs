using UnityEngine;
using UnityEngine.UI;
using PlanZ.Events;
using PlanZ.Weapons.Events;

namespace PlanZ.UI
{
    public class WeaponIconHUD : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private bool hideWhenUnequipped = true;

        private void OnEnable() => EventBus.Subscribe<WeaponEquippedEvent>(HandleEquipped);

        private void OnDisable() => EventBus.Unsubscribe<WeaponEquippedEvent>(HandleEquipped);

        private void HandleEquipped(WeaponEquippedEvent evt)
        {
            if (iconImage == null) return;

            iconImage.sprite = evt.Data.Icon;

            if (hideWhenUnequipped)
                iconImage.enabled = evt.Data.Icon != null;
        }
    }
}
