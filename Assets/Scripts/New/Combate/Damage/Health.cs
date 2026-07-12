using System.Collections;
using UnityEngine;
using PlanZ.Events;
using UnityEngine.Events;

namespace PlanZ.Combat.Damage
{
    // Core health component. Receives damage and healing, tracks current/max HP, publishes events.
    // Does NOT handle death visuals or game-specific cleanup - those live in separate handler
    // components that subscribe to EntityDiedEvent. Composition lets the same Health be used on
    // anything that can take damage (players, enemies, destructible props, generators).
    public class Health : MonoBehaviour, IDamageable
    {
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float deathDelay = 2f;
        [SerializeField] private bool destroyOnDeathFinished = true;
        [SerializeField] private Drop drop;

        private float _currentHealth;
        private bool _isDead;
        private bool _isDying;

        public float MaxHealth => maxHealth;
        public float CurrentHealth => _currentHealth;
        public float Normalized => maxHealth > 0f ? _currentHealth / maxHealth : 0f;
        public bool IsDead => _isDead;
        public bool IsDying => _isDying;

        private void Awake() => _currentHealth = maxHealth;

        public void ApplyDamage(DamageInfo info)
        {
            if (_isDead || _isDying) return;
            if (info.Amount <= 0f) return;

            _currentHealth = Mathf.Max(0f, _currentHealth - info.Amount);
            EventBus.Publish(new EntityDamagedEvent(this, info, _currentHealth));

            if (_currentHealth <= 0f) BeginDeath(info);
        }

        // Instant healing for medkits, pickups, scripted events. Regen-over-time would be a
        // separate HealthRegen component that calls this each tick.
        public void Heal(float amount)
        {
            if (_isDead || _isDying) return;
            if (amount <= 0f) return;

            float before = _currentHealth;
            _currentHealth = Mathf.Min(maxHealth, _currentHealth + amount);
            float actual = _currentHealth - before;
            if (actual <= 0f) return;

            EventBus.Publish(new EntityHealedEvent(this, actual, _currentHealth));
        }

        // Resets to full health and clears the dead/dying flags. Useful for respawn flows where
        // the same GameObject is reactivated rather than recreated.
        public void Revive()
        {
            _isDead = false;
            _isDying = false;
            _currentHealth = maxHealth;
        }

        private void BeginDeath(DamageInfo killingBlow)
        {
            _isDying = true;
            EventBus.Publish(new EntityDiedEvent(this, killingBlow));
            StartCoroutine(DeathRoutine());

            if (drop)
                drop.CallOnDie();
        }

        private IEnumerator DeathRoutine()
        {
            yield return new WaitForSeconds(deathDelay);

            _isDead = true;
            _isDying = false;
            EventBus.Publish(new EntityDeathFinishedEvent(this));
            
            if (drop)
                drop.CallOnDieWhitDelay();

            if (destroyOnDeathFinished) Destroy(gameObject);
        }
    }
}
