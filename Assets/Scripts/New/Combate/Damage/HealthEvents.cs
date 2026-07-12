using PlanZ.Events;

namespace PlanZ.Combat.Damage
{
    public readonly struct EntityDamagedEvent : IEvent
    {
        public Health Target { get; }
        public DamageInfo Info { get; }
        public float HealthAfter { get; }

        public EntityDamagedEvent(Health target, DamageInfo info, float healthAfter)
        {
            Target = target;
            Info = info;
            HealthAfter = healthAfter;
        }
    }

    public readonly struct EntityHealedEvent : IEvent
    {
        public Health Target { get; }
        public float Amount { get; }
        public float HealthAfter { get; }

        public EntityHealedEvent(Health target, float amount, float healthAfter)
        {
            Target = target;
            Amount = amount;
            HealthAfter = healthAfter;
        }
    }

    // Published the moment health hits zero, BEFORE the death delay starts. Listeners that need
    // to react to the lethal hit (drop loot, trigger ragdoll, freeze AI) subscribe to this.
    public readonly struct EntityDiedEvent : IEvent
    {
        public Health Target { get; }
        public DamageInfo KillingBlow { get; }

        public EntityDiedEvent(Health target, DamageInfo killingBlow)
        {
            Target = target;
            KillingBlow = killingBlow;
        }
    }

    // Published when the death coroutine finishes, just before the GameObject is destroyed or
    // disabled. Useful for cleanup or final cleanup-after-anim work.
    public readonly struct EntityDeathFinishedEvent : IEvent
    {
        public Health Target { get; }
        public EntityDeathFinishedEvent(Health target) => Target = target;
    }
}
