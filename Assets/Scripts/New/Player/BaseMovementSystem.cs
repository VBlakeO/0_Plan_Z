using System.Collections.Generic;
using UnityEngine;
using PlanZ.Combat.Status;

namespace PlanZ.Player
{
    public abstract class BaseMovementSystem : MonoBehaviour, ISlowable
    {
        private const float NoSlow = 1f;

        private readonly Dictionary<object, float> _slowSources = new();

        public float Speed { get; set; } = 3f;
        public float SlowMultiplier { get; private set; } = NoSlow;

        public void ApplySlow(object source, float multiplier)
        {
            _slowSources[source] = multiplier;
            RecalculateSlow();
        }

        public void RemoveSlow(object source)
        {
            if (!_slowSources.Remove(source)) return;
            RecalculateSlow();
        }

        private void RecalculateSlow()
        {
            float strongest = NoSlow;
            foreach (float multiplier in _slowSources.Values)
            {
                if (multiplier < strongest) strongest = multiplier;
            }

            SlowMultiplier = strongest;
        }
    }
}