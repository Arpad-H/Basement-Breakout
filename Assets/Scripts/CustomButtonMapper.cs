using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CustomButtonMapper : MonoBehaviour
{
    private bool leftControlable = false;
    private bool rightControlable = false;
    private bool leftGrabbing = false;
    private bool rightGrabbing = false;
    [SerializeField] private UnityEvent active = new UnityEvent();
    [SerializeField] private UnityEvent unactive = new UnityEvent();
    
    private void Update()
    {
        if (OVRInput.Get(OVRInput.Button.PrimaryHandTrigger) && leftControlable)
        {
            leftGrabbing = true;
        }
        else
        {
            leftGrabbing = false;
        }
        
        if (OVRInput.Get(OVRInput.Button.SecondaryHandTrigger) && rightControlable)
        {
            rightGrabbing = true;
        }
        else
        {
            rightGrabbing = false;
        }

        if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger) && leftGrabbing)
        {
            active.Invoke();
        }
        else if (OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger) && rightGrabbing)
        {
            active.Invoke();
        }
        else
        {
            unactive.Invoke();
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 21)
        {
            leftControlable = true;
        }
        if (other.gameObject.layer == 22)
        {
            rightControlable = true;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 21)
        {
            leftControlable = false;
        }
        if (other.gameObject.layer == 22)
        {
            rightControlable = false;
        }
    }
}
