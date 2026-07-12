using UnityEngine;

namespace PlanZ.AI.Zombies.Data
{
    [CreateAssetMenu(fileName = "ZombieData", menuName = "PlanZ/AI/Zombie Data")]
    public class ZombieData : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] private string displayName = "Zombie";

        [Header("Detection")]
        [SerializeField] private float hearingRange = 10f;

        // When true, the zombie starts in Idle and only activates when the player enters
        // hearing range. When false, the zombie starts already in Chasing mode.
        [SerializeField] private bool startIdle;

        [Header("Movement")]
        [SerializeField] private float chaseSpeed = 3.5f;
        [SerializeField] private float rotationSpeed = 6f;
        [SerializeField] private float pathRotationThreshold = 8f;

        [Header("Attack")]
        [SerializeField] private float attackRange = 1.8f;
        [SerializeField] private float attackDamage = 10f;
        [SerializeField] private float attackCooldown = 1.2f;

        [Header("Stagger")]
        [SerializeField] private float staggerDamageThreshold = 30f;
        [SerializeField] private float staggerDuration = 0.6f;

        public string DisplayName => displayName;
        public float HearingRange => hearingRange;
        public bool StartIdle => startIdle;
        public float ChaseSpeed => chaseSpeed;
        public float RotationSpeed => rotationSpeed;
        public float PathRotationThreshold => pathRotationThreshold;
        public float AttackRange => attackRange;
        public float AttackDamage => attackDamage;
        public float AttackCooldown => attackCooldown;
        public float StaggerDamageThreshold => staggerDamageThreshold;
        public float StaggerDuration => staggerDuration;
    }
}
