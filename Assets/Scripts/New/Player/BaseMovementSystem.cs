using UnityEngine;

namespace PlanZ.Player
{
    public abstract class BaseMovementSystem : MonoBehaviour
    {
        private const float NoSlow = 1f;

        public float Speed { get; set; } = 3f;
        public float SlowMultiplier { get; private set; } = NoSlow;

        public void ApplySlow(float intensity) => SlowMultiplier = intensity;

        public void RemoveSlow() => SlowMultiplier = NoSlow;
    }
}
