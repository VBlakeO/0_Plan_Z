using UnityEngine;

public class BaseLivingBeing : MonoBehaviour, IDamageable
{
    [Header("BaseLivingBeing")]
    public float maxLife = 100f;
    [SerializeField] protected float currentLife = 0f;
    
    public virtual void Awake() 
    {
        currentLife = maxLife;   
    }

    public virtual void ApplyDamage(float damage, Transform damageSource)
    {
        if(IsDead())
            return;

        print(transform.name + " recebeu dano de " + damageSource);

        currentLife -= damage;

        if (currentLife < 0)
            currentLife = 0;

        if (IsDead())
            Die();

    }
    
    public virtual void ApplyHealing(float healing)
    {
        currentLife += healing;

        if (currentLife > maxLife)
            currentLife = maxLife;
    }

    public float GetCurrentLife()
    {
        return currentLife;
    }

    public virtual void Die()
    {
        Destroy(gameObject);
        print (transform.name + " " + "morreu!");
    }

    public bool IsDead()
    {
        return currentLife <= 0f;
    }

    public bool IsInjured()
    {
        return currentLife > 0 && currentLife < maxLife;
    }
}
