using UnityEngine;

public class Debuff : MonoBehaviour
{
    public bool temporary = true;
    public float duration = 3f;

    protected virtual void OnEnable() 
    {
        if (temporary)
            Invoke(nameof(Disable), duration);
    }

    protected virtual void ApplyEffect() 
    {

    }

    protected virtual void RemoveEffect() 
    {
        enabled = false;
    }

    public void Disable()
    {
        RemoveEffect();
    }
}
