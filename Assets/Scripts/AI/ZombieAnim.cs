using UnityEngine;

public class ZombieAnim : MonoBehaviour
{
    [SerializeField] private Zombie_AI zombieAI = null;
    [Space]

    [SerializeField] private Animator anim = null;
    [SerializeField] private SphereCollider baseCollider = null;
    [SerializeField] private Rigidbody[] rbs = null;
    [SerializeField] private int animAttack = 0;

    private void OnEnable()
    {
        rbs = GetComponentsInChildren<Rigidbody>();
        zombieAI.OnFixedUpdate += ZFixedUpdate;
        zombieAI.OnChangeState += ChangeState;
        zombieAI.OnAttackDoor += AttackDoor;
        zombieAI.OnAttack += Attack;
        zombieAI.OnZDie += SetRagDoll;

        animAttack = Random.Range(0, 2);

        SetRagDoll(false);
    }

    private void SetRagDoll(bool state)
    {
        baseCollider.enabled = !state;

        for (int i = 0; i < rbs.Length; i++)
        {
            rbs[i].isKinematic = !state;
        }

        anim.enabled = !state;
    }

    private void ZFixedUpdate(float _pathDistance)
    {
        anim.SetFloat("Distance", _pathDistance);
    }

    private void Attack()
    {   
        anim.ResetTrigger("Walk");

        if (animAttack == 0)
            anim.SetTrigger("Attack");
        else
            anim.SetTrigger("Attack2");
    }

    private void AttackDoor()
    {
        anim.ResetTrigger("Walk");
        anim.SetTrigger("AttackDoor");
    }

    private void ChangeState(State _state)
    {
        if (_state != State.Attack)
        {
            if (animAttack == 0)
                anim.ResetTrigger("Attack");
            else
                anim.ResetTrigger("Attack2");

            anim.SetTrigger("Walk");
            anim.ResetTrigger("AttackDoor");
        }
    }
}