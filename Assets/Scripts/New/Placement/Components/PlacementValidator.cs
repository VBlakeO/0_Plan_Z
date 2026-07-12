using UnityEngine;

namespace PlanZ.Placement.Components
{
    // Counter-based collision detector. Uses a trigger collider to detect overlap with other
    // objects in the scene. IsValid is true only when zero overlapping objects are present.
    // Using a counter (rather than a single bool) makes the detector robust when multiple
    // colliders enter/exit overlapping the ghost at the same time.
    [RequireComponent(typeof(Collider))]
    public class PlacementValidator : MonoBehaviour
    {
        [SerializeField] private LayerMask blockingLayers = ~0;

        private int _overlapCount;

        public bool IsValid => _overlapCount == 0;

        public void ResetCounter() => _overlapCount = 0;

        private void OnTriggerEnter(Collider other)
        {
            if ((blockingLayers.value & (1 << other.gameObject.layer)) == 0) return;
            _overlapCount++;
        }

        private void OnTriggerExit(Collider other)
        {
            if ((blockingLayers.value & (1 << other.gameObject.layer)) == 0) return;
            _overlapCount = Mathf.Max(0, _overlapCount - 1);
        }
    }
}
