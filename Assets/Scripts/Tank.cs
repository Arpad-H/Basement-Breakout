using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Tank : MonoBehaviour
{
    [SerializeField] private GameObject tankHole;
    [SerializeField] private UnityEvent fueled = new UnityEvent();
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 7)
        {
            fueled.Invoke();
        }
            
    }
}
