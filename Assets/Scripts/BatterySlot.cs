using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BatterySlot : MonoBehaviour
{
    [SerializeField] private UnityEvent insertBattery;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            insertBattery.Invoke();
            Destroy(other.gameObject);
        }
    }
}
