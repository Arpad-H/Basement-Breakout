using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Tank : MonoBehaviour
{
    [SerializeField] private GameObject tankHole;
    [SerializeField] private UnityEvent fueled = new UnityEvent();
    private bool canSnap = false;
    private Vector3 snapPos;

    void FixedUpdate()
    {
        if (canSnap)
        {
            transform.position = snapPos;
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 7)
        {
            fueled.Invoke();
            snapPos = other.transform.position;
            canSnap = true;
        }
            
    }
}
