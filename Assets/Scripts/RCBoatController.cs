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
    [SerializeField] private GrabInteractable grabInteractable;
    
    void Update()
    {
        //OVRInput.Update();
        //.FixedUpdate();
        
        if (drive) {
            leftHanded = OVRInput.Get(OVRInput.Button.PrimaryHandTrigger);
            rightHanded = OVRInput.Get(OVRInput.Button.SecondaryHandTrigger);
            locomotion.SetActive(false);
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
    
}
