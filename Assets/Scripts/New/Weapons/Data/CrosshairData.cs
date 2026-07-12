using UnityEngine;

namespace PlanZ.Weapons.Data
{
    [CreateAssetMenu(fileName = "CrosshairData", menuName = "PlanZ/Weapons/Crosshair Data")]
    public class CrosshairData : ScriptableObject
    {
        [SerializeField] private Sprite sprite;
        [SerializeField] private float size = 32f;
        [SerializeField] private Color color = Color.white;

        public Sprite Sprite => sprite;
        public float Size => size;
        public Color Color => color;
    }
}
