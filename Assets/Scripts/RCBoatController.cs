using System;
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
    [SerializeField] private Rigidbody boat;
    [SerializeField] private float speed = 2f;
    [SerializeField] private Transform leftHand;
    [SerializeField] private Transform rightHand;
    //[SerializeField] private UnityEvent<Vector2> moving;
    [SerializeField] private GameObject locomotion;
    [SerializeField] private GameObject teleportIndicator1;
    [SerializeField] private GameObject teleportIndicator2;
    [SerializeField] private GrabInteractable grabInteractable;
    [SerializeField] private AudioSource boatSound;
    [SerializeField] private GameObject winBox;
    [SerializeField] private GameObject display;
    [SerializeField] private GameObject displayOff;
    [SerializeField] private GameObject highlighter;

    private GameObject battery;
    private GameObject emptyIndicator;
    private bool hasBattery = false;

    private void Awake() {
        display.SetActive(false);
        displayOff.SetActive(true);
    }

    void Start() {
        winBox.SetActive(false);
        battery = GameObject.Find("InsertedBatteryController");
        battery.SetActive(false);
        emptyIndicator = GameObject.Find("EmptyBattery");
    }
    void Update() {
        //OVRInput.Update();
        //.FixedUpdate();
        
        if (drive && hasBattery) {
            leftHanded = OVRInput.Get(OVRInput.Button.PrimaryHandTrigger);
            rightHanded = OVRInput.Get(OVRInput.Button.SecondaryHandTrigger);
            locomotion.SetActive(false);
            teleportIndicator1.SetActive(false);
            teleportIndicator2.SetActive(false);
            boatSound.volume = Mathf.Lerp(boatSound.volume, 1, Time.deltaTime);
            if (rightHanded) {
                direction = OVRInput.Get(OVRInput.RawAxis2D.RThumbstick);
            }
            else if (leftHanded) {
                direction = OVRInput.Get(OVRInput.RawAxis2D.LThumbstick);
            } else {
                direction = Vector2.zero;
            }
        }
        else {
            boatSound.volume = Mathf.Lerp(boatSound.volume, 0, Time.deltaTime);
            locomotion.SetActive(true);
            teleportIndicator1.SetActive(true);
            teleportIndicator2.SetActive(true);
            direction = Vector2.zero;
        }
        moveBoat();
    }

    private void moveBoat()
    {
        boat.AddRelativeForce(new Vector3(0, 0, direction.y * speed), ForceMode.Acceleration);
        boat.AddRelativeTorque(new Vector3(0, direction.x, 0), ForceMode.Acceleration);
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
        winBox.SetActive(true);
        display.SetActive(true);
        displayOff.SetActive(false);
        hasBattery = true;
        battery.SetActive(true);
        emptyIndicator.SetActive(false);
        highlighter.SetActive(false);
    }
    
}
