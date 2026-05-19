using UnityEngine.AI;
using UnityEngine;

public class BaseAI : ComplexLivingBeing
{
    [Space]
    [Header("BaseAI")]
    public State state = State.Chasing;
    [Space]

    [SerializeField] protected float defaltSpeed = 3f;
    [SerializeField] protected float distance = 100f;
    [SerializeField] protected NavMeshAgent agent = null;
    [SerializeField] protected float pathDistance = 0f;
    protected float slow = 1f;
    [Space]

    [SerializeField] protected float initialAttackDistance = 2f;
    public float attackDistance = 0f;

    [Space]
    [SerializeField] protected Transform target = null;

    protected virtual void OnEnable()
    {
        SetAttackDistance(initialAttackDistance);
    }

    public void SetTarget(Transform _target)
    {
        target = _target;
    }

    public Transform GetTarget()
    {
        return target;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<SlowAreaEffect>())
            ApllySlow(other.GetComponent<SlowAreaEffect>().intensity);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<SlowAreaEffect>())
            RemoveSlow();
    }

    public void ApllySlow(float intensity)
    {
        slow = intensity;
        agent.speed = defaltSpeed * slow;
    }

    public void RemoveSlow()
    {
        slow = 1f;
        agent.speed = defaltSpeed * slow;
    }

    public void SetAttackDistance(float _distance)
    {
        attackDistance = _distance;
    }

    protected void CalculatePathDistance()
    {
        NavMeshPath path = agent.path;

        pathDistance = 0f;
       
        for (int i = 0; i < path.corners.Length - 1; i++)
            pathDistance += Vector3.Distance(path.corners[i], path.corners[i + 1]);
    }

    void OnDrawGizmos()
    {
        if (agent == null || agent.path == null)
            return;

        // Obtém os cantos do caminho
        Vector3[] corners = agent.path.corners;

        // Desenha o caminho no editor
        Gizmos.color = Color.red;
        for (int i = 0; i < corners.Length - 1; i++)
        {
            Gizmos.DrawLine(corners[i], corners[i + 1]);
        }
    }
}
