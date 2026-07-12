using UnityEngine;

namespace PlanZ.Player.Config
{
    [CreateAssetMenu(fileName = "MovementConfig", menuName = "PlanZ/Player/Movement Config")]
    public class MovementConfig : ScriptableObject
    {
        [Header("Locomotion")]
        [SerializeField] private float walkSpeed = 3f;
        [SerializeField] private float sprintSpeed = 6f;
        [SerializeField, Range(0.1f, 1f)] private float crouchSpeedMultiplier = 0.5f;
        [SerializeField] private float moveSmoothTime = 0.3f;

        [Header("Gravity")]
        [SerializeField] private float gravity = -13f;
        [SerializeField] private float groundCheckRange = 0.75f;
        [SerializeField] private float crouchGroundCheckRange = 0.75f;
        [SerializeField] private float groundCheckRadius = 0.5f;
        [SerializeField] private LayerMask groundLayers = 1;

        [Header("Slope")]
        [SerializeField] private float slopeForce = 5f;
        [SerializeField] private float slopeRayLength = 2f;

        [Header("Jump")]
        [SerializeField] private float jumpForce = 7f;
        [SerializeField] private float jumpCooldown = 0.3f;
        [SerializeField] private float jumpAnticipationDelay = 0.15f;
        [SerializeField, Range(0f, 1f)] private float airControlMultiplier = 0.3f;
        [SerializeField] private float coyoteTime = 0.15f;

        [Header("Sprint")]
        [SerializeField] private bool cancelSprintWhenZoomed = true;

        [Header("Crouch")]
        [SerializeField] private float crouchHeight = 0.75f;
        [SerializeField] private float ceilingCheckRadius = 0.5f;
        [SerializeField] private float ceilingCheckDistance = 1.5f;
        [SerializeField] private float crouchTransitionSpeed = 4f;

        [Header("Camera")]
        [SerializeField, Range(0.1f, 10f)] private float mouseSensitivity = 2f;
        [SerializeField] private bool invertCamera;
        [SerializeField] private Vector2 pitchLimits = new(-60f, 60f);
        [SerializeField, Range(30f, 170f)] private float initialFOV = 90f;
        [SerializeField, Range(30f, 170f)] private float zoomFOV = 30f;
        [SerializeField, Range(30f, 170f)] private float sprintFOV = 100f;
        [SerializeField] private float fovLerpSpeed = 5f;

        [Header("Spine IK - Enable")]
        [SerializeField] private bool spineIKEnabled = true;

        [Header("Spine IK - Axis (which local axis bends the bone forward/back)")]
        [SerializeField] private SpineBoneAxis spineUpperAxis = SpineBoneAxis.X;
        [SerializeField] private SpineBoneAxis spineMiddleAxis = SpineBoneAxis.X;
        [SerializeField] private SpineBoneAxis spineLowerAxis = SpineBoneAxis.X;

        [Header("Spine IK - Sign (-1 inverts if the bone bends the wrong way)")]
        [SerializeField, Range(-1f, 1f)] private float spineUpperSign = 1f;
        [SerializeField, Range(-1f, 1f)] private float spineMiddleSign = 1f;
        [SerializeField, Range(-1f, 1f)] private float spineLowerSign = 1f;

        [Header("Spine IK - Weights (should sum to ~1.0)")]
        [SerializeField, Range(0f, 1f)] private float spineUpperWeight = 0.2f;
        [SerializeField, Range(0f, 1f)] private float spineMiddleWeight = 0.4f;
        [SerializeField, Range(0f, 1f)] private float spineLowerWeight = 0.4f;

        [Header("Spine IK - Angle Limits per bone (x = backward limit, y = forward limit)")]
        [SerializeField] private Vector2 spineUpperAngleLimits = new(-10f, 30f);
        [SerializeField] private Vector2 spineMiddleAngleLimits = new(-10f, 30f);
        [SerializeField] private Vector2 spineLowerAngleLimits = new(-5f, 20f);

        [Header("Animation - Locomotion Speed Multipliers")]
        [SerializeField, Range(0.1f, 3f)] private float walkAnimationSpeed = 1f;
        [SerializeField, Range(0.1f, 3f)] private float sprintAnimationSpeed = 1.5f;
        [SerializeField, Range(0.1f, 3f)] private float crouchAnimationSpeed = 0.7f;
        [SerializeField] private float animationSpeedSmoothing = 8f;

        public float WalkSpeed => walkSpeed;
        public float SprintSpeed => sprintSpeed;
        public float CrouchSpeedMultiplier => crouchSpeedMultiplier;
        public float MoveSmoothTime => moveSmoothTime;

        public float Gravity => gravity;
        public float GroundCheckRange => groundCheckRange;
        public float CrouchGroundCheckRange => crouchGroundCheckRange;
        public float GroundCheckRadius => groundCheckRadius;
        public LayerMask GroundLayers => groundLayers;

        public float SlopeForce => slopeForce;
        public float SlopeRayLength => slopeRayLength;

        public float JumpForce => jumpForce;
        public float JumpCooldown => jumpCooldown;
        public float JumpAnticipationDelay => jumpAnticipationDelay;
        public float AirControlMultiplier => airControlMultiplier;
        public float CoyoteTime => coyoteTime;

        public bool CancelSprintWhenZoomed => cancelSprintWhenZoomed;

        public float CrouchHeight => crouchHeight;
        public float CeilingCheckRadius => ceilingCheckRadius;
        public float CeilingCheckDistance => ceilingCheckDistance;
        public float CrouchTransitionSpeed => crouchTransitionSpeed;

        public float MouseSensitivity => mouseSensitivity;
        public bool InvertCamera => invertCamera;
        public Vector2 PitchLimits => pitchLimits;
        public float InitialFOV => initialFOV;
        public float ZoomFOV => zoomFOV;
        public float SprintFOV => sprintFOV;
        public float FOVLerpSpeed => fovLerpSpeed;

        public bool SpineIKEnabled => spineIKEnabled;

        public SpineBoneAxis SpineUpperAxis => spineUpperAxis;
        public SpineBoneAxis SpineMiddleAxis => spineMiddleAxis;
        public SpineBoneAxis SpineLowerAxis => spineLowerAxis;

        public float SpineUpperSign => spineUpperSign;
        public float SpineMiddleSign => spineMiddleSign;
        public float SpineLowerSign => spineLowerSign;

        public float SpineUpperWeight => spineUpperWeight;
        public float SpineMiddleWeight => spineMiddleWeight;
        public float SpineLowerWeight => spineLowerWeight;

        public Vector2 SpineUpperAngleLimits => spineUpperAngleLimits;
        public Vector2 SpineMiddleAngleLimits => spineMiddleAngleLimits;
        public Vector2 SpineLowerAngleLimits => spineLowerAngleLimits;

        public float WalkAnimationSpeed => walkAnimationSpeed;
        public float SprintAnimationSpeed => sprintAnimationSpeed;
        public float CrouchAnimationSpeed => crouchAnimationSpeed;
        public float AnimationSpeedSmoothing => animationSpeedSmoothing;
    }
    
    public enum SpineBoneAxis
    {
        X,
        Y,
        Z
    }
}