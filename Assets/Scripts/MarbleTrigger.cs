using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MarbleTrigger : MonoBehaviour
{
    [SerializeField] private GameObject deletable;
    [SerializeField] private GameObject battery;
    [SerializeField] private GameObject oldBattery;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 27)
        {
            Destroy(gameObject);
            if (deletable != null)
            {
                Destroy(deletable);
            }
            if (oldBattery != null)
            {
                Instantiate(battery, oldBattery.transform.position, oldBattery.transform.rotation);
                Destroy(oldBattery);
            }
        }
    }
}
