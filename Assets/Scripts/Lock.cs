using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using UnityEngine;

public class Lock : MonoBehaviour
{
    [SerializeField] private GameObject door;
    [SerializeField] private GameObject tankDummy;
    [SerializeField] private GameObject tank;
    [SerializeField] private GameObject leftHand;
    [SerializeField] private GameObject rightHand;
    //private GameObject key;
    private const int code = 0451;
    
    void Start()
    {
        tank.SetActive(false);
        //key = GameObject.Find("KeyInLock");
        //key.SetActive(false);
        //door.GetComponent<Grabbable>().enabled = false;
        door.SetActive(false);
        leftHand.SetActive(false);
        rightHand.SetActive(false);
    }

    private void unlocked()
    {
        door.SetActive(true);
        tank.SetActive(true);
        //key.SetActive(true);
        //door.GetComponent<Grabbable>().enabled = true;
        Destroy(tankDummy);
        GetComponent<Rigidbody>().isKinematic = false;
        //Destroy(other);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 21)
        {
            leftHand.SetActive(true);
            
        }

        if (other.gameObject.layer == 22)
        {
            rightHand.SetActive(true);
            
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 21)
        {
            leftHand.SetActive(false);
        }

        if (other.gameObject.layer == 22)
        {
            rightHand.SetActive(false);
        }
    }
}
