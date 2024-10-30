using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EzySlice;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using Plane = UnityEngine.Plane;

//Credits, Code by: https://www.youtube.com/watch?v=GQzW6ZJFQ94
public class SliceObject : MonoBehaviour
{
    [SerializeField] private Transform startSlicePoint;
    [SerializeField] private Transform endSlicePoint;
    [SerializeField] private VelocityEstimator velocityEstimator;
    [SerializeField] private LayerMask slicableLayer;
    [SerializeField] private Material crossSectionMaterial;
    [SerializeField] private float cutForce = 20;
    private bool canCut = false;
    private bool hasFuel = false;
    private GameObject FuelGauge;
    
    void Start()
    {
        FuelGauge = GameObject.Find("Fuel");
        FuelGauge.transform.localScale = new Vector3(0.001f, 1, 0);
    }

    void FixedUpdate()
    {
        bool hasHit = Physics.Linecast(startSlicePoint.position, endSlicePoint.position, out RaycastHit hit, slicableLayer);
        if (hasHit && canCut && hasFuel)
        {
            GameObject target = hit.transform.gameObject;
            Slice(target);
            print("RAAAAAAHHHHHH");
        }
    }

    public void Slice(GameObject target)
    {
        Vector3 velocity = velocityEstimator.GetVelocityEstimate();
        Vector3 planeNormal = Vector3.Cross(endSlicePoint.position - startSlicePoint.position, velocity);
        planeNormal.Normalize();
        
        SlicedHull hull = target.Slice(endSlicePoint.position, planeNormal);

        if (hull != null)
        {
            GameObject upperHull = hull.CreateUpperHull(target, crossSectionMaterial);
            SetupSlicedComponent(upperHull);
            GameObject lowerHull = hull.CreateLowerHull(target, crossSectionMaterial);
            SetupSlicedComponent(lowerHull);
            //upperHull.layer = LayerMask.NameToLayer("Sliceable");
            //lowerHull.layer = LayerMask.NameToLayer("Sliceable");
            
            Destroy(target);
        }
    }

    public void SetupSlicedComponent(GameObject slicedObject)
    {
        Rigidbody rb = slicedObject.AddComponent<Rigidbody>();
        MeshCollider collider = slicedObject.AddComponent<MeshCollider>();
        collider.convex = true;
        rb.AddExplosionForce(cutForce, slicedObject.transform.position, 1);
    }

    public void canSaw()
    {
        canCut = true;
    }
    
    public void cantSaw()
    {
        canCut = false;
    }

    public void fueledUp()
    {
        hasFuel = true;
    }

    public void fueling(float fuel)
    {
        FuelGauge.transform.localScale = new Vector3(0.001f, 1, fuel);
    }
}
