using UnityEngine;
using PlanZ.Combat.Damage;
using PlanZ.Events;
using PlanZ.Healing.Events;
using PlanZ.Player.Input;

namespace PlanZ.Healing.Components
{
    // Self-targeted "hold to use" healing. Listens for HealPressed/Released input, ticks
    // progress while the key is held, applies healing via the player's Health component on
    // completion, and consumes one medkit from the inventory. Sets HealingMode on PlayerInput
    // so weapons and placement are suppressed during the channel.
    public class HealController : MonoBehaviour
    {
        [SerializeField] private Health playerHealth;
        [SerializeField] private MedkitInventory inventory;

        [Header("Hold")]
        [SerializeField] private float holdDuration = 2f;

        [Header("Heal effect")]
        [SerializeField] private float healAmount = 50f;
        [SerializeField] private bool blockIfAlreadyFull = true;

        private float _progress;
        private bool _isHealing;

        public bool IsHealing => _isHealing;
        public float Progress => _progress;

        private void OnEnable()
        {
            EventBus.Subscribe<HealPressedEvent>(HandlePressed);
            EventBus.Subscribe<HealReleasedEvent>(HandleReleased);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<HealPressedEvent>(HandlePressed);
            EventBus.Unsubscribe<HealReleasedEvent>(HandleReleased);

            // Safety net: if the controller is disabled mid-heal (scene change, player death),
            // make sure the input mode flag is cleared so other systems don't stay suppressed.
            if (_isHealing) CancelHeal(publishEvent: false);
        }

        // Conditions to start: inventory has a kit, player is alive, not already in another
        // exclusive mode (placement), and (optionally) HP is not already full. Blocking when
        // full prevents wasting a kit; designers can disable this to allow over-time channels
        // for buff effects added later.
        private void HandlePressed()
        {
            if (_isHealing) return;
            if (PlayerInput.Instance != null && PlayerInput.Instance.PlacementMode) return;
            if (inventory == null || !inventory.HasAny) return;
            if (playerHealth == null || playerHealth.IsDead || playerHealth.IsDying) return;

            if (blockIfAlreadyFull && playerHealth.CurrentHealth >= playerHealth.MaxHealth)
                return;

            BeginHeal();
        }

        private void HandleReleased()
        {
            if (!_isHealing) return;
            CancelHeal(publishEvent: true);
        }

        private void Update()
        {
            if (!_isHealing) return;

            _progress = Mathf.Clamp01(_progress + Time.deltaTime / holdDuration);
            EventBus.Publish(new HealProgressedEvent(_progress));

            if (_progress >= 1f) CompleteHeal();
        }

        private void BeginHeal()
        {
            _isHealing = true;
            _progress = 0f;
            SetHealingMode(true);
            EventBus.Publish(new HealStartedEvent());
        }

        private void CompleteHeal()
        {
            // Consume only on success - if the inventory was emptied by another system mid-hold
            // (cheat menu, save load), abort without applying the heal or wasting nothing.
            if (!inventory.TryConsume())
            {
                CancelHeal(publishEvent: true);
                return;
            }

            float before = playerHealth.CurrentHealth;
            playerHealth.Heal(healAmount);
            float actual = playerHealth.CurrentHealth - before;

            _isHealing = false;
            _progress = 0f;
            SetHealingMode(false);
            EventBus.Publish(new HealCompletedEvent(actual));
        }

        private void CancelHeal(bool publishEvent)
        {
            _isHealing = false;
            _progress = 0f;
            SetHealingMode(false);

            if (publishEvent) EventBus.Publish(new HealCancelledEvent());
        }

        private void SetHealingMode(bool value)
        {
            if (PlayerInput.Instance == null) return;
            PlayerInput.Instance.HealingMode = value;
        }
    }
}
