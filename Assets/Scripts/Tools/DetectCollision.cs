using UnityEngine;

public class DetectCollision : MonoBehaviour
{
    public bool colliding = false;
    public Transform obj = null; 

    public bool Colliding()
    {
        return colliding;
    }

    void OnTriggerStay(Collider other)
    {
        colliding = true;
        obj = other.transform;
    }

    void OnTriggerExit(Collider other)
    {
        Exit();
    }

    public void Exit()
    {
        colliding = false;
        obj = null;
    }
}
