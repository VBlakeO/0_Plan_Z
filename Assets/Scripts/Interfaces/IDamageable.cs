using UnityEngine;

public interface IDamageable
{
    public void ApplyDamage(float damage, Transform damageSource);
    public void ApplyHealing(float damage);

    public void Die();

    public bool IsDead();
}
