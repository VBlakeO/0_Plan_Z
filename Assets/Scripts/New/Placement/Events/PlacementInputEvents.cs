using PlanZ.Events;

namespace PlanZ.Placement.Events
{
    public readonly struct PlacementSlotSelectedEvent : IEvent
    {
        public int SlotIndex { get; }
        public PlacementSlotSelectedEvent(int slotIndex) => SlotIndex = slotIndex;
    }

    public readonly struct PlacementConfirmPressedEvent : IEvent { }

    public readonly struct PlacementRotatePressedEvent : IEvent { }

    public readonly struct PlacementCancelPressedEvent : IEvent { }
}
