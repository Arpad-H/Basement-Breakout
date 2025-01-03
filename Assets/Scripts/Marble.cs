using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marble : MonoBehaviour
{
    [SerializeField] private Transform startTransform;
    
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != 28)
        {
            //transform.position = startTransform.position;
        }
    }
}
