using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] private float disableRate = 8f;
    [SerializeField] private Image remaningTime = null;

    private float currentRate = 0f;

    private void OnEnable()
    {
        
    }

    private void FixedUpdate() 
    {
        if (currentRate < disableRate)
            currentRate += Time.deltaTime;
        else
            Disable();

        if (!remaningTime)
            return;

        remaningTime.fillAmount = currentRate/disableRate;   
    }

    private void Disable()
    {
        Destroy(gameObject);
    }
}
