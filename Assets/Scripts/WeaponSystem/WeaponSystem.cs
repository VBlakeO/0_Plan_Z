using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public enum ShotType { Manual, Automatic }
public enum AmmoType { Pistol, Rifle, Shotgun }

public class WeaponSystem : MonoBehaviour
{
    [Header("Crosshair")]
    [SerializeField] private CrosshairData defaultCrosshair;
    [SerializeField] private CrosshairData targetCrosshair;
    
    [SerializeField] private Sprite weaponIcon = null;


    [Header("Weapon Setting")]
    [SerializeField] private float range = 0f;
    [SerializeField] private float damage = 30f;
    [SerializeField] private float delayBetweenShots = 0f;
    [SerializeField] private float delayToEquip = 0f;
    [SerializeField] private LayerMask attainableLayers = 1;
    [Space]
    [SerializeField] private float recoilY = 0.3f;
    [SerializeField] private Vector2 recoilMinMax = new (-0.04f, 0.04f);
    [Space]

    [SerializeField] private ShotType shotType = ShotType.Manual;
    [SerializeField] private AmmoType ammoType = AmmoType.Pistol;

    [Header("Reload")]
    [SerializeField] private float reloadTime = 0f;
    [SerializeField] private bool automaticReload = true;

    [Header("Aim")]
    [SerializeField] private float aimFOV = 55f;
    [SerializeField] private float baseFOV = 60f;
    [SerializeField] private Transform aimBulletPoint = null;
    [SerializeField] private Transform baseBulletPoint = null;
    private bool aiming = false;

    [Header("Ammo Parameters")]
    [SerializeField] private int maxAmmoInMag = 15;
    [SerializeField] private int maxAmmo = 150;
    [SerializeField] private bool infinityAmmo = false;

    [Header("VFX")]
    [SerializeField] private Camera mainCamera = null;
    [SerializeField] private Camera weaponCamera = null;
    [SerializeField] private Transform bulletPoint = null;
    [SerializeField] private Animator weaponAnimator = null;
    [SerializeField] private ParticleSystem muzzleEffect = null;
    [SerializeField] private TrailRenderer tracerEffect = null;
    [SerializeField] private Transform poitT = null;
    [Space]

    [Header("Info")]
    private int currentAmmo = 0;
    private int shotCounter = 0;
    private float currentRate = 0f;
    public bool lockWeapon = false;
    [SerializeField] private bool equipped = false;
    [HideInInspector] public bool canEquip = true;
    [HideInInspector] public bool isReloading = false;
    [Space]

    [SerializeField] private float healingDelay = 1f;
    [Space]

    [SerializeField] private HudManager hudManager = null;
    [SerializeField] private PoolingManager poolingManager = null;
    private PlayerManager playerManager = null;

    public UnityAction OnShoot = null;

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    [Header("Audio")]
    [SerializeField] private WeaponAudioSystem weaponAudioSystem = null;
    [Space]
    [SerializeField] private WeaponAudio weaponAudio = null;

    private bool CanShoot()
    {
        // Equipado && Tem munição && Não está recaregando && Tempo certo de atirar
        return equipped && currentAmmo > 0 && !isReloading && currentRate >= delayBetweenShots && !lockWeapon;
    }

    private void Awake()
    {
        currentRate = delayBetweenShots;
        currentAmmo = maxAmmoInMag;

        OnShoot += HandleHud;

        playerManager = GetComponentInParent<PlayerManager>();
    }

    private void Start()
    {
        playerManager.inventory.OnUpdateAmmo += HandleHud;
        playerManager.OnHealing += Healing;
    }

    private void OnEnable()
    {
        Equip();
        SetAudioSystem();
    }

    public void Equip()
    {
        lockWeapon = false;
        Invoke("EquipDelay", delayToEquip);
        weaponAnimator.Play("DrawWeapon", 0, 0);

        currentRate = delayBetweenShots;

        if (hudManager)
        {
            hudManager.SetCrosshairState(true);
            hudManager.SetCrosshairStyle(defaultCrosshair);
            hudManager.SetWeaponIcon(weaponIcon);
        }

        HandleHud();
    }

    private void EquipDelay()
    {  
        lockWeapon = false;
        equipped = true;
    }


    private void SetAudioSystem()
    {
        weaponAudioSystem.getWeaponSfx = weaponAudio.getWeaponSfx;
        weaponAudioSystem.getWeaponVolume = weaponAudio.getWeaponVolume;

        weaponAudioSystem.shootSfx = weaponAudio.shootSfx;
        weaponAudioSystem.shootVolume = weaponAudio.shootVolume;

        weaponAudioSystem.startReloadSfx = weaponAudio.startReloadSfx;
        weaponAudioSystem.startReloadVolume = weaponAudio.startReloadVolume;

        weaponAudioSystem.midlleReloadSfx = weaponAudio.midlleReloadSfx;
        weaponAudioSystem.midlleReloadVolume = weaponAudio.midlleReloadVolume;

        weaponAudioSystem.endReloadSfx = weaponAudio.endReloadSfx;
        weaponAudioSystem.endReloadVolume = weaponAudio.endReloadVolume;

        weaponAudioSystem.noAmmoSfx = weaponAudio.noAmmoSfx;
        weaponAudioSystem.noAmmoVolume = weaponAudio.noAmmoVolume;
    }

    public void Unequip()
    {
        weaponAnimator.Play("UnequipWeapon", 0, 0);
        equipped = false;
    }

    private void HandleHud()
    {
        if (hudManager)
            hudManager.UpdateAmmo(currentAmmo, playerManager.inventory.GetAmmo(ammoType));
    }

    private void Aim()
    {
        if (aiming)
        {
            if (weaponCamera.fieldOfView > aimFOV + 0.5f)
                weaponCamera.fieldOfView = Mathf.Lerp(weaponCamera.fieldOfView, aimFOV, Time.deltaTime * 2f);

            if (mainCamera.fieldOfView > aimFOV + 0.5f)
                mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, aimFOV, Time.deltaTime * 2f);

            bulletPoint.transform.position = aimBulletPoint.position;
        }
        else
        {
            if (weaponCamera.fieldOfView < baseFOV - 0.5f)
                weaponCamera.fieldOfView = Mathf.Lerp(weaponCamera.fieldOfView, baseFOV, Time.deltaTime * 2f);

            if (mainCamera.fieldOfView < playerManager.movement.initFOV - 0.5f)
                mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, playerManager.movement.initFOV, Time.deltaTime * 2f);

            bulletPoint.transform.position = baseBulletPoint.position;
        }

        if (hudManager)
            hudManager.SetCrosshairState(!aiming);
    }

    private void Update()
    {
        if (!equipped)
            return;

        if (Input.GetKeyDown(KeyCode.Mouse0) && shotType == ShotType.Manual)
            TryShoot();

        if (Input.GetKey(KeyCode.Mouse0) && shotType == ShotType.Automatic)
            TryShoot();

        if (Input.GetKeyUp(KeyCode.Mouse0) && shotType == ShotType.Automatic)
            shotCounter = 0;

        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < maxAmmoInMag)
            if (!isReloading)
                StartCoroutine(Reload());
    }

    private void FixedUpdate()
    {
        if (currentRate < delayBetweenShots)
            currentRate += Time.deltaTime;

        if (!equipped)
            return;

        aiming = Input.GetKey(KeyCode.Mouse1) && !isReloading && !playerManager.movement.isSprinting;
        weaponAnimator.SetBool("Aiming", aiming);

        Aim();
    }

    private void TryShoot()
    {
        if (currentAmmo == 0)
        {
            if (!isReloading && automaticReload && playerManager.inventory.GetAmmo(ammoType) > 0)
            {
                StartCoroutine(Reload());
                return;
            }

            if (!isReloading)
                weaponAudioSystem.NoAmmo();
        }

        if (!CanShoot())
            return;

        var tracer = Instantiate(tracerEffect, bulletPoint.transform.position, Quaternion.identity);
        tracer.AddPosition(bulletPoint.transform.position);
        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out RaycastHit hit, range, attainableLayers, QueryTriggerInteraction.Ignore))
        {
            IDamageable damageable = hit.transform.GetComponentInParent<IDamageable>();
            damageable?.ApplyDamage(damage, playerManager.transform);

            var decal = poolingManager.SpawnFromPool("Decal", hit.point, Quaternion.identity);
            decal.transform.rotation = Quaternion.FromToRotation(Vector3.forward, hit.normal);
            decal.transform.parent = null;

            tracer.transform.position = hit.point;
            tracer.transform.parent = null;
        }
        else
        {
            tracer.transform.position = poitT.position;
            tracer.transform.parent = null;
        }

        currentRate = 0f;
        currentAmmo--;

        if (shotType == ShotType.Automatic)
            shotCounter++;

        // VFX
        muzzleEffect.Play(true);

        if (aiming)
            weaponAnimator.Play("AimFire", 0, 0);
        else
            weaponAnimator.Play("Fire", 0, 0);

        // Event
        OnShoot?.Invoke();

        if (shotType == ShotType.Automatic)
        {
            if (shotCounter > 3)
                StartCoroutine(RecoilMath());
        }
        else
        {
            StartCoroutine(RecoilMath());
            return;
        }
    }

    public IEnumerator RecoilMath()
    {
        print("Vai");

        playerManager.movement.y_Recoil = recoilY;
        float t = Random.Range(recoilMinMax.x, recoilMinMax.y);
        playerManager.movement.x_Recoil = t;

        yield return new WaitForSeconds(0.1f);

        playerManager.movement.y_Recoil = -recoilY * 0.5f; 
        playerManager.movement.x_Recoil = -t * 0.5f;

        yield return new WaitForSeconds(0.01f);

        playerManager.movement.y_Recoil = 0f;
        playerManager.movement.x_Recoil = 0f;
    }

    private IEnumerator Reload()
    {
        if (playerManager.inventory.GetAmmo(ammoType) > 0 || infinityAmmo)
        {
            if (weaponAnimator)
                weaponAnimator.Play("Reload", 0, 0);

            isReloading = true;

            yield return new WaitForSeconds(reloadTime);

            int ammo = maxAmmoInMag - currentAmmo;
            for (int i = 0; i < ammo; i++)
            {
                currentAmmo++;
                playerManager.inventory.UpdateAmmo(ammoType, -1);

                HandleHud();

                if (playerManager.inventory.GetAmmo(ammoType) == 0 && !infinityAmmo)
                    break;

                yield return new WaitForSeconds(0.03f);
            }

            yield return new WaitForSeconds(0.13f);
            isReloading = false;

        }
        else
        {
            yield return null;
        }
    }

    public void Healing()
    {
        weaponAnimator.Play("Healing", 0, 0);
        StartCoroutine(EndHealing());
    }

    public IEnumerator EndHealing()
    {
        yield return new WaitForSeconds(healingDelay);
        playerManager.EndHealing();
    }
}

[System.Serializable]
public struct CrosshairData
{
    public Sprite CrosshairSprite;
    public float CrosshairSize;
    public Color CrosshairColor;
}

[System.Serializable]
public struct WeaponPosition
{
    public Transform defaltTransform;
    public Transform holsterTransform;
    public Transform rootObject;
    public float movementSpeed;
}

[System.Serializable]
public class WeaponAudio
{
    [Header("AudioClips")]
    public AudioClip getWeaponSfx = null;
    [Range(0f, 1f)] public float getWeaponVolume = 1f;
    [Space]

    public AudioClip shootSfx = null;
    [Range(0f, 1f)] public float shootVolume = 1f;
    [Space]

    public AudioClip startReloadSfx = null;
    [Range(0f, 1f)] public float startReloadVolume = 1f;
    [Space]

    public AudioClip midlleReloadSfx = null;
    [Range(0f, 1f)] public float midlleReloadVolume = 1f;
    [Space]

    public AudioClip endReloadSfx = null;
    [Range(0f, 1f)] public float endReloadVolume = 1f;
    [Space]

    public AudioClip noAmmoSfx = null;
    [Range(0f, 1f)] public float noAmmoVolume = 1f;
}