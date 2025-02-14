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
            gameObject.SetActive(false);
            //Destroy(gameObject);
            if (deletable != null)
            {
                deletable.SetActive(false);
                //Destroy(deletable);
            }
            if (oldBattery != null)
            {
                Instantiate(battery, oldBattery.transform.position, oldBattery.transform.rotation);
                oldBattery.SetActive(false);
                //Destroy(oldBattery);
            }
        }
    }
}
