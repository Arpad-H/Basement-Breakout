using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarbleTrigger : MonoBehaviour
{
    [SerializeField] private GameObject door;
    [SerializeField] private GameObject self;
    [SerializeField] private GameObject battery;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 27)
        {
            Destroy(gameObject);
            if (door != null)
            {
                Destroy(door);
            }
            if (self != null)
            {
                Instantiate(battery, self.transform.position, self.transform.rotation);
                Destroy(self);
            }
        }
    }
}
