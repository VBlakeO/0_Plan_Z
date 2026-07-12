using UnityEngine;
using UnityEngine.AI;
using PlanZ.AI.Zombies.Data;

namespace PlanZ.AI.Zombies.Components
{
    // Movement primitives for the zombie. Owns the NavMeshAgent and exposes a small API the
    // state machine calls. Keeping this isolated from state logic means future locomotion
    // changes (root motion, custom navigation) only touch this component.
    [RequireComponent(typeof(NavMeshAgent))]
    public class ZombieMovement : MonoBehaviour
    {
        [SerializeField] private ZombieData data;

        private const float MinRotateDirectionSqr = 0.001f;

        private NavMeshAgent _agent;

        public NavMeshAgent Agent => _agent;
        public float PathDistance { get; private set; }

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _agent.speed = data.ChaseSpeed;
        }

        public void MoveTo(Vector3 destination)
        {
            if (!_agent.enabled || !_agent.isOnNavMesh) return;

            _agent.isStopped = false;
            _agent.SetDestination(destination);
        }

        public void Stop()
        {
            if (!_agent.enabled || !_agent.isOnNavMesh) return;

            _agent.isStopped = true;
            _agent.velocity = Vector3.zero;
        }

        public void DisableAgent()
        {
            if (_agent.enabled) _agent.enabled = false;
        }

        // Manually orienting the zombie when close to the target gives more natural-looking
        // facing during attacks than letting the agent fully control rotation. The threshold
        // limits this to the final approach so distant movement still uses NavMesh smoothing.
        public void RotateTowards(Vector3 worldTarget)
        {
            Vector3 direction = worldTarget - transform.position;
            direction.y = 0f;
            if (direction.sqrMagnitude < MinRotateDirectionSqr) return;

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation,
                data.RotationSpeed * Time.deltaTime);
        }

        public bool ShouldManuallyRotate => PathDistance < data.PathRotationThreshold;

        // Path distance fed to the animator so locomotion blend trees can react to the
        // remaining travel distance instead of raw velocity (smoother results around corners).
        public void RecalculatePathDistance()
        {
            PathDistance = 0f;
            if (_agent.path == null) return;

            Vector3[] corners = _agent.path.corners;
            for (int i = 0; i < corners.Length - 1; i++)
                PathDistance += Vector3.Distance(corners[i], corners[i + 1]);
        }

        private void OnDrawGizmosSelected()
        {
            if (_agent == null || _agent.path == null) return;

            Gizmos.color = Color.red;
            Vector3[] corners = _agent.path.corners;
            for (int i = 0; i < corners.Length - 1; i++)
                Gizmos.DrawLine(corners[i], corners[i + 1]);
        }
    }
}
