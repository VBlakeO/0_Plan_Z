using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bleeding : Debuff
{   
    [SerializeField] private float damage = 5f;
    [SerializeField] private float effectRate = 0.5f;
    [SerializeField] private ComplexLivingBeing livingBeing = null;

    private float currentRate = 0.5f;

    protected override void OnEnable()
    {
        base.OnEnable();
        if (!livingBeing)
            livingBeing = GetComponent<ComplexLivingBeing>();

        currentRate = effectRate;
    }

    void FixedUpdate()
    {
        if (currentRate < effectRate)
            currentRate += Time.deltaTime;
        else
            ApplyEffect();
    }

    protected override void ApplyEffect()
    {
        base.ApplyEffect();
        livingBeing.ApplyDamage(damage, null);
        currentRate = 0f;
    }
}
