using UnityEngine;
using PlanZ.Core;
using PlanZ.Events;
using PlanZ.Player.Events;

namespace PlanZ.Player.Input
{
    // Single source of truth for raw input. Publishes domain events to the EventBus and exposes
    // immediate state (button-held flags, smoothed look delta) for systems that need it in FixedUpdate.
    public class PlayerInput : SingletonMonoBehaviour<PlayerInput>
    {
        [Header("Keys")]
        [SerializeField] private KeyCode jumpKey = KeyCode.Space;
        [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;
        [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
        [SerializeField] private KeyCode zoomKey = KeyCode.Mouse1;

        [Header("Options")]
        [SerializeField] private bool autoWalk;

        private const string HorizontalAxisName = "Horizontal";
        private const string VerticalAxisName = "Vertical";
        private const string MouseXAxisName = "Mouse X";
        private const string MouseYAxisName = "Mouse Y";
        private const string ScrollWheelAxisName = "Mouse ScrollWheel";

        private const float AutoWalkVerticalValue = 1f;
        private const float ScrollDeadZone = 0f;

        public Vector2 MoveAxis { get; private set; }
        public Vector2 LookDelta { get; private set; }
        public bool CrouchHeld { get; private set; }
        public bool SprintHeld { get; private set; }
        public bool ZoomHeld { get; private set; }

        private void Update()
        {
            if (Time.timeScale == 0f) return;

            ReadMovement();
            ReadLook();
            ReadHeldButtons();
            ReadPressedButtons();
            ReadScrollWheel();
        }

        private void ReadMovement()
        {
            float horizontal = UnityEngine.Input.GetAxisRaw(HorizontalAxisName);
            float vertical = autoWalk ? AutoWalkVerticalValue : UnityEngine.Input.GetAxisRaw(VerticalAxisName);

            MoveAxis = new Vector2(horizontal, vertical);
            EventBus.Publish(new PlayerMoveInputEvent(MoveAxis));
        }

        private void ReadLook()
        {
            float mouseX = UnityEngine.Input.GetAxis(MouseXAxisName);
            float mouseY = UnityEngine.Input.GetAxis(MouseYAxisName);

            LookDelta = new Vector2(mouseX, mouseY);
            EventBus.Publish(new PlayerLookInputEvent(LookDelta));
        }

        private void ReadHeldButtons()
        {
            CrouchHeld = UnityEngine.Input.GetKey(crouchKey);
            SprintHeld = UnityEngine.Input.GetKey(sprintKey);
            ZoomHeld = UnityEngine.Input.GetKey(zoomKey);
        }

        private void ReadPressedButtons()
        {
            if (UnityEngine.Input.GetKeyDown(jumpKey))
                EventBus.Publish(new PlayerJumpRequestedEvent());
        }

        private void ReadScrollWheel()
        {
            float scroll = UnityEngine.Input.GetAxis(ScrollWheelAxisName);
            if (scroll == ScrollDeadZone) return;

            int direction = (int)Mathf.Sign(scroll);
            EventBus.Publish(new PlayerScrollInputEvent(direction));
        }
    }
}
