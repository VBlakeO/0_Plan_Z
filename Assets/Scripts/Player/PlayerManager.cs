using UnityEngine.Events;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [HideInInspector] public PlayerMovementSystem movement = null;
    [HideInInspector] public BaseLivingBeing livingBeing = null;
    [HideInInspector] public PlayerInventory inventory = null;
    [SerializeField]  private HudManager hudManager = null;
    [Space]

    [SerializeField] private bool isHealing = false;

    public UnityAction OnHealing = null;

    private void Awake() 
    {
        movement = GetComponent<PlayerMovementSystem>();
        livingBeing = GetComponent<BaseLivingBeing>();   
        inventory = GetComponent<PlayerInventory>();
    }

    // Update is called once per frame
    void Update()
    {
       if (livingBeing.IsDead())
            return;

        if (Input.GetKeyDown(KeyCode.F))
            Healing();
    }

//////////////////////////////////////////////////////////////////Healing/////////////////////////////////////////////////////////////////////////////

    public void Healing()
    {
        if (inventory.currentFirstAidAmount > 0 && livingBeing.IsInjured() && !isHealing)
        {
            OnHealing?.Invoke();
            isHealing = true;
        }
    }

    public void EndHealing()
    {
        livingBeing.ApplyHealing(45f);
        inventory.UseFirstAid();
        isHealing = false;
    }

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}
