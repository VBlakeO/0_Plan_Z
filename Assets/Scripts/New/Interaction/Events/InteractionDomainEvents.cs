using PlanZ.Events;
using PlanZ.Interaction.Components;

namespace PlanZ.Interaction.Events
{
    public readonly struct InteractionTargetChangedEvent : IEvent
    {
        public Interactable Previous { get; }
        public Interactable Current { get; }

        public InteractionTargetChangedEvent(Interactable previous, Interactable current)
        {
            Previous = previous;
            Current = current;
        }
    }

    public readonly struct InteractionStartedEvent : IEvent
    {
        public Interactable Target { get; }
        public InteractionStartedEvent(Interactable target) => Target = target;
    }

    public readonly struct InteractionProgressedEvent : IEvent
    {
        public Interactable Target { get; }
        public float Progress { get; }

        public InteractionProgressedEvent(Interactable target, float progress)
        {
            Target = target;
            Progress = progress;
        }
    }

    public readonly struct InteractionCancelledEvent : IEvent
    {
        public Interactable Target { get; }
        public InteractionCancelledEvent(Interactable target) => Target = target;
    }

    public readonly struct InteractionCompletedEvent : IEvent
    {
        public Interactable Target { get; }
        public InteractionCompletedEvent(Interactable target) => Target = target;
    }

    public readonly struct InteractableLabelChangedEvent : IEvent
    {
        public Interactable Target { get; }
        public InteractableLabelChangedEvent(Interactable target) => Target = target;
    }
}