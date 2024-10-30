using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Tank : MonoBehaviour
{
    [SerializeField] private GameObject tankHole;
    [SerializeField] private UnityEvent fueled = new UnityEvent();
    [SerializeField] private UnityEvent<float> fueling;
    private bool fueledUp = false;
    private float fuelTimer = 0;

    void FixedUpdate()
    {
        if (fueledUp)
        {
            fuelTimer += Time.deltaTime;
            if (fuelTimer >= 3)
            {
                fueled.Invoke();
            }
            else
            {
                fueling.Invoke(fuelTimer/10);
            }
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 7)
        {
            fueledUp = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 7)
        {
            fueledUp = false;
        }
    }
}
