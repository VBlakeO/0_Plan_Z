using UnityEngine;

public class BaseDoor : MonoBehaviour
{
    public bool isOpen = false;
    [SerializeField] protected bool isLocked = false;
    [Space]

    [SerializeField] protected AnimationCurve speed; 
    [Space]

    [SerializeField] protected GameObject door = null;

    protected float timeLine = 0f;

    protected virtual void SwithDoor()
    {
        //isOpen = !isOpen;
    }
    
    protected virtual void OpenDoor()
    {
        isOpen = true;
    }

    protected virtual void CloseDoor()
    {
        isOpen = false;
    }

    public virtual void DoorEffect()
    {
        door.transform.rotation = Quaternion.Euler (0, 90 * speed.Evaluate(timeLine), 0);
    }
}
