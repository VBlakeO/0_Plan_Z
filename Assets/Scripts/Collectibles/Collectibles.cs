using UnityEngine;

public class Collectibles : MonoBehaviour
{
    [SerializeField] protected bool infinity = false;
    [Space]

    [SerializeField] protected int amount = 10;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.GetComponent<PlayerInventory>())
            return;

        Interact(other.GetComponent<PlayerInventory>());
    }

    protected virtual void Interact(PlayerInventory other)
    {

    }

    protected void Disable()
    {
        gameObject.SetActive(false);
    }
}