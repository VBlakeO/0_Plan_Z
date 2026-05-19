 using UnityEngine;

public class SoundTrap : MonoBehaviour
{
    void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<Zombie_AI>())
            if (other.GetComponent<Zombie_AI>().GetTarget() != transform)
                other.GetComponent<Zombie_AI>().SetTarget(transform);
    }
}
