using UnityEngine;

namespace PlanZ.Weapons.Data
{
    [CreateAssetMenu(fileName = "RecoilPattern", menuName = "PlanZ/Weapons/Recoil Pattern")]
    public class RecoilPattern : ScriptableObject
    {
        [Header("Spray pattern (x = horizontal, y = vertical kick per shot)")]
        [SerializeField] private Vector2[] pattern = {
            new(0.00f, 0.30f),
            new(-0.05f, 0.35f),
            new(0.05f, 0.40f),
            new(-0.10f, 0.45f),
            new(0.10f, 0.45f),
            new(-0.15f, 0.50f),
            new(0.15f, 0.50f),
            new(-0.20f, 0.55f),
            new(0.20f, 0.55f),
            new(0.00f, 0.55f)
        };

        [Header("Randomness")]
        [SerializeField, Range(0f, 1f)] private float randomness = 0.15f;

        [Header("Loop behaviour")]
        [SerializeField] private bool loopAfterEnd = true;
        [SerializeField] private Vector2 loopedPatternRange = new(-0.20f, 0.20f);
        [SerializeField] private float loopedVerticalKick = 0.50f;

        public int Length => pattern.Length;

        // Returns the kick for a given shot index. When the pattern is exhausted, either loops
        // back to randomized values within a configured range, or stays clamped on the last point.
        public Vector2 GetKick(int shotIndex)
        {
            Vector2 baseKick = ResolveBaseKick(shotIndex);
            return ApplyRandomness(baseKick);
        }

        private Vector2 ResolveBaseKick(int shotIndex)
        {
            if (pattern.Length == 0) return Vector2.zero;
            if (shotIndex < pattern.Length) return pattern[shotIndex];
            if (!loopAfterEnd) return pattern[pattern.Length - 1];

            float horizontal = Random.Range(loopedPatternRange.x, loopedPatternRange.y);
            return new Vector2(horizontal, loopedVerticalKick);
        }

        private Vector2 ApplyRandomness(Vector2 baseKick)
        {
            if (randomness <= 0f) return baseKick;

            float jitterX = baseKick.x * Random.Range(-randomness, randomness);
            float jitterY = baseKick.y * Random.Range(-randomness, randomness);
            return new Vector2(baseKick.x + jitterX, baseKick.y + jitterY);
        }
    }
}
