using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Video;

public class ElectricictyManager : MonoBehaviour
{
    
    
    
    private GameObject tv;
    private VideoPlayer Videoplayer;
    public bool flooding = false;
    
    [Header("LitScene")] 
    public Texture2D[] lightmapColorLit, lightmapDirLit;
    private LightmapData[] lightmapLit;
    
    [Header("DarkScene")] 
    public Texture2D[] lightmapColorDark, lightmapDirDark;
    private LightmapData[] lightmapDark;
    
    [Header("Other")] 
    public GameObject electricitySparks;
    private void Awake()
    {
        LeverInteractable.onLeverAction += LeverInteractableOnonLeverAction;
    }

    private void LeverInteractableOnonLeverAction(bool state)
    {
        
        if (state)
        {
           enableAllLights(); 
           enableTV();
           if (flooding)
           {
               electricitySparks.SetActive(true);
           }
           
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
        InitLightmaps();


        tv = GameObject.Find("TV Demo");
        if (tv != null)
        {
            Videoplayer = tv.GetComponent<VideoPlayer>();
            
        }
        else
        {
            Debug.LogError("[ElectricityManager] GamObject tv is null");
        }
        // lightmapSet1 = CreateLightmapData(lightmapColor1);
        // lightmapSet2 = CreateLightmapData(lightmapColor2);

      
    }

    private void InitLightmaps()
    {
        List<LightmapData> dlightmap = new List<LightmapData>();
        for (int i = 0; i < lightmapColorDark.Length; i++)
        {
            LightmapData lmd = new LightmapData();
            lmd.lightmapColor = lightmapColorDark[i];
            lmd.lightmapDir = lightmapDirDark[i];
            dlightmap.Add(lmd);
        }
        lightmapDark = dlightmap.ToArray();
        
        List<LightmapData> llightmap = new List<LightmapData>();
        for (int i = 0; i < lightmapColorLit.Length; i++)
        {
            LightmapData lmd = new LightmapData();
            lmd.lightmapColor = lightmapColorLit[i];
            lmd.lightmapDir = lightmapDirLit[i];
            llightmap.Add(lmd);
        }
        lightmapLit = llightmap.ToArray();
        
        LightmapSettings.lightmaps = lightmapLit;
    }

    private void disableAllLights()
    {
       LightmapSettings.lightmaps = lightmapDark;
       // Debug.Log("Disable all lights");
    }

    private void enableAllLights()
    {
        LightmapSettings.lightmaps = lightmapLit;
        // Debug.Log("Enable all lights");
    }



    private void disableTV()
    {
        if (Videoplayer == null)
        {
            return;
        }
        Videoplayer.enabled = false;
    }

    private void enableTV()
    {
        if (Videoplayer == null)
        {
            return;
        }
        Videoplayer.enabled = true;
    }
}
