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
    [SerializeField] private HapticClip hapticClip;
    private float chainsawRefuelSoundLength;
    private bool canCut = false;
    private bool hasFuel = false;
    private bool noWaterDamage = true;
    private GameObject FuelGauge;
    private HapticClipPlayer hapticClipPlayer;
    
    void Start() {
        hapticClipPlayer = new HapticClipPlayer(hapticClip);
        chainsawRefuelSoundLength = chainsawRefuelSound.length;
        FuelGauge = GameObject.Find("Fuel");
        FuelGauge.transform.localScale = new Vector3(0.001f, 1, 0);
    }

    void FixedUpdate() {
        if (!chainsawIdleSound.isPlaying && canCut) {
            chainsawIdleSound.Play();
            hapticClipPlayer.Play(Controller.Both);
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
        
        FuelGauge.transform.localScale = new Vector3(0.001f, 1, fuel);
    }

    public void sawing() {
        canCut = true;
    }
    
    public void notSawing() {
        canCut = false;
    }
}
