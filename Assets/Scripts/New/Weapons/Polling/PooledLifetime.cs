using UnityEngine;
using PlanZ.Combat.Pooling;

namespace PlanZ.Combat.Pooling
{
    // Attach to a pooled prefab so it returns itself to the registry after a configurable
    // lifetime. Hooks OnEnable instead of Start so it resets each time the pool spawns it.
    public class PooledLifetime : MonoBehaviour
    {
        [SerializeField] private string poolId;
        [SerializeField] private float lifetime = 2f;
        [SerializeField] private bool resetTrailOnSpawn = true;
        [SerializeField] private bool resetParticlesOnSpawn = true;

        private TrailRenderer _trail;
        private ParticleSystem[] _particles;
        private float _expireAt;

        private void Awake()
        {
            _trail = GetComponent<TrailRenderer>();
            _particles = GetComponentsInChildren<ParticleSystem>(true);
        }

        private void OnEnable()
        {
            _expireAt = Time.time + lifetime;

            if (resetTrailOnSpawn && _trail != null)
                _trail.Clear();

            if (resetParticlesOnSpawn && _particles != null)
                ResetParticles();
        }

        private void Update()
        {
            if (Time.time < _expireAt) return;
            ReleaseToPool();
        }

        private void ResetParticles()
        {
            for (int i = 0; i < _particles.Length; i++)
            {
                _particles[i].Clear(true);
                _particles[i].Play(true);
            }
        }

        private void ReleaseToPool()
        {
            if (PoolRegistry.Instance == null || string.IsNullOrEmpty(poolId))
            {
                gameObject.SetActive(false);
                return;
            }

            PoolRegistry.Instance.Release(poolId, gameObject);
        }
    }
}
