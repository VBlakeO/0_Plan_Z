using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{   
    [SerializeField] private PlayerMovementSystem playerMovement = null;
    [SerializeField] private Animator anim = null;
    [SerializeField] private Animator armsAnim = null;
    [SerializeField] private float distanceToGround = 1;
    [SerializeField] private float range = 1f;
    [SerializeField] private LayerMask layerMask = 1;


    private void Start() 
    {
     
    }

    void Update()
    {
    }
}
