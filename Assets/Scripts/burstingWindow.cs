using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class burstingWindow : MonoBehaviour
{
    [SerializeField]float timeTillBurst = 5f;
    [SerializeField] GameObject solidWindow;
    bool windowBurst = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timeTillBurst -= Time.deltaTime;
        if (timeTillBurst <= 0 && !windowBurst)
        {
            windowBurst = true;
          StartCoroutine(BurstWindow());
        }
    }
    IEnumerator BurstWindow()
    {
        Destroy(solidWindow);
        foreach (MeshCollider mc in GetComponentsInChildren<MeshCollider>())
        {
            mc.enabled = true;
            mc.GameObject().AddComponent<ConstantForce>().force = new Vector3(Random.Range(0f,1f)*8, 0, 0);
        }

        foreach (var rb in GetComponentsInChildren<Rigidbody>())
        {
            rb.useGravity = true;
        }
        
        yield return new WaitForSeconds(3);
        Destroy(this.GameObject());
    }
}
