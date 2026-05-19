using UnityEngine;

public class BleedingArea : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<ComplexLivingBeing>())
        {
            if (other.GetComponentInParent<Bleeding>())
                 other.GetComponentInParent<Bleeding>().enabled = true;
        }
    }
}
