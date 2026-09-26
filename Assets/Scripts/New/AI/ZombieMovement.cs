using UnityEngine;
using UnityEngine.AI;
using PlanZ.AI.Zombies.Data;

namespace PlanZ.AI.Zombies.Components
{
    // Movement primitives for the zombie. The NavMeshAgent plans the path and steers, but it no
    // longer moves the transform: the body is driven by the animation's root motion so the limp
    // cadence (push off one leg, drag the other) survives instead of being flattened into a
    // constant agent speed. The agent's internal position is resynced to the transform every
    // animation frame so pathfinding keeps working from where the body actually is.
    [RequireComponent(typeof(NavMeshAgent))]
    public class ZombieMovement : MonoBehaviour
    {
        private const float MinRotateDirectionSqr = 0.001f;

        // Beyond this gap the agent's simulated position is considered desynced from the body
        // (blocked animation, forced teleport) and is warped instead of nudged, which also
        // replans the path from the real position.
        private const float MaxAgentDriftDistance = 1f;

        [SerializeField] private ZombieData data;

        private NavMeshAgent _agent;
        private bool _isMovementBlocked;

        public NavMeshAgent Agent => _agent;
        public float PathDistance { get; private set; }
        public bool ShouldManuallyRotate => PathDistance < data.PathRotationThreshold;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();

            // Speed still matters: it feeds desiredVelocity and the steering behaviour even
            // though it no longer translates the transform. Keep it near the clip's average
            // travel speed or the agent will steer for a body that can't keep up.
            _agent.speed = data.ChaseSpeed;
            _agent.updatePosition = false;
            _agent.updateRotation = false;
        }

        // Called from the root motion relay that sits next to the Animator.
        public void ApplyRootMotion(Vector3 rootDelta)
        {
            if (!_isMovementBlocked)
                transform.position += rootDelta;

            SyncAgentPosition();
        }

        private void SyncAgentPosition()
        {
            if (!_agent.enabled || !_agent.isOnNavMesh) return;

            if (Vector3.Distance(transform.position, _agent.nextPosition) > MaxAgentDriftDistance)
            {
                _agent.Warp(transform.position);
                return;
            }

            _agent.nextPosition = transform.position;
        }

        // With agent rotation disabled, following the steering target is what keeps the zombie
        // pointed along the path. Close to the target the state machine takes over rotation, so
        // this backs off to avoid two systems fighting for the same transform.
        private void Update()
        {
            if (_isMovementBlocked) return;
            if (!_agent.enabled || !_agent.isOnNavMesh) return;
            if (ShouldManuallyRotate) return;

            RotateTowards(_agent.steeringTarget);
        }

        public void MoveTo(Vector3 destination)
        {
            if (!_agent.enabled || !_agent.isOnNavMesh) return;

            _isMovementBlocked = false;
            _agent.isStopped = false;
            _agent.SetDestination(destination);
        }

        // Blocking root motion is what actually holds the zombie in place: the locomotion clip may
        // keep playing during attacks, and its displacement would otherwise push the body forward.
        public void Stop()
        {
            _isMovementBlocked = true;

            if (!_agent.enabled || !_agent.isOnNavMesh) return;

            _agent.isStopped = true;
            _agent.velocity = Vector3.zero;
        }

        public void DisableAgent()
        {
            if (_agent.enabled) _agent.enabled = false;
        }

        // Manually orienting the zombie when close to the target gives more natural-looking
        // facing during attacks than letting the agent fully control rotation.
        public void RotateTowards(Vector3 worldTarget)
        {
            Vector3 direction = worldTarget - transform.position;
            direction.y = 0f;
            if (direction.sqrMagnitude < MinRotateDirectionSqr) return;

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation,
                data.RotationSpeed * Time.deltaTime);
        }

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