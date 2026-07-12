using UnityEngine;
using PlanZ.Events;
using PlanZ.Healing.Events;

namespace PlanZ.Healing.Components
{
    // Tracks how many medkits the player carries, with a configurable cap. Following the same
    // pattern as AmmoInventory and PlaceableInventory: initial amount + capacity, events on
    // change, and an Add that returns how much was actually added so pickups can decide whether
    // to consume themselves.
    public class MedkitInventory : MonoBehaviour
    {
        [SerializeField] private int initialAmount = 1;
        [SerializeField] private int maxCapacity = 3;

        private int _current;

        public int Current => _current;
        public int Capacity => maxCapacity;
        public bool HasAny => _current > 0;
        public bool IsFull => _current >= maxCapacity;

        private void Awake()
        {
            _current = Mathf.Clamp(initialAmount, 0, maxCapacity);
        }

        private void Start()
        {
            // Initial broadcast so HUDs that bind after Awake still see the starting count.
            EventBus.Publish(new MedkitInventoryChangedEvent(_current, maxCapacity));
        }

        public bool TryConsume()
        {
            if (_current <= 0) return false;

            _current--;
            EventBus.Publish(new MedkitInventoryChangedEvent(_current, maxCapacity));
            return true;
        }

        public int Add(int amount)
        {
            if (amount <= 0) return 0;

            int before = _current;
            _current = Mathf.Min(_current + amount, maxCapacity);
            int added = _current - before;
            if (added <= 0) return 0;

            EventBus.Publish(new MedkitInventoryChangedEvent(_current, maxCapacity));
            return added;
        }
    }
}
