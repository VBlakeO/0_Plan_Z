using PlanZ.Events;

namespace PlanZ.Weapons.Events
{
    public readonly struct WeaponFirePressedEvent : IEvent { }

    public readonly struct WeaponFireReleasedEvent : IEvent { }

    public readonly struct WeaponReloadPressedEvent : IEvent { }

    public readonly struct WeaponSlotSelectedEvent : IEvent
    {
        public int SlotIndex { get; }
        public WeaponSlotSelectedEvent(int slotIndex) => SlotIndex = slotIndex;
    }

    public readonly struct WeaponCycleRequestedEvent : IEvent
    {
        public int Direction { get; }
        public WeaponCycleRequestedEvent(int direction) => Direction = direction;
    }
}
