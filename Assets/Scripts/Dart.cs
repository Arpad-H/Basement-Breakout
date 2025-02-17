using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dart : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 25)
        {
            //gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            gameObject.GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    public void isNotKinematic()
    {
       //gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
       gameObject.GetComponent<Rigidbody>().isKinematic = false;
    }
}
