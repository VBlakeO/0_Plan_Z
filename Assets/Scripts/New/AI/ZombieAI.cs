using UnityEngine;
using PlanZ.AI.Zombies.Data;
using PlanZ.AI.Zombies.Events;
using PlanZ.AI.Zombies.Targets;
using PlanZ.Combat.Damage;
using PlanZ.Events;

namespace PlanZ.AI.Zombies.Components
{
    // State machine orchestrator. Holds the current state and target, ticks the active state,
    // and centralizes transitions through ChangeState so the event publishes consistently.
    // Specific behaviours live in companion components (movement, combat, animator) that listen
    // to events rather than reading from here directly.
    [RequireComponent(typeof(ZombieMovement))]
    [RequireComponent(typeof(ZombieCombat))]
    [RequireComponent(typeof(Health))]
    public class ZombieAI : MonoBehaviour
    {
        [SerializeField] private ZombieData data;
        [SerializeField] private string playerTag = "Player";

        private ZombieMovement _movement;
        private ZombieCombat _combat;
        private Health _health;

        private IZombieTarget _target;
        private ZombieState _state = ZombieState.Idle;
        private float _distanceToTarget;
        private float _staggerEndTime;

        public ZombieData Data => data;
        public ZombieState State => _state;
        public IZombieTarget Target => _target;
        public float DistanceToTarget => _distanceToTarget;

        private void Awake()
        {
            _movement = GetComponent<ZombieMovement>();
            _combat = GetComponent<ZombieCombat>();
            _health = GetComponent<Health>();
        }

        private void OnEnable()
        {
            EventBus.Subscribe<EntityDamagedEvent>(HandleDamaged);
            EventBus.Subscribe<EntityDiedEvent>(HandleDied);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<EntityDamagedEvent>(HandleDamaged);
            EventBus.Unsubscribe<EntityDiedEvent>(HandleDied);
        }

        private void Start()
        {
            ResolveInitialTarget();
            ChangeState(data.StartIdle ? ZombieState.Idle : ZombieState.Chasing);
        }

        // Player tag is the default lookup but the target system is interface-based, so any
        // GameObject with IZombieTarget could be assigned by other systems (e.g. a spawner
        // pointing zombies at a generator objective).
        private void ResolveInitialTarget()
        {
            GameObject tagged = GameObject.FindGameObjectWithTag(playerTag);
            if (tagged == null) return;

            IZombieTarget target = tagged.GetComponentInParent<IZombieTarget>();
            if (target == null) target = tagged.GetComponent<IZombieTarget>();
            if (target != null) SetTarget(target);
        }

        public void SetTarget(IZombieTarget target) => _target = target;

        private void Update()
        {
            if (_state == ZombieState.Dead) return;
            if (_target == null || !_target.IsValidTarget) return;

            _distanceToTarget = Vector3.Distance(transform.position, _target.Transform.position);
            _movement.RecalculatePathDistance();

            TickCurrentState();

            EventBus.Publish(new ZombieMovementTickEvent(this, _movement.PathDistance));
        }

        // Each state tick reads its own conditions and may request a transition. Transitions
        // happen through ChangeState so the event is always published and Enter side effects
        // (stop agent, etc.) run consistently.
        private void TickCurrentState()
        {
            switch (_state)
            {
                case ZombieState.Idle: TickIdle(); break;
                case ZombieState.Chasing: TickChasing(); break;
                case ZombieState.Attacking: TickAttacking(); break;
                case ZombieState.Staggered: TickStaggered(); break;
            }
        }

        private void TickIdle()
        {
            if (_distanceToTarget <= data.HearingRange)
                ChangeState(ZombieState.Chasing);
        }

        private void TickChasing()
        {
            if (_distanceToTarget <= data.AttackRange)
            {
                ChangeState(ZombieState.Attacking);
                return;
            }

            _movement.MoveTo(_target.Transform.position);

            // Manual rotation kicks in only when the path is short - long approach paths use
            // NavMesh's own steering for smoother turns at corners.
            if (_movement.ShouldManuallyRotate)
                _movement.RotateTowards(_target.Transform.position);
        }

        private void TickAttacking()
        {
            if (_distanceToTarget > data.AttackRange)
            {
                ChangeState(ZombieState.Chasing);
                return;
            }

            _movement.RotateTowards(_target.Transform.position);

            // Damage is applied by the claw hitbox during specific animation frames, not here.
            // The cooldown gates how often a NEW attack swing can start, so the zombie doesn't
            // spam-trigger the animation faster than it can play out.
            if (!_combat.IsOnCooldown)
            {
                int variant = Random.Range(0, 2);
                EventBus.Publish(new ZombieAttackStartedEvent(this, _target.Kind, variant));
                _combat.BeginSwing();
            }
        }

        private void TickStaggered()
        {
            if (Time.time >= _staggerEndTime)
                ChangeState(ZombieState.Chasing);
        }

        // Centralizing state transitions ensures Enter logic and the event are paired. Death
        // is special-cased to be terminal: nothing transitions out of Dead.
        public void ChangeState(ZombieState newState)
        {
            if (_state == newState) return;
            if (_state == ZombieState.Dead) return;

            ZombieState previous = _state;
            _state = newState;

            EnterState(newState);
            EventBus.Publish(new ZombieStateChangedEvent(this, previous, newState));
        }

        private void EnterState(ZombieState newState)
        {
            switch (newState)
            {
                case ZombieState.Idle:
                case ZombieState.Staggered:
                    _movement.Stop();
                    break;

                case ZombieState.Chasing:
                    if (_target != null) _movement.MoveTo(_target.Transform.position);
                    break;

                case ZombieState.Attacking:
                    _movement.Stop();
                    break;

                case ZombieState.Dead:
                    _movement.DisableAgent();
                    break;
            }

            if (newState == ZombieState.Staggered)
                _staggerEndTime = Time.time + data.StaggerDuration;
        }

        // Heavy hits stagger the zombie briefly. The threshold prevents tiny scratches from
        // interrupting attacks, which would feel cheesy to abuse from the player side.
        private void HandleDamaged(EntityDamagedEvent evt)
        {
            if (evt.Target != _health) return;
            if (_state == ZombieState.Dead || _state == ZombieState.Staggered) return;

            if (evt.Info.Amount >= data.StaggerDamageThreshold)
                ChangeState(ZombieState.Staggered);
        }

        private void HandleDied(EntityDiedEvent evt)
        {
            if (evt.Target != _health) return;
            ChangeState(ZombieState.Dead);
        }
    }
}