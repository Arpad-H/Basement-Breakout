using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EzySlice;
using Oculus.Haptics;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.XR;
using Plane = UnityEngine.Plane;

//Credits, Code by: https://www.youtube.com/watch?v=GQzW6ZJFQ94
public class SliceObject : MonoBehaviour {
    [SerializeField] private Transform startSlicePoint;
    [SerializeField] private Transform endSlicePoint;
    [SerializeField] private VelocityEstimator velocityEstimator;
    [SerializeField] private LayerMask slicableLayer;
    [SerializeField] private Material crossSectionMaterial;
    [SerializeField] private float cutForce = 20;
    [SerializeField] private Transform waterPos;
    [SerializeField] private AudioClip chainsawRefuelSound;
    [SerializeField] private AudioSource chainsawStartUpSound;
    [SerializeField] private AudioSource chainsawIdleSound;
    [SerializeField] private HapticClip chainsawRunningHapticClip;
    [SerializeField] private HapticClip chainsawPullHapticClip;
    [SerializeField] private LineRenderer pullLine;
    [SerializeField] private Transform pullPos1;
    [SerializeField] private Transform pullPos2;

    //[SerializeField] private Animator animator; //Auskommentiert, weil error
    private float chainsawRefuelSoundLength;
    private bool canCut = false;
    private bool hasFuel = false;
    private bool noWaterDamage = true;
    private bool started = false;
    private GameObject Fuelpointer;
    private HapticClipPlayer runningHapticPlayer;
    private HapticClipPlayer pullHapticPlayer;
    private GameObject PullMeterPointer;
    
    public static event Action<bool> OnHasFuelChanged; 
    
    void Start() {
        runningHapticPlayer = new HapticClipPlayer(chainsawRunningHapticClip);
        pullHapticPlayer = new HapticClipPlayer(chainsawPullHapticClip);
        chainsawRefuelSoundLength = chainsawRefuelSound.length;
        Fuelpointer = GameObject.Find("FuelSpin");
        PullMeterPointer = GameObject.Find("PullMeterPointer");
        pullLine.positionCount = 2;
    }
    
    float previousPullDistance = 0;
    float currentPullDistance = 0;
    void FixedUpdate() {
        pullLine.SetPosition(0, pullPos1.position);
        pullLine.SetPosition(1, pullPos2.position);
        currentPullDistance = (pullPos1.position - pullPos2.position).magnitude;
        if (!started) {
            PullMeterPointer.transform.localPosition = new Vector3(-(pullPos1.position - pullPos2.position).magnitude/50, 0, 0);
            // currentPullDistance = (pullPos1.position - pullPos2.position).magnitude;
            if (currentPullDistance > 0.3f) {
                started = true;
            }
        }
        
        if (currentPullDistance > 0.3f && currentPullDistance > previousPullDistance) {
            pullHapticPlayer.Play(Controller.Both);
        }
        previousPullDistance = currentPullDistance;
        
        if (!chainsawIdleSound.isPlaying && canCut && hasFuel && noWaterDamage) {
            chainsawIdleSound.Play();
            runningHapticPlayer.Play(Controller.Both);
            //animator.SetBool("isSawing", true);
        }
        bool hasHit = Physics.Linecast(startSlicePoint.position, endSlicePoint.position, out RaycastHit hit, slicableLayer);
        if (hasHit && canCut && hasFuel && noWaterDamage) {
            GameObject target = hit.transform.gameObject;
            Slice(target);
        }

        if (transform.position.y < waterPos.position.y) {
            noWaterDamage = false;
        }
    }
    
    public void Slice(GameObject target) {
        if (!chainsawStartUpSound.isPlaying) {
            chainsawStartUpSound.Play();
            
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
        if(started)
            canCut = true;
    }
    
    public void notSawing() {
        canCut = false;
        //animator.SetBool("isSawing", false);
    }
}
