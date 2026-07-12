using System.Collections.Generic;
using PlanZ.Weapons.Data;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInventory : MonoBehaviour
{
    //[SerializeField] private WeaponManager weaponManager = null;
    [SerializeField] private PlaceObject placeObject = null;
    [Space]

    #region AmmoInventory
    public Dictionary<AmmoType, int> ammoBag = new Dictionary<AmmoType, int>();
    [Header("AmmoInventory")]

    [SerializeField] private int rifleMaxAmmo = 300;
    [SerializeField] private int pistolMaxAmmo = 250;
    [SerializeField] private int shotgunMaxAmmo = 150;
    [Space]

    [SerializeField] private int rifleInitAmmo = 90;
    [SerializeField] private int pistolInitAmmo = 75;
    [SerializeField] private int shotgunInitAmmo = 45;
    [Space]

    public UnityAction OnUpdateAmmo = null;
    #endregion

    #region FirstAidInventory
    [Header("FirstAidInventory")]

    public int currentFirstAidAmount = 0;
    [SerializeField] private int maxFirstAid = 3;

    public UnityAction OnUpdateFAK = null;
    #endregion

    #region EquipamentInventory
    [Header("EquipamentInventory")]
    
    public int[] equipment = new int[2];
    public int maxValue = 3;
    public UnityAction OnUpdateItem = null;
    #endregion

    void Awake()
    {
        ammoBag.Add(AmmoType.Rifle, rifleInitAmmo);
        ammoBag.Add(AmmoType.Pistol, pistolInitAmmo);
        ammoBag.Add(AmmoType.Shotgun, shotgunInitAmmo );
    }

    public void Start()
    {
        for (int i = 0; i < equipment.Length; i++)
            equipment[i] = maxValue;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            placeObject.EquipObject(false);
            //weaponManager.ChangeWeapon(0);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            placeObject.EquipObject(false);
            //weaponManager.ChangeWeapon(1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            placeObject.EquipObject(false);
            //weaponManager.ChangeWeapon(2);
        }

        //////

        if (Input.GetKeyDown(KeyCode.Alpha4))
            placeObject.SelectObject(0);

        if (Input.GetKeyDown(KeyCode.Alpha5))
            placeObject.SelectObject(1);
    }

    #region Ammo
    public bool TryAddAmmo(AmmoType key, int value)
    {
        int weaponMaxAmmo = 0;

        if(ammoBag.ContainsKey(key))
        {
            weaponMaxAmmo = key switch
            {
                AmmoType.Rifle => rifleMaxAmmo,
                AmmoType.Pistol => pistolMaxAmmo,
                AmmoType.Shotgun => shotgunMaxAmmo,
                _ => 0,
            };
        }

        if (ammoBag[key] < weaponMaxAmmo)
        {
            print (ammoBag[key].ToString() + " " + weaponMaxAmmo.ToString());

            if (ammoBag[key] + value > weaponMaxAmmo)
                ammoBag[key] = weaponMaxAmmo;
            else
                ammoBag[key] += value;

            OnUpdateAmmo?.Invoke();

            return true;
        }
        else
            return false;
    }

    public void UpdateAmmo(AmmoType key, int value)
    {
        if(ammoBag.ContainsKey(key))
            ammoBag[key] += value;
        else
            ammoBag.Add(key, value);
    }

    public int GetAmmo(AmmoType key)
    {
        if (ammoBag.ContainsKey(key))
            return ammoBag[key];
        else
            return 0;
    }

    #endregion

    #region FirstAid
    public bool TryAddFirstAidKit(int value)
    {
        if (currentFirstAidAmount < maxFirstAid)
        {
            if (currentFirstAidAmount + value > maxFirstAid)
                currentFirstAidAmount = maxFirstAid;
            else
                currentFirstAidAmount += value;
            
            OnUpdateFAK?.Invoke();

            return true;
        }
        else
            return false;
    }

    public void UseFirstAid()
    {
        if (currentFirstAidAmount > 0)
        {
            currentFirstAidAmount -= 1;
            OnUpdateFAK?.Invoke();
        }
    }
    #endregion

    #region Equipament
    public bool TryAddItem(int item, int amount)
    {
        if (equipment[item] < maxValue)
        {
            if (equipment[item] + amount <= maxValue)
            {
                equipment[item] += amount;
                OnUpdateItem?.Invoke();
                return true;

            }
            else
                return false;
        }
        else
            return false;
    }

    public void RemoveItem(int item, int amount)
    {
        if (equipment[item] > 0)
            equipment[item] -= amount;

        if (equipment[item] < 0)
            equipment[item] = 0;

        OnUpdateItem?.Invoke();
    }
    #endregion
}
