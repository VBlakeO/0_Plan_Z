using UnityEngine;

namespace PlanZ.AI.Zombies.Targets
{
    public enum ZombieTargetKind
    {
        Player,
        Door,
        Generic
    }

    // Implemented by anything zombies want to attack. The Kind drives which attack animation
    // the zombie plays (clawing the player versus pounding on a door) without the zombie
    // needing to know any specific game class.
    public interface IZombieTarget
    {
        Transform Transform { get; }
        ZombieTargetKind Kind { get; }
        bool IsValidTarget { get; }
    }
}
