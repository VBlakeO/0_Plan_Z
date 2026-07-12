using UnityEngine;
using PlanZ.AI.Zombies.Events;
using PlanZ.Events;

namespace PlanZ.AI.Zombies.Components
{
    // Toggles ragdoll physics when the zombie dies. Caches rigidbodies and the base collider
    // once on awake instead of re-querying on each enable. Lives separately from the animator
    // so a designer can enable ragdoll on a zombie without animation, or vice versa.
    public class ZombieRagdoll : MonoBehaviour
    {
        [SerializeField] private ZombieAI zombie;
        [SerializeField] private Animator anim;
        [SerializeField] private Collider baseCollider;
        [SerializeField] private Rigidbody[] ragdollBodies;

        private void Awake()
        {
            if (ragdollBodies == null || ragdollBodies.Length == 0)
                ragdollBodies = GetComponentsInChildren<Rigidbody>();

            SetRagdollActive(false);
        }

        private void OnEnable() => EventBus.Subscribe<ZombieStateChangedEvent>(HandleStateChanged);

        private void OnDisable() => EventBus.Unsubscribe<ZombieStateChangedEvent>(HandleStateChanged);

        private void HandleStateChanged(ZombieStateChangedEvent evt)
        {
            if (evt.Zombie != zombie) return;
            if (evt.Current != ZombieState.Dead) return;

            SetRagdollActive(true);
        }

        // When ragdoll engages, animator is disabled (otherwise it'd fight the physics) and the
        // base collider is removed so the body falls without the upright capsule interfering.
        // Rigidbodies switch from kinematic (animated) to physics (free fall).
        private void SetRagdollActive(bool active)
        {
            if (baseCollider != null) baseCollider.enabled = !active;
            if (anim != null) anim.enabled = !active;

            for (int i = 0; i < ragdollBodies.Length; i++)
            {
                if (ragdollBodies[i] != null)
                    ragdollBodies[i].isKinematic = !active;
            }
        }
    }
}
