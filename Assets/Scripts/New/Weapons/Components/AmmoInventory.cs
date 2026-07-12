using System;
using System.Collections.Generic;
using UnityEngine;
using PlanZ.Events;
using PlanZ.Weapons.Data;
using PlanZ.Weapons.Events;

namespace PlanZ.Weapons.Components
{
    public class AmmoInventory : MonoBehaviour
    {
        [Serializable]
        private struct AmmoTypeEntry
        {
            public AmmoType ammoType;
            public int initialAmount;
            public int maxCapacity;
        }

        [SerializeField] private List<AmmoTypeEntry> ammoTypes = new();

        private readonly Dictionary<AmmoType, int> _reserves = new();
        private readonly Dictionary<AmmoType, int> _caps = new();

        private void Awake()
        {
            foreach (var entry in ammoTypes)
            {
                _reserves[entry.ammoType] = entry.initialAmount;
                _caps[entry.ammoType] = entry.maxCapacity;
            }
        }

        public int GetAmount(AmmoType type)
        {
            return _reserves.TryGetValue(type, out int amount) ? amount : 0;
        }

        public int GetCapacity(AmmoType type)
        {
            return _caps.TryGetValue(type, out int cap) ? cap : 0;
        }

        public bool IsFull(AmmoType type)
        {
            return GetAmount(type) >= GetCapacity(type);
        }

        public int GetMissingToFull(AmmoType type)
        {
            return Mathf.Max(0, GetCapacity(type) - GetAmount(type));
        }

        public int TakeFromReserve(AmmoType type, int requested)
        {
            int available = GetAmount(type);
            int taken = Mathf.Min(requested, available);

            _reserves[type] = available - taken;
            EventBus.Publish(new AmmoReserveChangedEvent(type, _reserves[type]));
            return taken;
        }

        // Returns the actual amount added (clamped by the type's capacity). Callers like ammo
        // boxes use this return value to decide whether the pickup should be consumed.
        public int Add(AmmoType type, int amount)
        {
            int current = GetAmount(type);
            int cap = GetCapacity(type);
            int newAmount = Mathf.Min(current + amount, cap);
            int added = newAmount - current;

            if (added <= 0) return 0;

            _reserves[type] = newAmount;
            EventBus.Publish(new AmmoReserveChangedEvent(type, newAmount));
            return added;
        }
    }
}