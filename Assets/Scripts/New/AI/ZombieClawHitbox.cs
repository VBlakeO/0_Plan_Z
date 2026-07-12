using System.Collections.Generic;
using UnityEngine;
using PlanZ.AI.Zombies.Components;
using PlanZ.Combat.Damage;

namespace PlanZ.AI.Zombies.Targets
{
    // Trigger collider on the zombie's claw. Enabled only during the strike frames of the
    // attack animation via animation events (EnableHitbox / DisableHitbox). Damages any
    // IDamageable it overlaps, except its own zombie (so the claw doesn't hit the zombie's
    // own body colliders). Tracks who was already hit during the current swing so a single
    // swing can't damage the player multiple times just by brushing multiple hitboxes on
    // the way through.
    [RequireComponent(typeof(Collider))]
    public class ZombieClawHitbox : MonoBehaviour
    {
        [SerializeField] private ZombieAI ownerZombie;
        [SerializeField] private float damage = 10f;

        private Collider _collider;
        private readonly HashSet<Health> _hitThisSwing = new();

        private void Awake()
        {
            _collider = GetComponent<Collider>();
            _collider.isTrigger = true;
            _collider.enabled = true;

            if (ownerZombie == null) ownerZombie = GetComponentInParent<ZombieAI>();
        }

        // Called by animation events on the attack clip. Toggling the Collider (not the whole
        // GameObject) is critical: a disabled GameObject stops receiving animation events, so
        // SetActive(false) between swings would prevent the next EnableHitbox call from ever
        // firing. Only the collider needs to go on/off - the component stays alive to hear the
        // next event.
        public void EnableHitbox()
        {
            _hitThisSwing.Clear();
            if (_collider != null) _collider.enabled = true;
        }

        public void DisableHitbox()
        {
            if (_collider != null) _collider.enabled = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponentInParent<ZombieAI>() == ownerZombie) return;

            Health targetHealth = other.GetComponentInParent<Health>();
            if (targetHealth == null) return;
            if (_hitThisSwing.Contains(targetHealth)) return;

            IDamageable damageable = other.GetComponentInParent<IDamageable>();
            if (damageable == null) return;

            Vector3 hitPoint = other.ClosestPoint(transform.position);
            Vector3 direction = (hitPoint - transform.position).normalized;
            DamageInfo info = new DamageInfo(damage, transform, hitPoint, direction);
            damageable.ApplyDamage(info);

            _hitThisSwing.Add(targetHealth);
        }
    }
}