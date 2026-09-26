using UnityEngine;

namespace PlanZ.AI.Zombies.Components
{
    // OnAnimatorMove is only delivered to components living on the GameObject that owns the
    // Animator. Rigs are usually a child of the zombie root, so this relay sits next to the
    // Animator and forwards the root motion delta up to the movement component.
    [RequireComponent(typeof(Animator))]
    public class ZombieRootMotionRelay : MonoBehaviour
    {
        [SerializeField] private ZombieMovement movement;

        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _animator.applyRootMotion = true;

            if (movement == null) movement = GetComponentInParent<ZombieMovement>();
        }

        private void OnAnimatorMove()
        {
            if (movement == null) return;

            movement.ApplyRootMotion(_animator.deltaPosition);
        }
    }
}
