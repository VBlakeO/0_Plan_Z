using UnityEngine;
using PlanZ.Combat.Damage;

namespace PlanZ.AI.Zombies.Targets
{
    // Drop-in component that marks a GameObject as targetable by zombies. The Kind drives the
    // zombie's attack flavour; IsValidTarget becomes false when the entity dies so zombies stop
    // pathing toward corpses and doors that have been broken open.
    public class ZombieTarget : MonoBehaviour, IZombieTarget
    {
        [SerializeField] private ZombieTargetKind kind = ZombieTargetKind.Generic;
        [SerializeField] private Health healthRef;

        public Transform Transform => transform;
        public ZombieTargetKind Kind => kind;

        // If a Health reference is provided, the target becomes invalid once it's dead or dying.
        // Without a Health (e.g. inanimate door before destruction is implemented) the target is
        // always valid.
        public bool IsValidTarget
        {
            get
            {
                if (healthRef == null) return true;
                return !healthRef.IsDead && !healthRef.IsDying;
            }
        }

        private void Reset()
        {
            healthRef = GetComponentInParent<Health>();
        }
    }
}
