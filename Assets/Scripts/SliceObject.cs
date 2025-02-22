using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EzySlice;
using Oculus.Haptics;
using Oculus.Interaction;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.XR;
using Plane = UnityEngine.Plane;

//Credits, Code with help of: https://www.youtube.com/watch?v=GQzW6ZJFQ94
public class SliceObject : MonoBehaviour {
    [SerializeField] private Transform startSlicePoint;
    [SerializeField] private Transform endSlicePoint;
    [SerializeField] private VelocityEstimator velocityEstimator;
    [SerializeField] private LayerMask slicableLayer;
    [SerializeField] private Material crossSectionMaterial;
    [SerializeField] private float cutForce = 20;
    [SerializeField] private Transform waterPos;
    [SerializeField] private AudioClip chainsawRefuelSound;
    [SerializeField] private AudioSource chainsawCutSound;
    [SerializeField] private AudioSource chainsawIdleSound;
    [SerializeField] private HapticClip chainsawRunningHapticClip;
    [SerializeField] private HapticClip chainsawPullHapticClip;
    [SerializeField] private LineRenderer pullLine;
    [SerializeField] private Transform pullPos1;
    [SerializeField] private Transform pullPos2;
    [SerializeField] private GrabInteractable grabbable;
    [SerializeField] private Blinker highlighter;

    [SerializeField] private Animator animator; //Auskommentiert, weil error
    private float chainsawRefuelSoundLength;
    private bool canCut = false;
    private bool hasFuel = false;
    private bool noWaterDamage = true;
    private bool started = false;
    private GameObject Fuelpointer;
    private HapticClipPlayer runningHapticPlayer;
    private HapticClipPlayer pullHapticPlayer;
    
    private bool leftGrabbed = false;
    private bool rightGrabbed = false;
    
    public static event Action<bool> OnHasFuelChanged; 
    
    void Start() {
        runningHapticPlayer = new HapticClipPlayer(chainsawRunningHapticClip);
        pullHapticPlayer = new HapticClipPlayer(chainsawPullHapticClip);
        chainsawRefuelSoundLength = chainsawRefuelSound.length;
        Fuelpointer = GameObject.Find("FuelSpin");
        pullLine.positionCount = 2;
        animator.Play("sawing");
    }
    
    float previousPullDistance = 0;
    float currentPullDistance = 0;
    void FixedUpdate() {
        animator.enabled = started;
        pullLine.SetPosition(0, pullPos1.position);
        pullLine.SetPosition(1, pullPos2.position);
        currentPullDistance = (pullPos1.position - pullPos2.position).magnitude;
        if (!started) {
            if (currentPullDistance > 0.3f && hasFuel) {
                started = true;
            }
        }
        HandleAudioAndHaptics();
        
        bool hasHit = Physics.Linecast(startSlicePoint.position, endSlicePoint.position, out RaycastHit hit, slicableLayer);
        if (hasHit && canCut && hasFuel && noWaterDamage) {
            GameObject target = hit.transform.gameObject;
            Slice(target);
        }

        if (transform.position.y < waterPos.position.y) {
            //noWaterDamage = false;
        }
    }
    
    public void HandleAudioAndHaptics() {
        if (grabbable.State != InteractableState.Select) {
            started = false;
            chainsawIdleSound.Stop();
            chainsawCutSound.Stop();
            runningHapticPlayer.Stop();
            pullHapticPlayer.Stop();
        }

        if (grabbable.State == InteractableState.Select) {
            if (currentPullDistance > 0.3f && currentPullDistance > previousPullDistance) {
                pullHapticPlayer.Play(Controller.Both);
            }

            previousPullDistance = currentPullDistance;

            if (started) {
                runningHapticPlayer.Play(Controller.Both);
                if (canCut) {
                    if (!chainsawCutSound.isPlaying) {
                        chainsawCutSound.volume = 1;
                        chainsawCutSound.Play();
                    }
                    //chainsawIdleSound.volume = Mathf.Lerp(chainsawIdleSound.volume, 0, Time.deltaTime);
                }
                else {
                    if (!chainsawIdleSound.isPlaying) {
                        chainsawIdleSound.volume = 1;
                        chainsawIdleSound.Play();
                    }
                    chainsawCutSound.volume = Mathf.Lerp(chainsawCutSound.volume, 0, Time.deltaTime);
                }
            }
        }
    }
    
    public void Slice(GameObject target) {
        if (!chainsawCutSound.isPlaying) {
            // chainsawCutSound.Play();
        }
        Vector3 velocity = velocityEstimator.GetVelocityEstimate();
        Vector3 planeNormal = Vector3.Cross(endSlicePoint.position - startSlicePoint.position, velocity);
        planeNormal.Normalize();
        
        SlicedHull hull = target.Slice(endSlicePoint.position, planeNormal);

        if (hull != null) {
            GameObject upperHull = hull.CreateUpperHull(target, crossSectionMaterial);
            SetupSlicedComponent(upperHull);
            GameObject lowerHull = hull.CreateLowerHull(target, crossSectionMaterial);
            SetupSlicedComponent(lowerHull);
            upperHull.layer = LayerMask.NameToLayer("Sliceable");
            lowerHull.layer = LayerMask.NameToLayer("Sliceable");
            
            print("HULLS: " + upperHull + " " + lowerHull);
            
            Destroy(target);
        }
    }

    public void SetupSlicedComponent(GameObject slicedObject) {
        Rigidbody rb = slicedObject.AddComponent<Rigidbody>();
        MeshCollider collider = slicedObject.AddComponent<MeshCollider>();
        collider.convex = true;
        rb.AddExplosionForce(cutForce, slicedObject.transform.position, 1);
    }

    public void fueledUp() {
        hasFuel = true;
        Destroy(highlighter);
        OnHasFuelChanged?.Invoke(hasFuel);
    }
    
    float timePlaying = 0;
    public void fueling(float fuel) {
        if (chainsawRefuelSoundLength - timePlaying >= chainsawRefuelSoundLength) {
            SoundFXManager.instance.PlaySoundFX(chainsawRefuelSound, this.transform, .5f);
        }
        timePlaying += Time.deltaTime;
        if (chainsawRefuelSoundLength - timePlaying <= 0) {
            timePlaying = 0;
        }
        
        Fuelpointer.transform.Rotate(0, fuel, 0);
    }

    public void sawing() {
        canCut = started;
        animator.SetFloat("Speed", 4f);
    }
    
    public void notSawing() {
        animator.SetFloat("Speed", 0.5f);
        chainsawCutSound.Stop();
        canCut = false;
    }
}
