using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;

public class MarbleMaze : MonoBehaviour
{
    [SerializeField] private Transform leftHand;
    [SerializeField] private Transform rightHand;
    [SerializeField] private GrabInteractable grabInteractable;
    
    public void setGrabPointLeft()
    {
        grabInteractable.InjectOptionalGrabSource(leftHand);
    }
    public void setGrabPointRight()
    {
        grabInteractable.InjectOptionalGrabSource(rightHand);
    }
}
