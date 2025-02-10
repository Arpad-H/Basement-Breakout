using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class burstingWindow : MonoBehaviour
{
    
    // [SerializeField] GameObject solidWindow;
    [SerializeField] GameObject shatteredWindow;
    ParticleSystem ps;

    private void Awake()
    {
        ps = shatteredWindow.GetComponent<ParticleSystem>();
    }
    public void BurstWindow() {
        Debug.LogError("Bursting window");
        ps.Play();
        // StartCoroutine(BurstWindowCorutine());
    }
    // IEnumerator BurstWindowCorutine() {
    //     shatteredWindow.SetActive(true);
    //     Destroy(solidWindow);
    //     foreach (MeshCollider mc in GetComponentsInChildren<MeshCollider>())
    //     {
    //         mc.enabled = true;
    //         mc.GameObject().AddComponent<ConstantForce>().force = new Vector3(Random.Range(0f,1.5f)*20, 0, 0);
    //     } 
    //     foreach (BoxCollider mc in GetComponentsInChildren<BoxCollider>())
    //     {
    //         mc.enabled = true;
    //         mc.GameObject().AddComponent<ConstantForce>().force = new Vector3(Random.Range(0f,1.5f)*20, 0, 0);
    //     }
    //
    //     foreach (var rb in GetComponentsInChildren<Rigidbody>())
    //     {
    //         rb.useGravity = true;
    //     }
    //     
    //     yield return new WaitForSeconds(5);
    //     Destroy(this.GameObject());
    // }
}
