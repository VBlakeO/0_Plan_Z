using UnityEngine;
using PlanZ.AI.Zombies.Events;
using PlanZ.AI.Zombies.Targets;
using PlanZ.Events;

namespace PlanZ.AI.Zombies.Components
{
    // Listens to zombie events and drives the Animator. Decoupled from the AI state machine -
    // the animator only knows about events and animator parameters, not the state class layout.
    // Lets the animator be replaced (different rig, different naming convention) without
    // touching gameplay code.
    public class ZombieAnimator : MonoBehaviour
    {
        [SerializeField] private ZombieAI zombie;
        [SerializeField] private Animator anim;

        private const string AnimDistance = "Distance";
        private const string AnimWalk = "Walk";
        private const string AnimAttack = "Attack";
        private const string AnimAttack2 = "Attack2";
        private const string AnimAttackDoor = "AttackDoor";
        private const string AnimStagger = "Stagger";
        private const string AnimDie = "Die";

        private static readonly int DistanceHash = Animator.StringToHash(AnimDistance);
        private static readonly int WalkHash = Animator.StringToHash(AnimWalk);
        private static readonly int AttackHash = Animator.StringToHash(AnimAttack);
        private static readonly int Attack2Hash = Animator.StringToHash(AnimAttack2);
        private static readonly int AttackDoorHash = Animator.StringToHash(AnimAttackDoor);
        private static readonly int StaggerHash = Animator.StringToHash(AnimStagger);
        private static readonly int DieHash = Animator.StringToHash(AnimDie);

        private void OnEnable()
        {
            EventBus.Subscribe<ZombieStateChangedEvent>(HandleStateChanged);
            EventBus.Subscribe<ZombieAttackStartedEvent>(HandleAttackStarted);
            EventBus.Subscribe<ZombieMovementTickEvent>(HandleMovementTick);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<ZombieStateChangedEvent>(HandleStateChanged);
            EventBus.Unsubscribe<ZombieAttackStartedEvent>(HandleAttackStarted);
            EventBus.Unsubscribe<ZombieMovementTickEvent>(HandleMovementTick);
        }

        // Filtering each handler by zombie reference avoids one zombie's events being processed
        // by another zombie's animator. Without this, all zombies on screen would react to a
        // single attack event.
        private void HandleStateChanged(ZombieStateChangedEvent evt)
        {
            if (evt.Zombie != zombie || anim == null) return;

            // When leaving the attacking state, clear pending attack triggers so a queued swing
            // doesn't fire on the next chase frame.
            if (evt.Previous == ZombieState.Attacking)
            {
                anim.ResetTrigger(AttackHash);
                anim.ResetTrigger(Attack2Hash);
                anim.ResetTrigger(AttackDoorHash);
            }

            switch (evt.Current)
            {
                case ZombieState.Chasing:
                    anim.SetTrigger(WalkHash);
                    break;

                case ZombieState.Staggered:
                    anim.SetTrigger(StaggerHash);
                    break;

                case ZombieState.Dead:
                    anim.SetTrigger(DieHash);
                    break;
            }
        }

        private void HandleAttackStarted(ZombieAttackStartedEvent evt)
        {
            if (evt.Zombie != zombie || anim == null) return;

            anim.ResetTrigger(WalkHash);

            int hash = ResolveAttackHash(evt.TargetKind, evt.AttackVariant);
            anim.SetTrigger(hash);
        }

        private static int ResolveAttackHash(ZombieTargetKind kind, int variant)
        {
            if (kind == ZombieTargetKind.Door) return AttackDoorHash;
            return variant == 0 ? AttackHash : Attack2Hash;
        }

        private void HandleMovementTick(ZombieMovementTickEvent evt)
        {
            if (evt.Zombie != zombie || anim == null) return;
            anim.SetFloat(DistanceHash, evt.PathDistance);
        }
    }
}
