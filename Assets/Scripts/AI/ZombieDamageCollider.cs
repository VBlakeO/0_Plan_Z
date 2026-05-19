using UnityEngine;

public class ZombieDamageCollider : MonoBehaviour
{
    public float damage = 10f;

    void OnTriggerEnter(Collider other)
    {
        BaseLivingBeing livingBeing = other.GetComponentInParent<BaseLivingBeing>();

        if (!other.GetComponentInParent<Zombie_AI>() && livingBeing)
            livingBeing.ApplyDamage(damage, transform);
    }
}
