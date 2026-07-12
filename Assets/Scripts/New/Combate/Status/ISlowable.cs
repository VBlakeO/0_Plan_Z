namespace PlanZ.Combat.Status
{
    // Implemented by anything that can be slowed (player movement, enemy NavMeshAgent driver).
    // Slows are keyed by source so multiple overlapping effects (two traps, a trap plus a web)
    // don't clobber each other - each source applies and removes its own slow independently.
    // The implementer decides how to combine active slows (typically: use the strongest).
    public interface ISlowable
    {
        // multiplier is a speed scale in (0, 1]; 0.5 means half speed. The source is any object
        // reference used as a key to later remove this specific slow.
        void ApplySlow(object source, float multiplier);

        void RemoveSlow(object source);
    }
}
