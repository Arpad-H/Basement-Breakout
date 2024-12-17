using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Video;

public class ElectricictyManager : MonoBehaviour
{
    /*
     * TODO: TV disablen
     */
    
    private List<Light> lights = new List<Light>();
    private GameObject tv;
    private VideoPlayer Videoplayer;
    
    [Header("Faerie Lights")] 
    public List<GameObject> faerieLights;
    public Material faerieLightsMaterialOn;
    public Material faerieLightsMaterialOff;
    [Header("other Lights")] 
    public List<GameObject> fluorescentLights;
    public Material fluorescentLightsMaterialOn;
    public GameObject electricitySparks;
    private void Awake()
    {
        
        
        LeverInteractable.onLeverAction += LeverInteractableOnonLeverAction;
        
    }

    private void LeverInteractableOnonLeverAction(bool state)
    {
        //Debug.Log($"[ElectricityManager] Lever Interactable is on {state}");
        if (state)
        {
           enableAllLights(); 
           enableTV();
           electricitySparks.SetActive(true);
        }
        else
        {
            disableAllLights();
            disableTV();
            electricitySparks.SetActive(false);
        }
    }

    private void Start()
    {
        
        findAllLights();
        tv = GameObject.Find("TV Demo");
        if (tv != null)
        {
            Videoplayer = tv.GetComponent<VideoPlayer>();
            
        }
        else
        {
            Debug.LogError("[ElectricityManager] GamObject tv is null");
        }
       
    }


    private void findAllLights()
    {
        GameObject[] lightsObjects = GameObject.FindGameObjectsWithTag("Light");
        foreach (GameObject lightObject in lightsObjects)
        {
            Light lightComponent = lightObject.GetComponent<Light>();
            if (lightComponent != null)
            {
                lights.Add(lightComponent);
            }
        }
    }

    private void disableAllLights()
    {
        foreach (Light light in lights)
        {
            light.enabled = false;
        }
        
        faerieLightsMaterialOn.SetInt("Emission" , 0);
        // foreach (GameObject faerieLight in faerieLights)
        // {
        //     MeshRenderer[] meshRenderers = faerieLight.GetComponentsInChildren<MeshRenderer>();
        //     meshRenderers = meshRenderers.Skip(1).ToArray();
        //     foreach (MeshRenderer mr in meshRenderers )
        //     {
        //         mr.material = faerieLightsMaterialOff;
        //     }
        // }
        //
        // foreach (GameObject fluorescentLight in fluorescentLights)
        // {
        //     // fluorescentLight.GetComponentInChildren<MeshRenderer>().material = fluorescentLightsMaterialOff;
        // }
     
        
    }

    private void enableAllLights()
    {
        foreach (Light light in lights)
        {
            light.enabled = true;
            //Debug.Log($"[ElectricityManager] Light {light.name} is enabled");
        }
        faerieLightsMaterialOn.SetInt("Emission" , 1);
        // foreach (GameObject faerieLight in faerieLights)
        // {
        //     MeshRenderer[] meshRenderers = faerieLight.GetComponentsInChildren<MeshRenderer>();
        //     meshRenderers = meshRenderers.Skip(1).ToArray();
        //     foreach (MeshRenderer mr in  meshRenderers)
        //     {
        //         mr.material = faerieLightsMaterialOn;
        //     }
        // }
        // foreach (GameObject fluorescentLight in fluorescentLights)
        // {
        //     fluorescentLight.GetComponentInChildren<MeshRenderer>().material = fluorescentLightsMaterialOn;
        // }
    }



    private void disableTV()
    {
        Videoplayer.enabled = false;
    }

    private void enableTV()
    {
        Videoplayer.enabled = true;
    }
}
