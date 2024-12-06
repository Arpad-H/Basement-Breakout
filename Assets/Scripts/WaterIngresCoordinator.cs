using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph;
using UnityEngine;

public class WaterIngresCoordinator : MonoBehaviour
{
    [Header("Objects affected by ingress")]
    public GameObject bookshelf;

    public GameObject burstWindow;
    public GameObject guitar;
    public GameObject beanbag;

    [Header("WaterParticles")] 
    public Material fallingWater;
    public Material groundWaterSplash;
    public Material groundWaterring;

    // Start is called before the first frame update
    void Start()
    {
        Vector4 col = fallingWater.GetColor("_Color");
        col.w = 0f;
        fallingWater.SetColor("_Color", col);
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

    public void DropGuitar()
    {
        guitar.GetComponent<Rigidbody>().useGravity = true;
    }

    public void PushBeanbag()
    {
        beanbag.GetComponent<Rigidbody>().AddForce(Vector3.forward * 50000);
    }
}