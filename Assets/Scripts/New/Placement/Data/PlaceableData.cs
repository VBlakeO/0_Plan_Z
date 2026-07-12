using UnityEngine;

namespace PlanZ.Placement.Data
{
    [CreateAssetMenu(fileName = "PlaceableData", menuName = "PlanZ/Placement/Placeable Data")]
    public class PlaceableData : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] private string displayName = "Placeable";

        [Header("Prefab")]
        [SerializeField] private GameObject placedPrefab;

        [Header("Ghost preview")]
        [SerializeField] private Mesh ghostMesh;
        [SerializeField] private Vector3 ghostScale = Vector3.one;
        [SerializeField] private Vector3 colliderSize = Vector3.one;

        [Header("Positioning")]
        [SerializeField] private float heightOffset = 1f;
        [SerializeField] private LayerMask surfaceLayers = 1;
        [SerializeField] private float maxRange = 8f;

        [Header("Rotation")]
        [SerializeField] private float snapAngle = 45f;

        public string DisplayName => displayName;
        public GameObject PlacedPrefab => placedPrefab;
        public Mesh GhostMesh => ghostMesh;
        public Vector3 GhostScale => ghostScale;
        public Vector3 ColliderSize => colliderSize;
        public float HeightOffset => heightOffset;
        public LayerMask SurfaceLayers => surfaceLayers;
        public float MaxRange => maxRange;
        public float SnapAngle => snapAngle;
    }
}
