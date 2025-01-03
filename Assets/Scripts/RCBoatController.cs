using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class RCBoatController : MonoBehaviour
{
    private bool drive = false;
    private bool leftHanded = false;
    private bool rightHanded = false;
    Vector2 direction = Vector2.zero;
    [SerializeField] private Transform leftHand;
    [SerializeField] private Transform rightHand;
    [SerializeField] private UnityEvent<Vector2> moving;
    [SerializeField] private GameObject locomotion;
    [SerializeField] private GameObject teleportIndicator1;
    [SerializeField] private GameObject teleportIndicator2;
    [SerializeField] private GrabInteractable grabInteractable;

    private GameObject battery;
    private GameObject emptyIndicator;
    private bool hasBattery = false;

    void Start()
    {
        battery = GameObject.Find("InsertedBatteryController");
        battery.SetActive(false);
        emptyIndicator = GameObject.Find("EmptyBattery");
    }
    void Update()
    {
        //OVRInput.Update();
        //.FixedUpdate();
        
        if (drive && hasBattery) {
            leftHanded = OVRInput.Get(OVRInput.Button.PrimaryHandTrigger);
            rightHanded = OVRInput.Get(OVRInput.Button.SecondaryHandTrigger);
            locomotion.SetActive(false);
            teleportIndicator1.SetActive(false);
            teleportIndicator2.SetActive(false);
            if (rightHanded)
            {
                direction = OVRInput.Get(OVRInput.RawAxis2D.RThumbstick);
            }
            else if(leftHanded)
            {
                direction = OVRInput.Get(OVRInput.RawAxis2D.LThumbstick);
            }
            else
            {
                direction = Vector2.zero;
            }
        }
        else {
            locomotion.SetActive(true);
            teleportIndicator1.SetActive(true);
            teleportIndicator2.SetActive(true);
            direction = Vector2.zero;
        }
        moving.Invoke(direction);
    }

    public void canDrive()
    {
        drive = true;
    }

    public void cannotDrive()
    {
        drive = false;
    }

    public void setGrabPointLeft()
    {
        grabInteractable.InjectOptionalGrabSource(leftHand);
    }
    public void setGrabPointRight()
    {
        grabInteractable.InjectOptionalGrabSource(rightHand);
    }
    
    public void isFull()
    {
        hasBattery = true;
        battery.SetActive(true);
        emptyIndicator.SetActive(false);
    }
    
}
