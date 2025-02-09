using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class UnderWaterVFX : MonoBehaviour
{
    [SerializeField] private Transform waterPos;
    [SerializeField] private Transform parent;
    [SerializeField] private float transitionDistance = 0.2f;
    [SerializeField] private float transitionStrength = 3.5f;
    [SerializeField] private float drownTime = 15f;
    [SerializeField] private VolumeProfile underwaterProfile;
    [SerializeField] private VolumeProfile aboveWaterProfile;
    [SerializeField] private Volume volume;
    
    private Color waterColor;// = new Color(0f, 0.6f, 1f, 0.77f);
    private float maxAlpha;
    private float timer = 0;
    void Start()
    {
        //gameObject.GetComponent<MeshRenderer>().enabled = false;
        waterColor = GetComponent<Renderer>().material.color;
        maxAlpha = waterColor.a;
        waterColor.a = 0.0f;
        gameObject.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", waterColor);
    }

    void Update()
    {
        float distance = parent.transform.position.y - waterPos.position.y;
       // Debug.Log("distnace: " + distance);
        if (parent.transform.position.y < waterPos.position.y)
            
        {
            //gameObject.GetComponent<MeshRenderer>().enabled = true;
         //   waterColor.a = maxAlpha + timer/drownTime;
            waterColor.a = maxAlpha;
            gameObject.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", waterColor);
            timer += Time.deltaTime;
            RenderSettings.fogDensity = Mathf.Min(0.7f, Mathf.Abs(distance)*2 );
            volume.enabled = true;
            StartCoroutine(FadePostProcessing(true, 0.5f));
            volume.weight = 1;


        }
        else if (distance <= transitionDistance)
        {
            RenderSettings.fogDensity = distance * 0.1f;
            timer = 0;
            waterColor.a = maxAlpha - (distance * transitionStrength);
            gameObject.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", waterColor);
        }
        else
        {
          StartCoroutine(FadePostProcessing(false, 1f));
            RenderSettings.fogDensity = 0.0f;
            timer = 0;
            //gameObject.GetComponent<MeshRenderer>().enabled = false;
            waterColor.a = 0.0f;
            gameObject.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", waterColor);
        }
    }
    IEnumerator FadePostProcessing(bool enable, float duration)
    {
        float startWeight = volume.weight;
        float targetWeight = enable ? 1f : 0f;
        float time = 0;

        while (time < duration)
        {
            volume.weight = Mathf.Lerp(startWeight, targetWeight, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        volume.enabled = enable;
        volume.weight = targetWeight;  // Ensure final value is set
        
    }
}
