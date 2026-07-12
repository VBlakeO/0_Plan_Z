using UnityEngine;

namespace PlanZ.Weapons.Data
{
    [CreateAssetMenu(fileName = "WeaponData", menuName = "PlanZ/Weapons/Weapon Data")]
    public class WeaponData : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] private string displayName = "Unnamed";
        [SerializeField] private Sprite icon;

        [Header("Ballistics")]
        [SerializeField] private float damage = 30f;
        [SerializeField] private float range = 100f;
        [SerializeField] private LayerMask hitLayers = ~0;

        [Header("Fire")]
        [SerializeField] private ShotType shotType = ShotType.Manual;
        [SerializeField] private float fireRate = 0.15f;

        [Header("Ammo")]
        [SerializeField] private AmmoType ammoType = AmmoType.Pistol;
        [SerializeField] private int magazineCapacity = 15;
        [SerializeField] private float reloadDuration = 1.5f;
        [SerializeField] private bool autoReloadOnEmpty = true;

        [Header("Recoil")]
        [SerializeField] private RecoilPattern recoilPattern;
        [SerializeField] private float recoilIntensity = 1f;
        [SerializeField] private float recoilRiseSpeed = 18f;
        [SerializeField] private float recoilDecaySpeed = 6f;
        [SerializeField] private float recoilResetDelay = 0.2f;
        [SerializeField, Range(0f, 1f)] private float aimedRecoilMultiplier = 0.5f;

        [Header("Aim")]
        [SerializeField] private float aimedFOV = 55f;
        [SerializeField] private float hipFOV = 60f;
        [SerializeField] private float aimLerpSpeed = 2f;

        [Header("Equip")]
        [SerializeField] private float equipDuration = 0.3f;

        [Header("Visuals")]
        [SerializeField] private CrosshairData defaultCrosshair;
        [SerializeField] private CrosshairData aimingCrosshair;

        [Header("Audio")]
        [SerializeField] private WeaponAudioData audioData;

        [Header("Pool Identifiers")]
        [SerializeField] private string decalPoolId = "Decal";
        [SerializeField] private string tracerPoolId = "Tracer";

        public string DisplayName => displayName;
        public Sprite Icon => icon;

        public float Damage => damage;
        public float Range => range;
        public LayerMask HitLayers => hitLayers;

        public ShotType ShotType => shotType;
        public float FireRate => fireRate;

        public AmmoType AmmoType => ammoType;
        public int MagazineCapacity => magazineCapacity;
        public float ReloadDuration => reloadDuration;
        public bool AutoReloadOnEmpty => autoReloadOnEmpty;

        public RecoilPattern RecoilPattern => recoilPattern;
        public float RecoilIntensity => recoilIntensity;
        public float RecoilRiseSpeed => recoilRiseSpeed;
        public float RecoilDecaySpeed => recoilDecaySpeed;
        public float RecoilResetDelay => recoilResetDelay;
        public float AimedRecoilMultiplier => aimedRecoilMultiplier;

        public float AimedFOV => aimedFOV;
        public float HipFOV => hipFOV;
        public float AimLerpSpeed => aimLerpSpeed;

        public float EquipDuration => equipDuration;

        public CrosshairData DefaultCrosshair => defaultCrosshair;
        public CrosshairData AimingCrosshair => aimingCrosshair;

        public WeaponAudioData AudioData => audioData;

        public string DecalPoolId => decalPoolId;
        public string TracerPoolId => tracerPoolId;
    }

    public enum AmmoType
    {
        Pistol,
        Rifle,
        Shotgun
    }

    public enum ShotType 
    { 
        Manual, 
        Automatic 
    }
}

