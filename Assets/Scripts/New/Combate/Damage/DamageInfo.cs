using UnityEngine;

namespace PlanZ.Combat.Damage
{
    // Carries all context about a damage event from the attacker to the recipient. Passing a
    // struct instead of a bare float lets damage handlers reason about who hit them, where, and
    // with what - enabling features like directional damage indicators, hit feedback at the
    // impact point, knockback, and source-based damage filtering (friendly fire toggles, etc).
    public readonly struct DamageInfo
    {
        public float Amount { get; }
        public Transform Source { get; }
        public Vector3 HitPoint { get; }
        public Vector3 HitDirection { get; }

        public DamageInfo(float amount, Transform source, Vector3 hitPoint, Vector3 hitDirection)
        {
            Amount = amount;
            Source = source;
            HitPoint = hitPoint;
            HitDirection = hitDirection;
        }

        public DamageInfo WithAmount(float newAmount)
        {
            return new DamageInfo(newAmount, Source, HitPoint, HitDirection);
        }
    }
}
