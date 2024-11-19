using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class RCBoatController : MonoBehaviour
{

    private bool controlable = false;
    private bool leftHanded = false;
    private bool rightHanded = false;
    Vector2 direction = Vector2.zero;
    [SerializeField] private Transform leftHand;
    [SerializeField] private Transform rightHand;
    [SerializeField] private UnityEvent<Vector2> moving;
    [SerializeField] private GameObject locomotion;
    
    void Update()
    {
        OVRInput.Update();
        OVRInput.FixedUpdate();
        if (controlable)
        {
            locomotion.SetActive(false);
            if (rightHanded)
            {
                direction = OVRInput.Get(OVRInput.RawAxis2D.LThumbstick);
            }
            else if(leftHanded)
            {
                direction = OVRInput.Get(OVRInput.RawAxis2D.RThumbstick);
            }
            moving.Invoke(direction);
        }
        else
        {
            locomotion.SetActive(true);
        }
    }

    public void canControl()
    {
        controlable = true;
    }

    public void cannotControl()
    {
        controlable = false;
        direction = Vector2.zero;
    }

    public void pickup()
    {
        leftHanded = OVRInput.Get(OVRInput.Button.PrimaryHandTrigger);
        rightHanded = OVRInput.Get(OVRInput.Button.SecondaryHandTrigger);

        if (leftHanded)
        {
            GetComponent<XRGrabInteractable>().attachTransform = leftHand;
        }
        else if(rightHanded)
        {
            GetComponent<XRGrabInteractable>().attachTransform = rightHand;
        }
    }
    
    
}
