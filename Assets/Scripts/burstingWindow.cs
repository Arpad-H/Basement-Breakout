using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class burstingWindow : MonoBehaviour
{
    
    [SerializeField] GameObject solidWindow;
    [SerializeField] GameObject shatteredWindow;
   
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }
    public void BurstWindow()
    {
            StartCoroutine(BurstWindowCorutine());
      
    }
    IEnumerator BurstWindowCorutine()
    {
        shatteredWindow.SetActive(true);
        Destroy(solidWindow);
        foreach (MeshCollider mc in GetComponentsInChildren<MeshCollider>())
        {
            mc.enabled = true;
            mc.GameObject().AddComponent<ConstantForce>().force = new Vector3(Random.Range(0f,1.5f)*20, 0, 0);
        } 
        foreach (BoxCollider mc in GetComponentsInChildren<BoxCollider>())
        {
            mc.enabled = true;
            mc.GameObject().AddComponent<ConstantForce>().force = new Vector3(Random.Range(0f,1.5f)*20, 0, 0);
        }

        foreach (var rb in GetComponentsInChildren<Rigidbody>())
        {
            rb.useGravity = true;
        }
        
        yield return new WaitForSeconds(5);
        Destroy(this.GameObject());
    }
}
