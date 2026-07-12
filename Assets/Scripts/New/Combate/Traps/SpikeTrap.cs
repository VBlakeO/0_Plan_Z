using System.Collections.Generic;
using UnityEngine;
using PlanZ.Combat.Damage;
using PlanZ.Combat.Status;

namespace PlanZ.Combat.Traps
{
    // Damages any character standing on it at fixed intervals and slows them while they remain.
    // Works for any entity exposing IDamageable (for ticks) and ISlowable (for the slow). Both
    // are resolved per-occupant on enter so the trap stays decoupled from player vs enemy
    // specifics. Uses a trigger collider; characters need a Rigidbody for trigger callbacks.
    [RequireComponent(typeof(Collider))]
    public class SpikeTrap : MonoBehaviour
    {
        [Header("Damage")]
        [SerializeField] private float damagePerTick = 10f;
        [SerializeField] private float tickInterval = 0.5f;

        [Header("Slow")]
        [SerializeField, Range(0f, 1f)] private float slowMultiplier = 0.5f;

        [Header("Targeting")]
        [SerializeField] private LayerMask affectedLayers = ~0;

        // Tracks each occupant alongside its next damage time, so multiple characters on the
        // same trap tick on independent schedules (whoever stepped on first ticks first).
        private readonly Dictionary<Collider, Occupant> _occupants = new();

        // Reused each frame to iterate occupant keys without allocating, and to avoid mutating
        // the dictionary while enumerating it.
        private readonly List<Collider> _tickBuffer = new();

        private struct Occupant
        {
            public IDamageable Damageable;
            public ISlowable Slowable;
            public float NextTickTime;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!IsAffected(other)) return;
            if (_occupants.ContainsKey(other)) return;

            IDamageable damageable = other.GetComponentInParent<IDamageable>();
            ISlowable slowable = other.GetComponentInParent<ISlowable>();

            // Ignore anything that can neither be damaged nor slowed - it's not a character.
            if (damageable == null && slowable == null) return;

            slowable?.ApplySlow(this, slowMultiplier);

            _occupants[other] = new Occupant
            {
                Damageable = damageable,
                Slowable = slowable,
                NextTickTime = Time.time
            };
        }

        private void OnTriggerExit(Collider other)
        {
            if (!_occupants.TryGetValue(other, out Occupant occupant)) return;

            occupant.Slowable?.RemoveSlow(this);
            _occupants.Remove(other);
        }

        // Iterating a copy of the values isn't needed because we only mutate NextTickTime in
        // place (struct reassignment via the key), never add/remove during the loop. Damage is
        // applied through the cached IDamageable so a dead occupant simply stops taking ticks
        // once Health flips to dying (Health ignores damage while dead).
        private void Update()
        {
            if (_occupants.Count == 0) return;

            float now = Time.time;
            _tickBuffer.Clear();
            _tickBuffer.AddRange(_occupants.Keys);

            foreach (Collider key in _tickBuffer)
            {
                Occupant occupant = _occupants[key];
                if (now < occupant.NextTickTime) continue;

                ApplyTick(occupant, key);

                occupant.NextTickTime = now + tickInterval;
                _occupants[key] = occupant;
            }
        }

        private void ApplyTick(Occupant occupant, Collider key)
        {
            if (occupant.Damageable == null) return;

            Vector3 hitPoint = key.bounds.center;
            Vector3 direction = Vector3.up;
            DamageInfo info = new DamageInfo(damagePerTick, transform, hitPoint, direction);
            occupant.Damageable.ApplyDamage(info);
        }

        private bool IsAffected(Collider other)
        {
            return (affectedLayers.value & (1 << other.gameObject.layer)) != 0;
        }

        // Safety net: if the trap is destroyed while characters stand on it, lift their slows
        // so they aren't permanently slowed by a source that no longer exists.
        private void OnDisable()
        {
            foreach (Occupant occupant in _occupants.Values)
                occupant.Slowable?.RemoveSlow(this);

            _occupants.Clear();
        }
    }
}
