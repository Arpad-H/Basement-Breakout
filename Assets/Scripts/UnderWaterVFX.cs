using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnderWaterVFX : MonoBehaviour
{
    [SerializeField] private Transform waterPos;
    [SerializeField] private Transform parent;
    [SerializeField] private float transitionDistance = 0.2f;
    [SerializeField] private float transitionStrength = 3.5f;
    [SerializeField] private float drownTime = 15f;
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
        Debug.Log("distnace: " + distance);
        if (parent.transform.position.y < waterPos.position.y)
            
        {
            //gameObject.GetComponent<MeshRenderer>().enabled = true;
          //  waterColor.a = maxAlpha + timer/drownTime;
            waterColor.a = maxAlpha;
            gameObject.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", waterColor);
            timer += Time.deltaTime;
            RenderSettings.fogDensity = Mathf.Min(0.7f, Mathf.Abs(distance)*3 );
           
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
            RenderSettings.fogDensity = 0.0f;
            timer = 0;
            //gameObject.GetComponent<MeshRenderer>().enabled = false;
            waterColor.a = 0.0f;
            gameObject.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", waterColor);
        }
    }
}
