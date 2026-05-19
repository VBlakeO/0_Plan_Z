using System.Collections.Generic;
using UnityEngine;

public class SlowAreaEffect : Debuff
{
    public float intensity = 0.7f;
    public BaseMovementSystem movementSystem = null;
    private List<BaseAI> baseAIs;

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<BaseMovementSystem>())
        {
            movementSystem = other.GetComponent<BaseMovementSystem>();
            movementSystem.ApllySlow(intensity);
        }

        if (other.GetComponentInParent<BaseAI>())
        {
            BaseAI baseAI = other.GetComponentInParent<BaseAI>();

            // if (!baseAIs.Contains(baseAI))
            //     baseAIs.Add(baseAI);

            baseAI.ApllySlow(intensity);

            // for (int i = 0; i < baseAIs.Count; i++)
            // {
                // baseAIs[i].ApllySlow(intensity);
            // }
        }
    }

    protected override void ApplyEffect()
    {
        //base.ApplyEffect();
        //movementSystem.ApllySlow(intensity);
    }

    protected override void RemoveEffect()
    {
        if (movementSystem)
        {
            movementSystem.RemoveSlow();
            base.RemoveEffect();
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<BaseMovementSystem>())
        {
            movementSystem = other.GetComponent<BaseMovementSystem>();
            RemoveEffect();
        }

        if (other.GetComponentInParent<BaseAI>())
        {
            BaseAI baseAI = other.GetComponentInParent<BaseAI>();
            baseAI.RemoveSlow();
            
            // if (baseAIs.Contains(baseAI))
            // {
            //     //baseAIs.Remove(baseAI);
            // }

        }
    }
}
