using UnityEngine;
using PlanZ.Events;
using PlanZ.Placement.Data;
using PlanZ.Placement.Events;
using PlanZ.Player.Input;

namespace PlanZ.Placement.Components
{
    // Top-level orchestrator for placement mode. Receives input events, drives the ghost preview,
    // delegates collision checks to the validator, consumes from the inventory on confirm, and
    // toggles the PlacementMode flag on PlayerInput so weapon/interaction inputs are suppressed.
    public class PlacementController : MonoBehaviour
    {
        [SerializeField] private Camera aimCamera;
        [SerializeField] private PlaceableInventory inventory;
        [SerializeField] private PlacementGhost ghost;

        private PlaceableData _activeData;
        private float _currentYaw;

        public bool IsActive => _activeData != null;
        public PlaceableData ActiveData => _activeData;

        private void OnEnable()
        {
            EventBus.Subscribe<PlacementSlotSelectedEvent>(HandleSlotSelected);
            EventBus.Subscribe<PlacementConfirmPressedEvent>(HandleConfirm);
            EventBus.Subscribe<PlacementRotatePressedEvent>(HandleRotate);
            EventBus.Subscribe<PlacementCancelPressedEvent>(HandleCancel);
            EventBus.Subscribe<PlaceableInventoryChangedEvent>(HandleInventoryChanged);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<PlacementSlotSelectedEvent>(HandleSlotSelected);
            EventBus.Unsubscribe<PlacementConfirmPressedEvent>(HandleConfirm);
            EventBus.Unsubscribe<PlacementRotatePressedEvent>(HandleRotate);
            EventBus.Unsubscribe<PlacementCancelPressedEvent>(HandleCancel);
            EventBus.Unsubscribe<PlaceableInventoryChangedEvent>(HandleInventoryChanged);
        }

        private void Start() => ghost.SetVisible(false);

        private void Update()
        {
            if (!IsActive) return;

            UpdateGhostPosition();
            ghost.RefreshMaterial(CanPlaceNow());
        }

        // Selecting the same slot twice cancels placement (toggle behaviour). Selecting a
        // different slot while in placement mode swaps the active placeable seamlessly.
        private void HandleSlotSelected(PlacementSlotSelectedEvent evt)
        {
            PlaceableData data = inventory.GetSlotData(evt.SlotIndex);
            if (data == null) return;
            if (!inventory.HasAny(data)) return;

            if (_activeData == data)
            {
                ExitPlacement();
                return;
            }

            EnterPlacement(data);
        }

        private void HandleConfirm()
        {
            if (!IsActive) return;
            if (!CanPlaceNow()) return;
            if (!inventory.TryConsume(_activeData)) return;

            PlaceObject();

            // If the inventory ran out, leave placement mode. Otherwise keep the ghost active
            // so the player can place multiple items in succession without re-pressing the slot.
            if (!inventory.HasAny(_activeData))
                ExitPlacement();
        }

        private void HandleRotate()
        {
            if (!IsActive) return;
            _currentYaw = Mathf.Repeat(_currentYaw + _activeData.SnapAngle, 360f);
        }

        private void HandleCancel()
        {
            if (!IsActive) return;
            ExitPlacement();
        }

        // If the player runs out of the active placeable through some external means (used by
        // another script, dropped, etc.), exit placement automatically. Keeps state consistent.
        private void HandleInventoryChanged(PlaceableInventoryChangedEvent evt)
        {
            if (_activeData != evt.Data) return;
            if (evt.Amount > -1) return;
            ExitPlacement();
        }

        private void EnterPlacement(PlaceableData data)
        {
            _activeData = data;
            _currentYaw = 0f;
            ghost.Configure(data);
            ghost.SetVisible(true);
            PlayerInput.Instance.PlacementMode = true;
            EventBus.Publish(new PlacementEnteredEvent(data));
        }

        private void ExitPlacement()
        {
            PlaceableData previous = _activeData;
            _activeData = null;
            ghost.SetVisible(false);
            PlayerInput.Instance.PlacementMode = false;
            EventBus.Publish(new PlacementExitedEvent(previous));
        }

        private void UpdateGhostPosition()
        {
            if (aimCamera == null) return;

            bool hit = Physics.Raycast(aimCamera.transform.position, aimCamera.transform.forward,
                out RaycastHit hitInfo, _activeData.MaxRange, _activeData.SurfaceLayers,
                QueryTriggerInteraction.Ignore);

            ghost.SetVisible(hit);
            if (!hit) return;

            ghost.transform.position = hitInfo.point + Vector3.up * _activeData.HeightOffset;
            ghost.transform.rotation = Quaternion.Euler(0f, _currentYaw, 0f);
        }

        // The ghost only needs to be visible AND non-overlapping AND have stock left. The
        // visibility check guards against the case where the raycast misses (ghost hidden):
        // confirming with no surface in front would silently consume an item.
        private bool CanPlaceNow()
        {
            if (!ghost.gameObject.activeSelf) return false;
            if (!ghost.Validator.IsValid) return false;
            if (!inventory.HasAny(_activeData)) return false;
            return true;
        }

        private void PlaceObject()
        {
            Vector3 position = ghost.transform.position;
            Quaternion rotation = ghost.transform.rotation;

            if(_activeData != null)
                Instantiate(_activeData.PlacedPrefab, position, rotation);
            EventBus.Publish(new PlacementPlacedEvent(_activeData, position, rotation));
            //_activeData = null;
        }
    }
}
