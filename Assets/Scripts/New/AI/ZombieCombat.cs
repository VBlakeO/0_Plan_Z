using UnityEngine;
using PlanZ.AI.Zombies.Data;

namespace PlanZ.AI.Zombies.Components
{
    public class ZombieCombat : MonoBehaviour
    {
        [SerializeField] private ZombieData data;

        private float _nextSwingTime;

        public bool IsOnCooldown => Time.time < _nextSwingTime;

        public void BeginSwing()
        {
            _nextSwingTime = Time.time + data.AttackCooldown;
        }

        public void ResetCooldown() => _nextSwingTime = 0f;
    }
}