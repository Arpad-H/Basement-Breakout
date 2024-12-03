using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterIngresCoordinator : MonoBehaviour
{
    public GameObject bookshelf;
    public GameObject burstWindow;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void TipOverBookshelf()
    {
       
        bookshelf.GetComponent<Rigidbody>().AddForce(Vector3.right * 10000);
    }
    public void BurstWindow()
    {
        burstWindow.GetComponent<burstingWindow>().BurstWindow();
    }
}
