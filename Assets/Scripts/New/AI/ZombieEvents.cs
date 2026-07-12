using PlanZ.AI.Zombies.Components;
using PlanZ.AI.Zombies.Targets;
using PlanZ.Events;

namespace PlanZ.AI.Zombies.Events
{
    public readonly struct ZombieStateChangedEvent : IEvent
    {
        public ZombieAI Zombie { get; }
        public ZombieState Previous { get; }
        public ZombieState Current { get; }

        public ZombieStateChangedEvent(ZombieAI zombie, ZombieState previous, ZombieState current)
        {
            Zombie = zombie;
            Previous = previous;
            Current = current;
        }
    }

    public readonly struct ZombieAttackStartedEvent : IEvent
    {
        public ZombieAI Zombie { get; }
        public ZombieTargetKind TargetKind { get; }
        public int AttackVariant { get; }

        public ZombieAttackStartedEvent(ZombieAI zombie, ZombieTargetKind targetKind, int attackVariant)
        {
            Zombie = zombie;
            TargetKind = targetKind;
            AttackVariant = attackVariant;
        }
    }

    public readonly struct ZombieMovementTickEvent : IEvent
    {
        public ZombieAI Zombie { get; }
        public float PathDistance { get; }

        public ZombieMovementTickEvent(ZombieAI zombie, float pathDistance)
        {
            Zombie = zombie;
            PathDistance = pathDistance;
        }
    }
}
