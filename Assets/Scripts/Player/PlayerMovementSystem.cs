using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovementSystem : BaseMovementSystem
{
    [Header("Camera")]
    [SerializeField] private Camera playerCamera = null;
    [SerializeField] private bool invertCamera = false;
    [Range(30f, 170f)] public float initFOV = 90f;
    [SerializeField] private Vector2 maxLookAngle = Vector2.zero;

    [SerializeField] private float headPitch = 0f;
    [HideInInspector] public float y_Recoil = 0.0f, x_Recoil = 0.0f;


    [Header("Zoom")]
    [SerializeField] private KeyCode zoomKey = KeyCode.Mouse1;
    [SerializeField] private bool enableZoom = true;
    [SerializeField] private bool holdToZoom = false;
    [SerializeField] private float zoomFOV = 30f;
    [SerializeField] private float zoomStepTime = 5f;
    
    [Header("Gravity")]
    [SerializeField] private float gravity = -13.0f;
    [SerializeField] private float groundCheckRange = 0.75f;
    [SerializeField] private float crouchGroundCheckRange = 0.75f;


    [Header("Jump")]
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float jumpStaminaDrain = 15f;

    [Header("Crouch")]
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;
    [SerializeField] private float crouchHeight = .75f;
    [SerializeField] private float speedReduction = .5f;
    [Space]

    [Header("Initial Valueds")]
    [Range(0.1f, 10f)]private float mouseSensitivity = 2f;
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float moveSmoothTime = 0.30f;

    [Header("Components")]
    [SerializeField] private Animator anim = null;

    [Header("Ceiling Test")]
    [SerializeField] private float castRadius = 0.5f;
    [SerializeField] private float castDistance = 1.5f;
    [SerializeField] private LayerMask layerMask = 1;
    [Space]

    [SerializeField] private Transform spine1 = null;
    [SerializeField] private Transform spine2 = null;
    [SerializeField] private Transform spine3 = null;

    [Header("===PlayerMovement===")]
    [SerializeField] private bool autoWalk = false;
    [Space]
    [SerializeField] private bool cantMove = false;
    [SerializeField] private bool cantLook = false;
    [SerializeField] private bool cantJump = false;
    [SerializeField] private bool cantSprint = false;
    [SerializeField] private bool cantCrouch = false;

    //=============================================================

    // Movement Base
    private Vector2 targetDir = Vector2.zero;
    private CharacterController controller = null;
    private Vector2 currentDirVelocity = Vector2.zero;
    private Vector3 velocity = Vector3.zero;
    [HideInInspector] public Vector2 currentDir = Vector2.zero;

    // Camera Rotation
    [SerializeField] private float pitch = 0.0f;
    [SerializeField] private float yaw = 0.0f;

    // Zoom
    private bool isZoomed = false;

    // Movement Debug
    [HideInInspector] public float velocityY = 0f;
    public bool isJumping = false;
    [HideInInspector] public bool isWalking = false;
     public bool isGrounded = false;
    [HideInInspector] public bool isCrouched = false;
    [HideInInspector] public bool isSprinting = false;

    // GroudCheck
    private bool wasGrounded;
    private float originalGroundCheckRange = 0.75f;

    //Slop
    private float originalHeight  = 2f;
    private float slopeForce = 5.0f;
    private float slopeForceRayLength = 2.0f;

    //Test
    public bool inside = false;


    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        playerCamera.fieldOfView = initFOV;
        originalHeight = controller.height;
        originalGroundCheckRange = groundCheckRange;

        speed = walkSpeed;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (Time.timeScale == 0)
            return;

        targetDir = new Vector2(Input.GetAxisRaw("Horizontal"), autoWalk? 1f : Input.GetAxisRaw("Vertical"));

        MouseLook();
        ZoomControl();
        Jump();

        if (wasGrounded != CheckGround())
        {
            GroundedChanged(CheckGround());
            wasGrounded = CheckGround();
        }
    }

    private void FixedUpdate()
    {
        Movement();
        Crouch();
    }

    private void MouseLook()
    {
        if (cantLook)
            return;

        yaw = transform.localEulerAngles.y + (Input.GetAxis("Mouse X") + x_Recoil) * mouseSensitivity;

        if (!invertCamera)
            pitch -= mouseSensitivity * (Input.GetAxis("Mouse Y") + y_Recoil);
        else
            pitch += mouseSensitivity * (Input.GetAxis("Mouse Y") + y_Recoil);

        pitch = Mathf.Clamp(pitch, maxLookAngle.x, maxLookAngle.y);

        transform.localEulerAngles = new Vector3(0f, yaw, 0f);

        if (pitch > 8)
        {
            spine1.localRotation = Quaternion.Euler(0f, 0f, pitch);
            spine2.localRotation = Quaternion.Euler(0f, 0f, 7.9f);
        }
        else if (pitch <= 30 && pitch >= -20 )
        {
            spine1.localRotation = Quaternion.Euler(0f, 0f, 8f);
            spine2.localRotation = Quaternion.Euler(0f, 0f, pitch);
            spine3.localRotation = Quaternion.Euler(0f, 0f, -20);
        }
        else if (pitch < -20)
        { 
            spine3.localRotation = Quaternion.Euler(0f, 0f, pitch);
            spine2.localRotation = Quaternion.Euler(0f, 0f, -19.9f);
        }
    }

    private void Movement()
    {
        velocityY += gravity * Time.deltaTime;

        if (cantMove)
        {
            controller.Move(Vector3.up * velocityY * Time.deltaTime);
            return;
        }

        isWalking = targetDir.x != 0 || targetDir.y != 0 && CheckGround();

        targetDir.Normalize();
        currentDir = Vector2.SmoothDamp(currentDir, targetDir, ref currentDirVelocity, moveSmoothTime);

        if (CheckGround() && !isJumping)
            velocityY = 0;

        velocity = (transform.forward * currentDir.y + transform.right * currentDir.x) * (speed * slow) + Vector3.up * velocityY;

        controller.Move(velocity * Time.deltaTime);

        if ((Mathf.Abs(targetDir.x) > 0 || Mathf.Abs(targetDir.y) > 0) && OnSlope())
            controller.Move(Vector3.down * controller.height / 2 * slopeForce * Time.deltaTime);

        anim.SetFloat("SpeedY", currentDir.y);
        anim.SetFloat("SpeedX", currentDir.x);
    }

    private void ZoomControl()
    {
        if (!enableZoom)
            return;

        if (Input.GetKeyDown(zoomKey) && !holdToZoom && !isSprinting)
        {
            if (!isZoomed)
                isZoomed = true;
            else
                isZoomed = false;
        }

        if (holdToZoom && !isSprinting)
        {
            if (Input.GetKeyDown(zoomKey))
                isZoomed = true;
            else if (Input.GetKeyUp(zoomKey))
                isZoomed = false;
        }

        if (isZoomed)
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, zoomFOV, zoomStepTime * Time.deltaTime);
        else if (!isZoomed && !isSprinting)
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, initFOV, zoomStepTime * Time.deltaTime);
    }

    private void Jump()
    {
        if (Input.GetKeyDown(jumpKey) && CheckGround() && !cantJump && !isCrouched)
        {
            isJumping = true;
            velocityY = jumpForce;
            StartCoroutine(BackToGround());
        }
    }

    private void Crouch()
    {
        if (cantCrouch)
            return;

        if (Input.GetKey(crouchKey) && !isCrouched)
        {
            controller.height = crouchHeight;
            groundCheckRange = crouchGroundCheckRange;
            isCrouched = true;
        }

        if (isCrouched && !Input.GetKey(crouchKey))
        {
            if (!CheckCeilingHeight())
            {
                controller.height = originalHeight;
                groundCheckRange = originalGroundCheckRange;
                controller.Move(new Vector3(0.0f,-0.01f,0f));

                isCrouched = false;
            }
        }
    }

    public void LockPlayer(bool _lock)
    {
        cantMove = _lock;
        cantLook = _lock;
        cantJump = _lock;
        cantSprint = _lock;
        cantCrouch = _lock;
    }

    public void LockVision(bool _lock)
    {
        cantLook = _lock;
    }

    private bool OnSlope()
    {
        if (isJumping)
            return false;

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, controller.height / 2 * slopeForceRayLength))
            if (hit.normal != Vector3.up)
                return true;

        return false;
    }

    private bool CheckGround()
    {
        return Physics.SphereCast(transform.position, castRadius, -transform.up, out RaycastHit hit, groundCheckRange, layerMask, QueryTriggerInteraction.Ignore);
    }

    void GroundedChanged(bool state)
    {
        if (state)
            isGrounded = true;
        else
            isGrounded = false;
    }
    
    private bool CheckCeilingHeight()
    {
        return Physics.SphereCast(transform.position, castRadius, transform.up, out RaycastHit hit, castDistance, layerMask, QueryTriggerInteraction.Ignore);
    }

    private IEnumerator BackToGround()
    {
        WaitForSeconds wfs = new WaitForSeconds(0.3f);
        yield return wfs;
        isJumping = false;
    }

    private void OnDrawGizmos()
    {
        // Gizmos.color = Color.red;
        // Gizmos.DrawSphere(transform.position - transform.up * groundCheckRange, castRadius);
        // Gizmos.DrawSphere(transform.position + transform.up * castDistance, castRadius);
    }

    public void SetInsideShelter(bool _inside)
    {
        inside = _inside;
    }
}