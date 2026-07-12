using UnityEngine;
using PlanZ.Placement.Data;

namespace PlanZ.Placement.Components
{
    // Renders the ghost preview of the placeable being positioned. Configures the mesh, scale,
    // collider size, and swaps materials between valid/invalid based on the validator state.
    // Lives on a dedicated GameObject that is enabled only while placement mode is active.
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(BoxCollider))]
    [RequireComponent(typeof(PlacementValidator))]
    public class PlacementGhost : MonoBehaviour
    {
        [Header("Shared materials")]
        [SerializeField] private Material validMaterial;
        [SerializeField] private Material invalidMaterial;

        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;
        private BoxCollider _boxCollider;
        private PlacementValidator _validator;

        public PlacementValidator Validator => _validator;

        private void Awake()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _meshRenderer = GetComponent<MeshRenderer>();
            _boxCollider = GetComponent<BoxCollider>();
            _validator = GetComponent<PlacementValidator>();

            _boxCollider.isTrigger = true;
        }

        public void Configure(PlaceableData data)
        {
            _meshFilter.sharedMesh = data.GhostMesh;
            transform.localScale = data.GhostScale;
            _boxCollider.size = data.ColliderSize;
            _validator.ResetCounter();
        }

        // Called every frame by the controller after the validator has settled. Swapping the
        // material here (rather than from the validator) keeps the visual concern out of the
        // collision detection component.
        public void RefreshMaterial(bool canPlace)
        {
            _meshRenderer.sharedMaterial = canPlace ? validMaterial : invalidMaterial;
        }

        public void SetVisible(bool value) => gameObject.SetActive(value);
    }
}
