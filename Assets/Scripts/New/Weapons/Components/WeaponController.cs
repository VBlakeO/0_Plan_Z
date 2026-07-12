using UnityEngine;
using PlanZ.Events;
using PlanZ.Weapons.Data;
using PlanZ.Weapons.Events;

namespace PlanZ.Weapons.Components
{
    // Orchestrator. Holds the WeaponData and exposes runtime state (ammo, equipped, reloading)
    // that subcomponents read. Does not perform shooting/reloading/aiming logic itself; those
    // live in dedicated components on the same GameObject.
    public class WeaponController : MonoBehaviour
    {
        [SerializeField] private WeaponData data;

        public WeaponData Data => data;

        public int CurrentMagazine { get; private set; }
        public bool IsEquipped { get; private set; }
        public bool IsReloading { get; set; }
        public bool IsLocked { get; set; }

        private float _equipFinishTime;

        public bool IsFullyEquipped => IsEquipped && Time.time >= _equipFinishTime;

        private void Awake() => CurrentMagazine = data.MagazineCapacity;

        public void NotifyAmmoChanged(int reserve)
        {
            EventBus.Publish(new WeaponAmmoChangedEvent(data, CurrentMagazine, reserve));
        }

        public void Equip()
        {
            gameObject.SetActive(true);
            IsEquipped = true;
            IsLocked = false;
            _equipFinishTime = Time.time + data.EquipDuration;
            EventBus.Publish(new WeaponEquippedEvent(data, CurrentMagazine));
        }

        public void Unequip()
        {
            IsEquipped = false;
            EventBus.Publish(new WeaponUnequippedEvent(data));
            gameObject.SetActive(false);
        }

        public bool TryConsumeRound()
        {
            if (CurrentMagazine <= 0) return false;

            CurrentMagazine--;
            return true;
        }

        public void RefillMagazine(int amount)
        {
            CurrentMagazine = Mathf.Min(CurrentMagazine + amount, data.MagazineCapacity);
        }

        public int GetMissingFromMagazine()
        {
            return data.MagazineCapacity - CurrentMagazine;
        }
    }
}