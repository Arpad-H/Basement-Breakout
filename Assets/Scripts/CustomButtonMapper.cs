using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
[ExecuteAlways]
public class CustomButtonMapper : MonoBehaviour
{
    private bool leftControlable = false;
    private bool rightControlable = false;
    private bool leftGrabbing = false;
    private bool rightGrabbing = false;
    [SerializeField] private UnityEvent active = new UnityEvent();
    [SerializeField] private UnityEvent unactive = new UnityEvent();
    [SerializeField] private UnityEvent leftGrab = new UnityEvent();
    [SerializeField] private UnityEvent leftUngrab = new UnityEvent();
    [SerializeField] private UnityEvent rightGrab = new UnityEvent();
    [SerializeField] private UnityEvent rightUngrab = new UnityEvent();
    
    [SerializeField] private UnityEvent justActive = new UnityEvent();
    [SerializeField] private UnityEvent justLeftGrab = new UnityEvent();
    [SerializeField] private UnityEvent justRightGrab = new UnityEvent();
    
    private void Update()
    {
        if (OVRInput.Get(OVRInput.Button.PrimaryHandTrigger) && leftControlable)
        {
            leftGrabbing = true;
            leftGrab.Invoke();
        }
        else
        {
            leftGrabbing = false;
            leftUngrab.Invoke();
        }
        
        if (OVRInput.Get(OVRInput.Button.SecondaryHandTrigger) && rightControlable)
        {
            rightGrabbing = true;
            rightGrab.Invoke();
        }
        else
        {
            rightGrabbing = false;
            rightUngrab.Invoke();
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

        if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger) && leftControlable)
        {
            justLeftGrab.Invoke();
        }
        else if (OVRInput.GetDown(OVRInput.Button.SecondaryHandTrigger) && rightControlable)
        {
            justRightGrab.Invoke();
        }

        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) && leftGrabbing)
        {
            justActive.Invoke();
        }
        else if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger) && rightGrabbing)
        {
            justActive.Invoke();
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
    public void AddActiveListener(UnityAction action)
    {
        active.AddListener(action);
    }
    public void AddUnactiveListener(UnityAction action)
    {
        unactive.AddListener(action);
    }
}
