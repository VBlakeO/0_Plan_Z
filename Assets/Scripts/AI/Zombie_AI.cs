using System;
using UnityEngine;

public enum State { Idle, Chasing, Attack, Damage, Death }

public class Zombie_AI : BaseAI
{   
    [Space]
    [Header("Zombie_AI")]

    public bool enemyInside = false;
    public float rotationSpeed = 4f;
    [SerializeField] private PlayerMovementSystem playerMovement = null;


    public Action OnAttack = null;
    public Action OnAttackDoor = null;
    public Action OutAttack = null;
    public Action<bool> OnZDie = null;
    public Action<State> OnChangeState = null;
    public Action<float> OnFixedUpdate = null;

    protected override void OnEnable()
    {
        base.OnEnable();
        target = playerMovement.transform;

        agent.speed = defaltSpeed;
        agent.SetDestination(target.position);
    }

    void FixedUpdate()
    {
        if (IsDead())
            return;

        if (!target)
        {
            target = playerMovement.transform;
            agent.SetDestination(target.position);
            SetAttackDistance(initialAttackDistance);
            return;
        }

        distance = Vector3.Distance(transform.position, target.position);

        if(!agent.enabled)
            return;

        if (state == State.Chasing)
        {
            if (distance > attackDistance && distance < attackDistance + 2f)
                agent.SetDestination(target.position);
        }

        if (state != State.Attack)
        {
            OutAttack?.Invoke();
        }
        
        if (state == State.Death)
        {
            agent.enabled = false;
            return;
        }

        if (pathDistance < 8f)
            RotateTowardsTarget();

        OnFixedUpdate?.Invoke(pathDistance);

        CheckState();
        CalculatePathDistance();
    }

    private void CheckState()
    {
        if (!target)
            return;

        if (distance <= attackDistance)
        {
            if (state != State.Attack)
            {
                ChangeState(State.Attack);
                Attack();
            }
        }
        else if (state != State.Death && state != State.Damage)
        {
            ChangeState(State.Chasing);
        }

        if (IsDead())
        {
            ChangeState(State.Death);
        }
    }

    private void ChangeState(State _state)
    {
        print(_state);
        state = _state;

        OnChangeState?.Invoke(_state);

        if (_state == State.Chasing)
        {
            agent.isStopped = false;
            agent.SetDestination(target.position);
        }

        if (_state == State.Attack)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }
    }

    void RotateTowardsTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0; // Ignora o eixo Y para manter a rotação apenas no plano

        if (direction.magnitude > 0.01f)
        {
            // Calcula a rotação alvo
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // Interpola a rotação suavemente
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    private void Attack()
    {
        if (target.GetComponent<Door>())
            OnAttackDoor?.Invoke();
        else
            OnAttack?.Invoke();
    }

    public void SetInsideShelter(bool _inside)
    {
        enemyInside = _inside;
    }

    public bool CheckDoor()
    {
        if (playerMovement.inside != enemyInside)
            return true;
        else
            return false;
    }

    public override void Die()
    {
        OnZDie?.Invoke(true);
        agent.enabled = false;
    }
}