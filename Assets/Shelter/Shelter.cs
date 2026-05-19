using System;
using UnityEngine;
using UnityEngine.Events;

public class Shelter : MonoBehaviour
{
    [SerializeField] private int count = 0;
    public bool playerInsideThisShelter = false;
    
    public Action OnPlayerEnter = null;
    public Action OnPlayerExit = null;

    void OnTriggerEnter(Collider other)
    {
        other.GetComponent<PlayerMovementSystem>()?.SetInsideShelter(true);
        other.GetComponent<Zombie_AI>()?.SetInsideShelter(true);
        
        if (other.GetComponent<PlayerMovementSystem>())
        {
            playerInsideThisShelter = true;
            OnPlayerEnter?.Invoke();
        }
        
        count++;
    }

    void OnTriggerExit(Collider other)
    {
        other.GetComponent<PlayerMovementSystem>()?.SetInsideShelter(false);
        other.GetComponent<Zombie_AI>()?.SetInsideShelter(false);

        if (other.GetComponent<PlayerMovementSystem>())
        {
            playerInsideThisShelter = false;
            OnPlayerExit?.Invoke();
        }

        count--;
    }
}
