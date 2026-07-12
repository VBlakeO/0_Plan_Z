using UnityEngine;

namespace PlanZ.Interaction.Data
{
    [CreateAssetMenu(fileName = "InteractionData", menuName = "PlanZ/Interaction/Interaction Data")]
    public class InteractionData : ScriptableObject
    {
        [Header("Display")]
        [SerializeField] private string promptLabel = "Interact";

        [Header("Behaviour")]
        [SerializeField] private InteractionMode mode = InteractionMode.Quick;
        [SerializeField] private float duration = 1f;
        [SerializeField] private float cooldownAfterComplete = 1f;
        [SerializeField] private float completedEventDelay;
        [SerializeField] private bool singleUse;

        public string PromptLabel => promptLabel;
        public InteractionMode Mode => mode;
        public float Duration => duration;
        public float CooldownAfterComplete => cooldownAfterComplete;
        public float CompletedEventDelay => completedEventDelay;
        public bool SingleUse => singleUse;
    }
}

public enum InteractionMode
{
    // Single press triggers completion immediately. Holding the key after the first frame
    // does not retrigger. Good for: doors, switches, picking up items.
    Quick,

    // Player must hold the key for the full duration. Releasing before completion cancels
    // the interaction and resets the progress. Good for: hacking, planting bombs.
    Hold,

    // Player holds the key, but progress is preserved if they release and resume later.
    // Useful for: long crafting, lockpicking with stamina mechanics.
    Continuous
}