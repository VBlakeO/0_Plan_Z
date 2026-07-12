using System.Collections.Generic;
using UnityEngine;
using PlanZ.Core;

namespace PlanZ.Combat.Pooling
{
    // Replaces the old PoolingManager. Pools are registered through serialized definitions and
    // accessed by an identifier (typed enum or string key) so consumers don't pass magic strings
    // through call sites. Each pool wraps a real ObjectPool instead of the original circular queue.
    public class PoolRegistry : SingletonMonoBehaviour<PoolRegistry>
    {
        [System.Serializable]
        public class PoolDefinition
        {
            public string id;
            public GameObject prefab;
            public int initialSize = 10;
            public Transform parent;
        }

        [SerializeField] private List<PoolDefinition> definitions = new();

        private readonly Dictionary<string, ObjectPool> _pools = new();

        protected override void Awake()
        {
            base.Awake();
            BuildPools();
        }

        private void BuildPools()
        {
            foreach (var def in definitions)
            {
                if (string.IsNullOrEmpty(def.id) || def.prefab == null) continue;
                _pools[def.id] = new ObjectPool(def.prefab, def.initialSize, def.parent);
            }
        }

        public GameObject Spawn(string poolId, Vector3 position, Quaternion rotation)
        {
            if (!_pools.TryGetValue(poolId, out ObjectPool pool))
            {
                Debug.LogWarning($"PoolRegistry: pool '{poolId}' was not registered.");
                return null;
            }

            return pool.Get(position, rotation);
        }

        public void Release(string poolId, GameObject instance)
        {
            if (!_pools.TryGetValue(poolId, out ObjectPool pool)) return;
            pool.Release(instance);
        }
    }
}
