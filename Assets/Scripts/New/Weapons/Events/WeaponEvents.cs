using UnityEngine;
using PlanZ.Events;
using PlanZ.Weapons.Data;

namespace PlanZ.Weapons.Events
{
    public readonly struct WeaponEquippedEvent : IEvent
    {
        public WeaponData Data { get; }
        public int CurrentMagazine { get; }

        public WeaponEquippedEvent(WeaponData data, int currentMagazine)
        {
            Data = data;
            CurrentMagazine = currentMagazine;
        }
    }

    public readonly struct WeaponUnequippedEvent : IEvent
    {
        public WeaponData Data { get; }
        public WeaponUnequippedEvent(WeaponData data) => Data = data;
    }

    public readonly struct WeaponFiredEvent : IEvent
    {
        public WeaponData Data { get; }
        public bool HitSomething { get; }
        public Vector3 HitPoint { get; }
        public Vector3 HitNormal { get; }

        public WeaponFiredEvent(WeaponData data, bool hitSomething, Vector3 hitPoint, Vector3 hitNormal)
        {
            Data = data;
            HitSomething = hitSomething;
            HitPoint = hitPoint;
            HitNormal = hitNormal;
        }
    }

    public readonly struct WeaponEmptyTriggerEvent : IEvent
    {
        public WeaponData Data { get; }
        public WeaponEmptyTriggerEvent(WeaponData data) => Data = data;
    }

    public readonly struct WeaponReloadStartedEvent : IEvent
    {
        public WeaponData Data { get; }
        public WeaponReloadStartedEvent(WeaponData data) => Data = data;
    }

    public readonly struct WeaponReloadFinishedEvent : IEvent
    {
        public WeaponData Data { get; }
        public WeaponReloadFinishedEvent(WeaponData data) => Data = data;
    }

    public readonly struct WeaponAmmoChangedEvent : IEvent
    {
        public WeaponData Data { get; }
        public int CurrentMagazine { get; }
        public int Reserve { get; }

        public WeaponAmmoChangedEvent(WeaponData data, int currentMagazine, int reserve)
        {
            Data = data;
            CurrentMagazine = currentMagazine;
            Reserve = reserve;
        }
    }

    public readonly struct WeaponAimStateChangedEvent : IEvent
    {
        public WeaponData Data { get; }
        public bool IsAiming { get; }

        public WeaponAimStateChangedEvent(WeaponData data, bool isAiming)
        {
            Data = data;
            IsAiming = isAiming;
        }
    }

    public readonly struct AmmoReserveChangedEvent : IEvent
    {
        public AmmoType AmmoType { get; }
        public int Amount { get; }

        public AmmoReserveChangedEvent(AmmoType ammoType, int amount)
        {
            AmmoType = ammoType;
            Amount = amount;
        }
    }
}