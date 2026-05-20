using System;
using UnityEngine;
using PlanZ.Events;
using PlanZ.Player.Events;

namespace PlanZ.Player
{
    [Serializable]
    public class PlayerStateLocks
    {
        [SerializeField] private bool cantMove;
        [SerializeField] private bool cantLook;
        [SerializeField] private bool cantJump;
        [SerializeField] private bool cantSprint;
        [SerializeField] private bool cantCrouch;

        public bool CantMove => cantMove;
        public bool CantLook => cantLook;
        public bool CantJump => cantJump;
        public bool CantSprint => cantSprint;
        public bool CantCrouch => cantCrouch;

        public void LockAll(bool value)
        {
            cantMove = value;
            cantLook = value;
            cantJump = value;
            cantSprint = value;
            cantCrouch = value;
            EventBus.Publish(new PlayerLockChangedEvent());
        }

        public void SetLookLock(bool value)
        {
            cantLook = value;
            EventBus.Publish(new PlayerLockChangedEvent());
        }
    }
}
