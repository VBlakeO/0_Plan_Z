using PlanZ.Events;

namespace PlanZ.Healing.Events
{
    public readonly struct HealStartedEvent : IEvent { }

    public readonly struct HealProgressedEvent : IEvent
    {
        public float Progress { get; }
        public HealProgressedEvent(float progress) => Progress = progress;
    }

    public readonly struct HealCancelledEvent : IEvent { }

    public readonly struct HealCompletedEvent : IEvent
    {
        public float AmountHealed { get; }
        public HealCompletedEvent(float amountHealed) => AmountHealed = amountHealed;
    }

    public readonly struct MedkitInventoryChangedEvent : IEvent
    {
        public int Current { get; }
        public int Capacity { get; }

        public MedkitInventoryChangedEvent(int current, int capacity)
        {
            Current = current;
            Capacity = capacity;
        }
    }
}
