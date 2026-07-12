using UnityEngine;

namespace PlanZ.Weapons.Data
{
    [CreateAssetMenu(fileName = "WeaponAudioData", menuName = "PlanZ/Weapons/Weapon Audio Data")]
    public class WeaponAudioData : ScriptableObject
    {
        [System.Serializable]
        public struct Clip
        {
            public AudioClip audioClip;
            [Range(0f, 1f)] public float volume;
        }

        [Header("Equip")]
        [SerializeField] private Clip equip;

        [Header("Shoot")]
        [SerializeField] private Clip shoot;
        [SerializeField] private Vector2 shootPitchRange = new(0.95f, 1.05f);

        [Header("Reload")]
        [SerializeField] private Clip reloadStart;
        [SerializeField] private Clip reloadMiddle;
        [SerializeField] private Clip reloadEnd;

        [Header("Empty")]
        [SerializeField] private Clip emptyMag;

        public Clip Equip => equip;
        public Clip Shoot => shoot;
        public Vector2 ShootPitchRange => shootPitchRange;
        public Clip ReloadStart => reloadStart;
        public Clip ReloadMiddle => reloadMiddle;
        public Clip ReloadEnd => reloadEnd;
        public Clip EmptyMag => emptyMag;
    }
}
