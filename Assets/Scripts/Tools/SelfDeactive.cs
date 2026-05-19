using UnityEngine;

public class SelfDeactive : MonoBehaviour
{
    [SerializeField] private float disableTime = 0.3f;
    public Transform origin = null;

    private void OnEnable() 
    {
        Invoke("Disable", disableTime);
    }

    private void Disable()
    {
        if (origin)
            transform.parent = origin;
        gameObject.SetActive(false);
    }
}
