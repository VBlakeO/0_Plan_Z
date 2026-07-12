using System;
using System.Collections.Generic;
using UnityEngine;
using PlanZ.Events;
using PlanZ.Placement.Data;
using PlanZ.Placement.Events;

namespace PlanZ.Placement.Components
{
    public class PlaceableInventory : MonoBehaviour
    {
        [Serializable]
        private struct InventorySlot
        {
            public PlaceableData data;
            public int amount;
        }

        [SerializeField] private List<InventorySlot> slots = new();

        private readonly Dictionary<PlaceableData, int> _amounts = new();

        private void Awake()
        {
            foreach (var slot in slots)
            {
                if (slot.data == null) continue;
                _amounts[slot.data] = slot.amount;
            }
        }

        // Look up the placeable assigned to a slot index. Returns null if the slot is empty or
        // out of range, allowing the controller to safely call this from input handlers.
        public PlaceableData GetSlotData(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= slots.Count) return null;
            return slots[slotIndex].data;
        }

        public int GetAmount(PlaceableData data)
        {
            if (data == null) return 0;
            return _amounts.TryGetValue(data, out int amount) ? amount : 0;
        }

        public bool HasAny(PlaceableData data) => GetAmount(data) > 0;

        public bool TryConsume(PlaceableData data)
        {
            int current = GetAmount(data);
            if (current <= 0) return false;

            _amounts[data] = current - 1;
            EventBus.Publish(new PlaceableInventoryChangedEvent(data, _amounts[data]));
            return true;
        }

        public void Add(PlaceableData data, int amount)
        {
            if (data == null || amount <= 0) return;

            int current = GetAmount(data);
            _amounts[data] = current + amount;
            EventBus.Publish(new PlaceableInventoryChangedEvent(data, _amounts[data]));
        }
    }
}
