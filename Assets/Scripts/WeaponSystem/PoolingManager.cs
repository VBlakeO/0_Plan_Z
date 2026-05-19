using System.Collections.Generic;
using UnityEngine;

public class PoolingManager : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string tag = "";   // Tag para identificar o tipo de objeto
        public GameObject prefab = null;   // Prefab que será instanciado
        public Transform initPosition = null;   // Prefab que será instanciado
        public int size = 10;   // Tamanho inicial da pool
    }

    [SerializeField] private List<Pool> pools = new List<Pool>();   // Lista de pools
    [SerializeField] private Dictionary<string, Queue<GameObject>> poolDictionary = new Dictionary<string, Queue<GameObject>>();

    void Awake()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab, pool.initPosition.position, Quaternion.identity, pool.initPosition);
                
                if (obj.GetComponent<SelfDeactive>())
                    obj.GetComponent<SelfDeactive>().origin = pool.initPosition;


                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"Pool com a tag {tag} não existe.");
            return null;
        }

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }
}

/*
    public Transform[] m_poolingDecal;
    public Transform[] m_poolingBlood;    

    int activateDecal = 0;
    int activateBlood = 0;

    public void ActivateDecal(RaycastHit hit)
    {
        if (activateDecal < m_poolingDecal.Length)
        {
            m_poolingDecal[activateDecal].position = hit.point + hit.normal * 0.01f;
            m_poolingDecal[activateDecal].forward = hit.normal;
            m_poolingDecal[activateDecal].SetParent(hit.transform);
            m_poolingDecal[activateDecal].gameObject.SetActive(true);

            activateDecal++;
        }
        else
        {
            activateDecal = 0;
            m_poolingDecal[activateDecal].position = hit.point + hit.normal * 0.01f;
            m_poolingDecal[activateDecal].forward = hit.normal;
            m_poolingDecal[activateDecal].SetParent(hit.transform);
            m_poolingDecal[activateDecal].gameObject.SetActive(true);

            activateDecal++;
        }
    }

    public void ActivateBlood(RaycastHit hit)
    {
        if (activateBlood < m_poolingBlood.Length)
        {
            m_poolingBlood[activateBlood].position = hit.point + hit.normal * 0.01f;
            m_poolingBlood[activateBlood].rotation = Quaternion.LookRotation(hit.normal);
            m_poolingBlood[activateBlood].SetParent(null);
            m_poolingBlood[activateBlood].gameObject.SetActive(true);

            activateBlood++;
        }
        else
        {
            activateBlood = 0;
            m_poolingBlood[activateBlood].position = hit.point + hit.normal * 0.01f;
            m_poolingBlood[activateBlood].rotation = Quaternion.LookRotation(hit.normal);
            m_poolingBlood[activateBlood].SetParent(null);
            m_poolingBlood[activateBlood].gameObject.SetActive(true);

            activateBlood++;
        }
    }
*/