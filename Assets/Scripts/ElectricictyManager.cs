using System;
using System.Collections;
using System.Collections.Generic;
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
    private void Awake()
    {
        
        
        LeverInteractable.onLeverAction += LeverInteractableOnonLeverAction;
        
    }

    private void LeverInteractableOnonLeverAction(bool state)
    {
        Debug.Log($"[ElectricityManager] Lever Interactable is on {state}");
        if (state)
        {
           enableAllLights(); 
           enableTV();
        }
        else
        {
            disableAllLights();
            disableTV();
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
        {
            
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
            Debug.Log($"[ElectricityManager] Light {light.name} is enabled");
        }
    }

    private void enableAllLights()
    {
        foreach (Light light in lights)
        {
            light.enabled = true;
            Debug.Log($"[ElectricityManager] Light {light.name} is enabled");
        }
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
