using UnityEngine;
using PlanZ.Weapons.Data;

namespace PlanZ.Weapons.Pickups
{
    [CreateAssetMenu(fileName = "AmmoBoxData", menuName = "PlanZ/Weapons/Ammo Box Data")]
    public class AmmoBoxData : ScriptableObject
    {
        [SerializeField] private AmmoType ammoType = AmmoType.Pistol;
        [SerializeField] private int amount = 30;

        public AmmoType AmmoType => ammoType;
        public int Amount => amount;
    }
}
