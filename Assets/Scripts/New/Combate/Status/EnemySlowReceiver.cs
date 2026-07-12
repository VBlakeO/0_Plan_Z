using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace PlanZ.Combat.Status
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemySlowReceiver : MonoBehaviour, ISlowable
    {
        private const float NoSlow = 1f;

        private NavMeshAgent _agent;
        private float _baseSpeed;
        private readonly Dictionary<object, float> _slowSources = new();

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
        }

        private void Start()
        {
            _baseSpeed = _agent.speed;
        }

        public void ApplySlow(object source, float multiplier)
        {
            _slowSources[source] = multiplier;
            RecalculateSpeed();
        }

        public void RemoveSlow(object source)
        {
            if (!_slowSources.Remove(source)) return;
            RecalculateSpeed();
        }

        private void RecalculateSpeed()
        {
            float strongest = NoSlow;
            foreach (float multiplier in _slowSources.Values)
            {
                if (multiplier < strongest) strongest = multiplier;
            }

            _agent.speed = _baseSpeed * strongest;
        }
    }
}