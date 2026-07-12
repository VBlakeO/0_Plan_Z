using UnityEngine;
using PlanZ.Combat.Damage;
using PlanZ.Events;
using PlanZ.Player.Events;

namespace PlanZ.Player.Components
{
    // Listens to the generic Health events from the Player's Health component and republishes
    // them as Player-specific events. The HUD subscribes to PlayerHealthChangedEvent rather
    // than the generic EntityDamagedEvent, which would fire for every enemy and prop in the
    // scene too.
    [RequireComponent(typeof(Health))]
    public class PlayerHealthBridge : MonoBehaviour
    {
        private Health _health;

        private void Awake() => _health = GetComponent<Health>();

        private void OnEnable()
        {
            EventBus.Subscribe<EntityDamagedEvent>(HandleDamaged);
            EventBus.Subscribe<EntityHealedEvent>(HandleHealed);
            EventBus.Subscribe<EntityDiedEvent>(HandleDied);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<EntityDamagedEvent>(HandleDamaged);
            EventBus.Unsubscribe<EntityHealedEvent>(HandleHealed);
            EventBus.Unsubscribe<EntityDiedEvent>(HandleDied);
        }

        private void Start() => PublishCurrentState();

        private void HandleDamaged(EntityDamagedEvent evt)
        {
            if (evt.Target != _health) return;
            EventBus.Publish(new PlayerHealthChangedEvent(_health.CurrentHealth, _health.MaxHealth, _health.Normalized));
            EventBus.Publish(new PlayerDamagedEvent(evt.Info));
        }

        private void HandleHealed(EntityHealedEvent evt)
        {
            if (evt.Target != _health) return;
            EventBus.Publish(new PlayerHealthChangedEvent(_health.CurrentHealth, _health.MaxHealth, _health.Normalized));
        }

        private void HandleDied(EntityDiedEvent evt)
        {
            if (evt.Target != _health) return;
            EventBus.Publish(new PlayerDiedEvent(evt.KillingBlow));
        }

        // Initial state push so HUDs that subscribe after Start still see the right numbers.
        private void PublishCurrentState()
        {
            EventBus.Publish(new PlayerHealthChangedEvent(_health.CurrentHealth, _health.MaxHealth, _health.Normalized));
        }
    }
}
