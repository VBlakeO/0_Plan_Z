using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using DG.Tweening;

public class Door : BaseLivingBeing
{
    [Space]
    public bool isOpen = false; 
    [SerializeField] private float time = 2f;
    [Space]

    [SerializeField] private AnimationCurve curve = null;
    [Space]

    [SerializeField] private Transform door = null;
    [SerializeField] private NavMeshObstacle navMeshObstacle = null;
    [SerializeField] private NavMeshObstacle doorNavMeshObstacle = null;
    [Space]

    public List<BaseAI> AIList = new List<BaseAI>();
    private float mDuration = 0f;

    public void SwithDoor()
    {
        if (isOpen)
            TryCloseDoor();
        else
            TryOpenDoor();
    }

    public void TryOpenDoor()
    {
        DoorEffect(true);
    }

    private void OpenDoor()
    {
        if (AIList.Count > 0)
        {
            for (int i = 0; i < AIList.Count; i++)
            {
                if (AIList[i] != null)
                    AIList[i].SetTarget(null);
            }
        }

        //doorCollider.enabled = false;
        navMeshObstacle.enabled = false;
        doorNavMeshObstacle.enabled = false;
    }

    public void TryCloseDoor()
    {
        DoorEffect(false);
    }

    private void CloseDoor()
    {
        navMeshObstacle.enabled = true;
        doorNavMeshObstacle.enabled = true;
    }

    public void DoorEffect(bool state)
    {
        if(state)
        {
            isOpen = true;
            mDuration = 0f;

            door.DORotate(new Vector3(0f, 90f, 0f), time, RotateMode.FastBeyond360).SetEase(curve).OnUpdate(() => 
            {
                
                if (mDuration >= 0 && mDuration < 1)
                {
                    mDuration += Time.deltaTime;
                }
                else if(mDuration >= 1)
                {
                    OpenDoor();
                    mDuration = -1;
                }
            });
        }
        else
        {
            isOpen = false;
            mDuration = 0f;

            door.DORotate(new Vector3(0f, 0f, 0f), time, RotateMode.FastBeyond360).SetEase(curve).OnUpdate(() => 
            {
                if (mDuration >= 0 && mDuration < 1)
                {
                    mDuration += Time.deltaTime;
                }
                else if(mDuration >= 1)
                {
                    CloseDoor();
                    mDuration = -1;
                }
            });
        }
    }

    public override void Die()
    {
        TryOpenDoor();
        navMeshObstacle.enabled = false;
        doorNavMeshObstacle.enabled = false;
        door.gameObject.SetActive(false);
    }
}
