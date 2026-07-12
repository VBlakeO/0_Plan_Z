using PlanZ.Core;
using UnityEngine;

public class PlayerReference : SingletonMonoBehaviour<PlayerReference>
{
    [SerializeField] private Transform player;

    public Transform GetPlayerTransform()
    {
        if (player == null)
        {
            Debug.LogError("Player transform = NULL");
            return transform;
        }
        else
        {
            return player;
        }
    }
}