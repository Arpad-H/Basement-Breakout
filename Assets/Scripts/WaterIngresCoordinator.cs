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
    public Material groundWaterDrop;
    public Material runningWaterSheet;
    public Material waterStream;
    public Material waterDroplets;
    public Material groundWaterRings;
    public GameObject lighningSpark;
   
    

    // Start is called before the first frame update
    void Start()
    {
        Vector4 colFallingWater = fallingWater.GetColor("_Color");
        Vector4 colGroundWaterSplash = groundWaterSplash.GetColor("_Color");
        Vector4 colGroundWaterDrop = groundWaterDrop.GetColor("_Color");
        Vector4 colRunningWaterSheet = runningWaterSheet.GetColor("_Color");
        Vector4 colWaterStream = waterStream.GetColor("_Color");
        Vector4 colWaterDroplets = waterDroplets.GetColor("_Color");
        Vector4 colGroundWaterRings = groundWaterRings.GetColor("_Color");
    
        colFallingWater.w = 0f;
        colGroundWaterSplash.w = 0f;
        colGroundWaterDrop.w = 0f;
        colRunningWaterSheet.w = 0f;
        colWaterStream.w = 0f;
        colWaterDroplets.w = 0f;
        colGroundWaterRings.w = 0f;
       
        fallingWater.SetColor("_Color", colFallingWater);
        groundWaterSplash.SetColor("_Color", colGroundWaterSplash);
        groundWaterDrop.SetColor("_Color", colGroundWaterDrop);
        runningWaterSheet.SetColor("_Color", colRunningWaterSheet);
        waterStream.SetColor("_Color", colWaterStream);
        waterDroplets.SetColor("_Color", colWaterDroplets);
        groundWaterRings.SetColor("_Color", colGroundWaterRings);
       
        
        lighningSpark.SetActive(false);
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
    public void StartParticles()
    {
        // Define target alpha for the colors
        float targetAlpha = 1f;
        float lerpDuration = 6f; // Duration of the lerp in seconds
    
        // Start coroutines for each color transition
        StartCoroutine(LerpMaterialColor(fallingWater, "_Color", targetAlpha, lerpDuration));
        StartCoroutine(LerpMaterialColor(groundWaterSplash, "_Color", targetAlpha, lerpDuration));
        StartCoroutine(LerpMaterialColor(groundWaterDrop, "_Color", targetAlpha, lerpDuration));
        StartCoroutine(LerpMaterialColor(runningWaterSheet, "_Color", targetAlpha, lerpDuration));
        StartCoroutine(LerpMaterialColor(waterStream, "_Color", targetAlpha, lerpDuration));
        StartCoroutine(LerpMaterialColor(waterDroplets, "_Color", targetAlpha, lerpDuration));
        StartCoroutine(LerpMaterialColor(groundWaterRings, "_Color", targetAlpha, lerpDuration));
    
        // Activate the lightning spark effect
        lighningSpark.SetActive(true);
    }

    private IEnumerator LerpMaterialColor(Material material, string colorProperty, float targetAlpha, float duration)
    {
        Color initialColor = material.GetColor(colorProperty);
        Color targetColor = initialColor;
        targetColor.a = targetAlpha; // Set target alpha value
    
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            float t = timeElapsed / duration;
            Color newColor = Color.Lerp(initialColor, targetColor, t);
            material.SetColor(colorProperty, newColor);

            timeElapsed += Time.deltaTime;
            yield return null; // Wait until the next frame
        }

        // Ensure the final color is exactly the target color
        material.SetColor(colorProperty, targetColor);
    }
}
