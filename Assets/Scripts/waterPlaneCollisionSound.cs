using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class waterPlaneCollisionSound : MonoBehaviour{
    [SerializeField] private MeshCollider meshCollider  = null;
    [SerializeField] private AudioSource audioSplash = null;

    void Update() {
        if (meshCollider) {
            
        }
    }
    
    void OnCollisionEnter(Collision collision) {
        audioSplash.Play();
    }
}
