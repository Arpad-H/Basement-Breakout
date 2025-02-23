using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class UnderWaterVFX : MonoBehaviour
{
   
   [SerializeField] private Transform EyeHeight;
   [SerializeField] private Transform waterPlane;
    [SerializeField] private float transitionDistance = 0.2f;
    [SerializeField] private float transitionStrength = 3.5f;
    [SerializeField] private float drownTime = 15f;
    [SerializeField] private VolumeProfile underwaterProfile;
    [SerializeField] private VolumeProfile aboveWaterProfile;
    [SerializeField] private Volume volume;
    // [SerializeField] private Material distortionMaterial;
  
    //VOLUME PP EFFECTS
    private ColorAdjustments colorAdjustments;
    private Vignette vignette;

    
    
    [SerializeField] private float distortionStrength = 0.1f;
    private float currentDistortion = 0.0f;
    private WaterBehaviour waterBehaviour;
    
  
    private float timer = 0;
    void Start()
    {
        waterBehaviour = FindObjectOfType<WaterBehaviour>();
        underwaterProfile.TryGet(out colorAdjustments);
        colorAdjustments.contrast.overrideState = true;
        underwaterProfile.TryGet(out vignette);
      
    }

    void Update()
    {
       
       // Vector3 waterHeight = waterPlane.position; 
       
        Vector3 EyeHeightWorld = EyeHeight.transform.position;
        Vector3 waterHeight = waterBehaviour.GetWaveDisplacement(EyeHeightWorld, Time.time);
        float distance = EyeHeight.transform.position.y - waterHeight.y;
       // Debug.Log("EyeHeightWorld: " + EyeHeightWorld + " waterHeight: " + waterHeight);
      //   avgDistance = EyeHeight.transform.position.y - waterPlane.position.y;
        if (EyeHeightWorld.y < waterHeight.y) //FULLY UNDERWATER
        {
          
            timer += Time.deltaTime;
            RenderSettings.fogDensity = Mathf.Min(0.7f, Mathf.Abs(distance)*2 );
            volume.enabled = true;
            // StartCoroutine(FadePostProcessing(true, 0.5f));
            volume.weight = 1;
            // distortionMaterial.SetFloat("_blend", 0.1f);


        }
        else if (Mathf.Abs(distance) <= transitionDistance) //TRANSITION
        {
            // RenderSettings.fogDensity = distance * 0.1f;
            volume.enabled = true;
            volume.weight = 1- distance * transitionStrength;
            colorAdjustments.contrast.Override(Mathf.Lerp(-60f, 0f, Mathf.Abs(distance) * 5)); 
                
        }
        else //ABOVE WATER
        {
            // distortionMaterial.SetFloat("_blend", 0.0f);
          // StartCoroutine(FadePostProcessing(false, 1f));
            RenderSettings.fogDensity = 0.0f;
            timer = 0;
            volume.weight = 0;
            volume.enabled = false;
           
            //gameObject.GetComponent<MeshRenderer>().enabled = false;
           
        }
    }
    IEnumerator FadePostProcessing(bool enable, float duration)
    {
        float startWeight = volume.weight;
        float targetWeight = enable ? 1f : 0f;
        float targetDistortion = enable ? distortionStrength : 0f;
        float startDistortion = enable ? 0f : distortionStrength;
        float time = 0;
        

        while (time < duration)
        {
       //     distortionMaterial.SetFloat("_blend", Mathf.Lerp(startDistortion, targetDistortion, time / duration));
            volume.weight = Mathf.Lerp(startWeight, targetWeight, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        volume.enabled = enable;
       // distortionMaterial.SetFloat("_blend", targetDistortion);  // Ensure final value is set
        volume.weight = targetWeight;  // Ensure final value is set
        
    }
}
