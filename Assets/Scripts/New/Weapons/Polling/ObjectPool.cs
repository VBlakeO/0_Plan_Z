using System.Collections.Generic;
using UnityEngine;

namespace PlanZ.Combat.Pooling
{
    // A proper object pool: Get takes an object out, Release puts it back. Unlike a circular queue,
    // an object in use is NOT recycled until explicitly released, so spawning more objects than the
    // initial pool size grows the pool instead of teleporting active instances.
    public class ObjectPool
    {
        private readonly GameObject _prefab;
        private readonly Transform _parent;
        private readonly Stack<GameObject> _available;

        public ObjectPool(GameObject prefab, int initialSize, Transform parent = null)
        {
            _prefab = prefab;
            _parent = parent;
            _available = new Stack<GameObject>(initialSize);

            for (int i = 0; i < initialSize; i++)
                _available.Push(CreateInstance());
        }

        public GameObject Get(Vector3 position, Quaternion rotation)
        {
            GameObject instance = _available.Count > 0 ? _available.Pop() : CreateInstance();

            instance.transform.SetPositionAndRotation(position, rotation);
            instance.SetActive(true);
            return instance;
        }

        public void Release(GameObject instance)
        {
            if (instance == null) return;

            instance.SetActive(false);
            instance.transform.SetParent(_parent, false);
            _available.Push(instance);
        }

        private GameObject CreateInstance()
        {
            GameObject instance = Object.Instantiate(_prefab, _parent);
            instance.SetActive(false);
            return instance;
        }
    }
}
