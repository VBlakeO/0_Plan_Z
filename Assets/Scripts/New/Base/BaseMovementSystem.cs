using UnityEngine;

public class BaseMovementSystem : MonoBehaviour
{
    public float speed = 3f; 
    public float slow = 1f; 

    public void ApllySlow(float intensity)
    {
        slow = intensity;
    }

    public void RemoveSlow()
    {
        slow = 1f;
    }
}
