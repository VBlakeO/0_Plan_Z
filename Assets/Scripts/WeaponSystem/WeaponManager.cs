using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] private WeaponSystem currentWeapon = null;
    [SerializeField] private WeaponSystem[] weapons = null;
    [Space]

    [SerializeField] private HudManager hudManager = null;

    public void ChangeWeapon(int selectedWeapon)
    {
        if (!weapons[selectedWeapon].canEquip)
            return;

        if (currentWeapon)
        {
            if (currentWeapon == weapons[selectedWeapon] || currentWeapon.isReloading)
                return;

            WeaponSystem tempWeapon = currentWeapon;

            tempWeapon.Unequip();

            currentWeapon = weapons[selectedWeapon];
            currentWeapon.gameObject.SetActive(true);
            currentWeapon.Equip();
        }
        else
        {
            currentWeapon = weapons[selectedWeapon];
            currentWeapon.gameObject.SetActive(true);
            currentWeapon.Equip();
        }

        UnlockAllWeapoms();
    }

    public void LockAllWeapoms()
    {
        for (int i = 0; i < weapons.Length; i++)
            weapons[i].lockWeapon = true;
    }

    public void UnlockAllWeapoms()
    {
        for (int i = 0; i < weapons.Length; i++)
            weapons[i].lockWeapon = false;
    }
}