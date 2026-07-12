using UnityEngine;
using PlanZ.Core;
using PlanZ.Events;
using PlanZ.Healing.Events;
using PlanZ.Interaction.Events;
using PlanZ.Placement.Events;
using PlanZ.Player.Events;
using PlanZ.Weapons.Events;

namespace PlanZ.Player.Input
{
    public class PlayerInput : SingletonMonoBehaviour<PlayerInput>
    {
        [Header("Movement Keys")]
        [SerializeField] private KeyCode jumpKey = KeyCode.Space;
        [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;
        [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
        [SerializeField] private KeyCode zoomKey = KeyCode.Mouse1;

        [Header("Weapon Keys")]
        [SerializeField] private KeyCode fireKey = KeyCode.Mouse0;
        [SerializeField] private KeyCode aimKey = KeyCode.Mouse1;
        [SerializeField] private KeyCode reloadKey = KeyCode.R;
        [SerializeField] private KeyCode[] weaponSlotKeys = { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3 };

        [Header("Interaction Keys")]
        [SerializeField] private KeyCode interactKey = KeyCode.E;

        [Header("Placement Keys")]
        [SerializeField] private KeyCode[] placementSlotKeys = { KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9 };
        [SerializeField] private KeyCode placementConfirmKey = KeyCode.Mouse0;
        [SerializeField] private KeyCode placementRotateKey = KeyCode.Mouse1;
        [SerializeField] private KeyCode placementCancelKey = KeyCode.Q;

        [Header("Heal Keys")]
        [SerializeField] private KeyCode healKey = KeyCode.F;

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
        public bool FireHeld { get; private set; }
        public bool AimHeld { get; private set; }
        public bool InteractHeld { get; private set; }

        // Mutually exclusive player modes. PlacementMode is set by PlacementController;
        // HealingMode by HealController. While either is true, weapon/interaction inputs are
        // suppressed at the source so downstream listeners never see them.
        public bool PlacementMode { get; set; }
        public bool HealingMode { get; set; }

        private bool ActionsSuppressed => PlacementMode || HealingMode;

        private void Update()
        {
            if (Time.timeScale == 0f) return;

            ReadMovement();
            ReadLook();
            ReadHeldButtons();
            ReadPressedButtons();
            ReadScrollWheel();
            ReadWeaponSlots();
            ReadPlacementSlots();
            ReadHealKey();
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
            FireHeld = !ActionsSuppressed && UnityEngine.Input.GetKey(fireKey);
            AimHeld = !ActionsSuppressed && UnityEngine.Input.GetKey(aimKey);
            InteractHeld = !ActionsSuppressed && UnityEngine.Input.GetKey(interactKey);
        }

        private void ReadPressedButtons()
        {
            if (UnityEngine.Input.GetKeyDown(jumpKey))
                EventBus.Publish(new PlayerJumpRequestedEvent());

            if (PlacementMode)
            {
                ReadPlacementActions();
                return;
            }

            if (HealingMode) return;

            if (UnityEngine.Input.GetKeyDown(fireKey))
                EventBus.Publish(new WeaponFirePressedEvent());

            if (UnityEngine.Input.GetKeyUp(fireKey))
                EventBus.Publish(new WeaponFireReleasedEvent());

            if (UnityEngine.Input.GetKeyDown(reloadKey))
                EventBus.Publish(new WeaponReloadPressedEvent());

            if (UnityEngine.Input.GetKeyDown(interactKey))
                EventBus.Publish(new InteractionPressedEvent());

            if (UnityEngine.Input.GetKeyUp(interactKey))
                EventBus.Publish(new InteractionReleasedEvent());
        }

        private void ReadPlacementActions()
        {
            if (UnityEngine.Input.GetKeyDown(placementConfirmKey))
                EventBus.Publish(new PlacementConfirmPressedEvent());

            if (UnityEngine.Input.GetKeyDown(placementRotateKey))
                EventBus.Publish(new PlacementRotatePressedEvent());

            if (UnityEngine.Input.GetKeyDown(placementCancelKey))
                EventBus.Publish(new PlacementCancelPressedEvent());
        }

        private void ReadScrollWheel()
        {
            float scroll = UnityEngine.Input.GetAxis(ScrollWheelAxisName);
            if (scroll == ScrollDeadZone) return;

            int direction = (int)Mathf.Sign(scroll);
            EventBus.Publish(new PlayerScrollInputEvent(direction));

            if (!ActionsSuppressed)
                EventBus.Publish(new WeaponCycleRequestedEvent(direction));
        }

        private void ReadWeaponSlots()
        {
            if (ActionsSuppressed) return;

            for (int i = 0; i < weaponSlotKeys.Length; i++)
            {
                if (UnityEngine.Input.GetKeyDown(weaponSlotKeys[i]))
                    EventBus.Publish(new WeaponSlotSelectedEvent(i));
            }
        }

        private void ReadPlacementSlots()
        {
            // Placement slots are usable to enter placement mode while not already in another
            // exclusive mode. Once in placement mode they keep working for slot swapping; once
            // in healing mode they're suppressed entirely.
            if (HealingMode) return;

            for (int i = 0; i < placementSlotKeys.Length; i++)
            {
                if (UnityEngine.Input.GetKeyDown(placementSlotKeys[i]))
                    EventBus.Publish(new PlacementSlotSelectedEvent(i));
            }
        }

        // The heal key always publishes its events; the HealController itself decides whether
        // to act on them. This keeps mode-coordination logic in the controller rather than
        // scattered across the input layer.
        private void ReadHealKey()
        {
            if (UnityEngine.Input.GetKeyDown(healKey))
                EventBus.Publish(new HealPressedEvent());

            if (UnityEngine.Input.GetKeyUp(healKey))
                EventBus.Publish(new HealReleasedEvent());
        }
    }
}