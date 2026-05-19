using UnityEngine;

public class Spikes : BaseLivingBeing
{
    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<BaseLivingBeing>())
        {
            ApplyDamage(1f, other.transform);
            print("Dano de Spike");
        }
    }
}
