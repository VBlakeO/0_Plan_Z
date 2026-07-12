using UnityEngine;
using PlanZ.Combat.Pooling;

namespace PlanZ.Combat.Pooling
{
    // Visual-only tracer for hitscan weapons. Draws an instantaneous LineRenderer from the
    // weapon muzzle to the impact point, then fades and returns to the pool. The damage was
    // already applied by the shooter's raycast; this component is pure VFX.
    [RequireComponent(typeof(LineRenderer))]
    public class BulletTracer : MonoBehaviour
    {
        [SerializeField] private string poolId = "Tracer";
        [SerializeField] private float lifetime = 0.08f;
        [SerializeField] private AnimationCurve widthOverLifetime = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
        [SerializeField] private float minTravelDistance = 0.5f;

        private LineRenderer _line;
        private float _baseStartWidth;
        private float _baseEndWidth;
        private float _expireAt;
        private bool _isActive;

        private void Awake()
        {
            _line = GetComponent<LineRenderer>();
            _baseStartWidth = _line.startWidth;
            _baseEndWidth = _line.endWidth;
            _line.positionCount = 2;
            _line.useWorldSpace = true;
        }

        public void Launch(Vector3 origin, Vector3 destination)
        {
            float distance = Vector3.Distance(origin, destination);
            if (distance < minTravelDistance)
            {
                Release();
                return;
            }

            _line.SetPosition(0, origin);
            _line.SetPosition(1, destination);
            _line.startWidth = _baseStartWidth;
            _line.endWidth = _baseEndWidth;

            _expireAt = Time.time + lifetime;
            _isActive = true;
        }

        // The width curve drives the fade so the tracer feels like a momentary flash rather than
        // a hard pop. Evaluating at progress 0 returns the configured base widths; at 1 returns 0.
        private void Update()
        {
            if (!_isActive) return;

            float remaining = _expireAt - Time.time;
            if (remaining <= 0f)
            {
                Release();
                return;
            }

            float progress = 1f - (remaining / lifetime);
            float multiplier = widthOverLifetime.Evaluate(progress);

            _line.startWidth = _baseStartWidth * multiplier;
            _line.endWidth = _baseEndWidth * multiplier;
        }

        private void Release()
        {
            _isActive = false;

            if (PoolRegistry.Instance == null || string.IsNullOrEmpty(poolId))
            {
                gameObject.SetActive(false);
                return;
            }

            PoolRegistry.Instance.Release(poolId, gameObject);
        }
    }
}
