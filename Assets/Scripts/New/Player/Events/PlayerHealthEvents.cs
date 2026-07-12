using PlanZ.Combat.Damage;
using PlanZ.Events;

namespace PlanZ.Player.Events
{
    public readonly struct PlayerHealthChangedEvent : IEvent
    {
        public float Current { get; }
        public float Max { get; }
        public float Normalized { get; }

        public PlayerHealthChangedEvent(float current, float max, float normalized)
        {
            Current = current;
            Max = max;
            Normalized = normalized;
        }
    }

    public readonly struct PlayerDamagedEvent : IEvent
    {
        public DamageInfo Info { get; }
        public PlayerDamagedEvent(DamageInfo info) => Info = info;
    }

    public readonly struct PlayerDiedEvent : IEvent
    {
        public DamageInfo KillingBlow { get; }
        public PlayerDiedEvent(DamageInfo killingBlow) => KillingBlow = killingBlow;
    }
}
