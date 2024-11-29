using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricictyManager : MonoBehaviour
{
    /*
     * TODO: TV disablen
     */
    
    private List<Light> lights = new List<Light>();
    private void Awake()
    {
        findAllLights();
        
        LeverInteractable.onLeverAction += LeverInteractableOnonLeverAction;
        
    }

    private void LeverInteractableOnonLeverAction(bool state)
    {
        if (state)
        {
           enableAllLights(); 
        }
        else
        {
            disableAllLights();
        }
    }


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
    }

    private void enableAllLights()
    {
        foreach (Light light in lights)
        {
            light.enabled = true;
        }
    }
}
