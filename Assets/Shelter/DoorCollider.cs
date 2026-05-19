using System.Collections.Generic;
using UnityEngine;

public class DoorCollider : MonoBehaviour
{
    [SerializeField] private Door myDoor = null;
    [SerializeField] private float attackDistance = 1f;
    [SerializeField] private Shelter shelter = null;
    
    [SerializeField] private List<Zombie_AI> enemysInDoor = new List<Zombie_AI>();


    void Start()
    {
        myDoor = GetComponentInParent<Door>();

        shelter.OnPlayerEnter += PlayerInside;
        shelter.OnPlayerExit += PlayerInside;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!myDoor.isOpen && other.GetComponent<Zombie_AI>())
        {
            Zombie_AI zombie_AI = other.GetComponent<Zombie_AI>();  
                
            if ((zombie_AI.enemyInside && !shelter.playerInsideThisShelter) || (!zombie_AI.enemyInside && shelter.playerInsideThisShelter))
            {
                //if (zombie_AI.CheckDoor())
                //{
                zombie_AI.SetTarget(myDoor.transform);
                zombie_AI.SetAttackDistance(attackDistance);

                if (!enemysInDoor.Contains(zombie_AI))
                    enemysInDoor.Add(zombie_AI);

                if (!myDoor.AIList.Contains(zombie_AI))
                    myDoor.AIList.Add(other.GetComponent<Zombie_AI>());
                //}
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Zombie_AI>())
        {
            Zombie_AI zombie_AI = other.GetComponent<Zombie_AI>();

            if (enemysInDoor.Contains(zombie_AI))
                enemysInDoor.Remove(zombie_AI);
        }
    }

    private void PlayerInside()
    {
        print("Ele Saiuuu");

        for (int i = 0; i < enemysInDoor.Count; i++)
        {
            // Eles estão em lados opostos?
            if (!enemysInDoor[i].CheckDoor()) // Se não
            {
                print("Saiu Mesmo");
                if (enemysInDoor[i].GetTarget() == myDoor.transform) 
                {
                    print("Mesmo Mesmo");
                    enemysInDoor[i].SetTarget(null);
                }
            }
        }
    }

    void OnDisable()
    {
        shelter.OnPlayerEnter -= PlayerInside;
        shelter.OnPlayerExit -= PlayerInside;
    }
}
