using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Battery : MonoBehaviour
{
    
    [SerializeField] private UnityEvent insertBattery;
    private bool inserted = false;

    void Update()
    {
        if (inserted)
        {
            inserted = false;
            insertBattery.Invoke();
            Destroy(gameObject);
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            inserted = true;
        }
    }
}
