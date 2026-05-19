using UnityEngine;

public class WeaponAudioSystem : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource = null;
    [SerializeField] private AudioSource shootAudioSource = null;
    [Space]

    [Header("AudioClips")]
    [HideInInspector] public AudioClip getWeaponSfx = null;
    [HideInInspector] [Range(0f,1f)] public float getWeaponVolume = 1f;
    [Space]

    [HideInInspector] public AudioClip shootSfx = null;
    [HideInInspector] [Range(0f,1f)] public float shootVolume = 1f;
    [Space]

    [HideInInspector] public AudioClip startReloadSfx = null;
    [HideInInspector] [Range(0f,1f)] public float startReloadVolume = 1f;
    [Space]

    [HideInInspector] public AudioClip midlleReloadSfx = null;
    [HideInInspector] [Range(0f,1f)] public float midlleReloadVolume = 1f;
    [Space]

    [HideInInspector] public AudioClip endReloadSfx = null;
    [HideInInspector] [Range(0f,1f)] public float endReloadVolume = 1f;
    [Space]

    [HideInInspector] public AudioClip noAmmoSfx = null;
    [HideInInspector] [Range(0f, 1f)] public float noAmmoVolume = 1f;

    public void GetWeapon()
    {
        if (getWeaponSfx)
        {
            audioSource.volume = getWeaponVolume;
            audioSource.PlayOneShot(getWeaponSfx);
        }
    }

    public void Shoot()
    {
        if (shootSfx)
        {
            shootAudioSource.volume = shootVolume;
            shootAudioSource.PlayOneShot(shootSfx);
        }
    }

    public void StartReload()
    {
        if (startReloadSfx)
        {
            audioSource.volume = startReloadVolume;
            audioSource.PlayOneShot(startReloadSfx);
        }
    }

    public void MidleReload()
    {
        if (midlleReloadSfx)
        {
            audioSource.volume = midlleReloadVolume;
            audioSource.PlayOneShot(midlleReloadSfx);
        }
    }

    public void EndReload()
    {
        if (endReloadSfx)
        {
            audioSource.volume = endReloadVolume;
            audioSource.PlayOneShot(endReloadSfx);
        }
    }

    public void NoAmmo()
    {
        if (noAmmoSfx)
        {
            audioSource.volume = noAmmoVolume;
            audioSource.PlayOneShot(noAmmoSfx);
        }
    }
}