using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class waterPlaneCollisionSound : MonoBehaviour{
    [SerializeField] private MeshCollider meshCollider  = null;
    [SerializeField] private AudioSource audioSplash = null;
    [SerializeField] private AudioSource audioSplash2 = null;
    [SerializeField] private AudioSource audioSplash3 = null;

    private void OnTriggerEnter(Collider other) {
        int random = Random.Range(0, 3);
        if (random == 0) audioSplash.Play();
        else if (random == 1) audioSplash2.Play();
        else audioSplash3.Play();
    }
}
