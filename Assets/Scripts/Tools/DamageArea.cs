using System.Collections.Generic;
using UnityEngine;

public enum DamageType { Unique, Continuous }

public class DamageArea : MonoBehaviour
{
    [SerializeField] private DamageType damageType = DamageType.Unique;
    [Space]
    [SerializeField] private bool healing = false;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float delayBetweenDamage = 1f;

    private float currentRate = 0f;
    public List<BaseLivingBeing> livingBeings = new List<BaseLivingBeing>();

    private void Start()
    {
        currentRate = delayBetweenDamage;
    }

    private void FixedUpdate() 
    {
        if (damageType != DamageType.Continuous)
            return;

        if (Time.deltaTime == 0)
            return;

        if (livingBeings.Count == 0)
            return;

        if (currentRate < delayBetweenDamage)
        {
            currentRate += Time.deltaTime;
        }
        else
        {
            for (int i = 0; i < livingBeings.Count; i++)
            {
                if (!healing)
                    livingBeings[i].ApplyDamage(damage, transform);
                else
                    livingBeings[i].ApplyHealing(damage);
            }

            currentRate = 0f;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        BaseLivingBeing livingBeing = other.GetComponent<BaseLivingBeing>();

        if (livingBeings.Count == 0 && livingBeing)
            currentRate = delayBetweenDamage;

        if (livingBeing && !livingBeings.Contains(livingBeing))
        {
            livingBeings.Add(livingBeing);
            if (damageType == DamageType.Unique)
            {
                // Aplica o dano imediatamente
                if (!healing)
                    livingBeing.ApplyDamage(damage, transform);
                else
                    livingBeing.ApplyHealing(damage);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        BaseLivingBeing livingBeing = other.GetComponent<BaseLivingBeing>();
        if (livingBeing && livingBeings.Contains(livingBeing))
            livingBeings.Remove(livingBeing);
    }
}