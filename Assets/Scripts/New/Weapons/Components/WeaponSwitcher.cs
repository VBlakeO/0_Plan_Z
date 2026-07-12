using UnityEngine;
using PlanZ.Events;
using PlanZ.Placement.Events;
using PlanZ.Weapons.Events;

namespace PlanZ.Weapons.Components
{
    public class WeaponSwitcher : MonoBehaviour
    {
        [SerializeField] private WeaponController[] slots;

        private WeaponController _currentWeapon;
        private int _currentIndex = -1;
        private int _suspendedIndex = -1;

        public WeaponController Current => _currentWeapon;

        private void OnEnable()
        {
            EventBus.Subscribe<WeaponSlotSelectedEvent>(HandleSlotSelected);
            EventBus.Subscribe<WeaponCycleRequestedEvent>(HandleCycleRequested);
            EventBus.Subscribe<PlacementEnteredEvent>(HandlePlacementEntered);
            EventBus.Subscribe<PlacementExitedEvent>(HandlePlacementExited);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<WeaponSlotSelectedEvent>(HandleSlotSelected);
            EventBus.Unsubscribe<WeaponCycleRequestedEvent>(HandleCycleRequested);
            EventBus.Unsubscribe<PlacementEnteredEvent>(HandlePlacementEntered);
            EventBus.Unsubscribe<PlacementExitedEvent>(HandlePlacementExited);
        }

        private void Start() => EquipSlot(0);

        private void HandleSlotSelected(WeaponSlotSelectedEvent evt) => EquipSlot(evt.SlotIndex);

        private void HandleCycleRequested(WeaponCycleRequestedEvent evt) => CycleByDirection(evt.Direction);

        // Placement mode requires the weapon to be put away. We remember which slot was active
        // so it can be restored when placement ends, preserving the player's last selection.
        private void HandlePlacementEntered(PlacementEnteredEvent evt)
        {
            if (_currentWeapon == null) return;

            _suspendedIndex = _currentIndex;
            _currentWeapon.Unequip();
            _currentWeapon = null;
            _currentIndex = -1;
        }

        private void HandlePlacementExited(PlacementExitedEvent evt)
        {
            if (_suspendedIndex < 0) return;

            int slotToRestore = _suspendedIndex;
            _suspendedIndex = -1;
            EquipSlot(slotToRestore);
        }

        private void CycleByDirection(int direction)
        {
            if (slots == null || slots.Length == 0) return;

            int next = _currentIndex + direction;
            if (next < 0) next = slots.Length - 1;
            if (next >= slots.Length) next = 0;

            EquipSlot(next);
        }

        private void EquipSlot(int index)
        {
            if (slots == null) return;
            if (index < 0 || index >= slots.Length) return;
            if (index == _currentIndex) return;
            if (slots[index] == null) return;

            if (_currentWeapon != null && _currentWeapon.IsReloading) return;

            if (_currentWeapon != null)
                _currentWeapon.Unequip();

            _currentWeapon = slots[index];
            _currentIndex = index;
            _currentWeapon.Equip();
        }
    }
}