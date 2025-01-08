using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using UnityEngine;

public class Lock : MonoBehaviour
{
    [SerializeField] private GameObject door;
    [SerializeField] private GameObject tankDummy;
    [SerializeField] private GameObject tank;
    private GameObject key;
    
    void Start()
    {
        tank.SetActive(false);
        key = GameObject.Find("KeyInLock");
        key.SetActive(false);
        //door.GetComponent<Grabbable>().enabled = false;
        door.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 28)
        {
            door.SetActive(true);
            tank.SetActive(true);
            key.SetActive(true);
            //door.GetComponent<Grabbable>().enabled = true;
            Destroy(tankDummy);
            GetComponent<Rigidbody>().isKinematic = false;
            Destroy(other.gameObject);
        }
    }
}
