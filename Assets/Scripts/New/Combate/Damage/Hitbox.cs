using UnityEngine;

namespace PlanZ.Combat.Damage
{
    // Sits on body parts (head, torso, limbs) with a Collider and forwards damage to the central
    // Health component, applying a region-specific multiplier first. The weapon raycast finds
    // the Hitbox via GetComponentInParent<IDamageable>, so each body part can have its own
    // collider and the system stays decoupled from any specific creature anatomy.
    public class Hitbox : MonoBehaviour, IDamageable
    {
        [SerializeField] private Health targetHealth;
        [SerializeField] private float damageMultiplier = 1f;

        // The auto-resolve runs only when the inspector reference is empty. Manual assignment
        // wins because rigs sometimes have multiple Health components (a vehicle with a driver
        // inside, for example) and we can't guess which one this hitbox should hit.
        private void Awake()
        {
            if (targetHealth == null)
                targetHealth = GetComponentInParent<Health>();
        }

        public void ApplyDamage(DamageInfo info)
        {
            if (targetHealth == null) return;

            DamageInfo scaled = info.WithAmount(info.Amount * damageMultiplier);
            targetHealth.ApplyDamage(scaled);
        }
    }
}
