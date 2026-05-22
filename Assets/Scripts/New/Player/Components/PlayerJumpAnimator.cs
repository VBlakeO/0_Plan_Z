using UnityEngine;
using PlanZ.Events;
using PlanZ.Player.Events;

namespace PlanZ.Player.Components
{
    // Owns Animator triggers and parameters related to jumping. Listens to the jump lifecycle
    // events (start, peak, land) and writes them to the Animator without other components
    // having to know that an Animator exists.
    public class PlayerJumpAnimator : MonoBehaviour
    {
        [SerializeField] private Animator anim;

        private const string AnimJumpStart = "JumpStart";
        private const string AnimLand = "Land";
        private const string AnimIsGrounded = "IsGrounded";

        private static readonly int JumpStartHash = Animator.StringToHash(AnimJumpStart);
        private static readonly int LandHash = Animator.StringToHash(AnimLand);
        private static readonly int IsGroundedHash = Animator.StringToHash(AnimIsGrounded);

        private void OnEnable()
        {
            EventBus.Subscribe<PlayerJumpStartedEvent>(HandleJumpStarted);
            EventBus.Subscribe<PlayerGroundStateChangedEvent>(HandleGroundStateChanged);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<PlayerJumpStartedEvent>(HandleJumpStarted);
            EventBus.Unsubscribe<PlayerGroundStateChangedEvent>(HandleGroundStateChanged);
        }

        private void HandleJumpStarted()
        {
            if (anim == null) return;
            anim.SetTrigger(JumpStartHash);
        }

        private void HandleGroundStateChanged(PlayerGroundStateChangedEvent evt)
        {
            if (anim == null) return;

            anim.SetBool(IsGroundedHash, evt.IsGrounded);

            if (evt.IsGrounded)
                anim.SetTrigger(LandHash);
        }
    }
}
