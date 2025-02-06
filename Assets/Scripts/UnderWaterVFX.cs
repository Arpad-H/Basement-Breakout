using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnderWaterVFX : MonoBehaviour
{
    [SerializeField] private Transform waterPos;
    [SerializeField] private Transform parent;
    [SerializeField] private float transitionDistance = 0.2f;
    [SerializeField] private float transitionStrength = 3.5f;
    private Color waterColor;// = new Color(0f, 0.6f, 1f, 0.77f);
    private float maxAlpha;
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
        if (parent.transform.position.y < waterPos.position.y)
        {
            //gameObject.GetComponent<MeshRenderer>().enabled = true;
            waterColor.a = maxAlpha;
            gameObject.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", waterColor);
        }
        else if (distance <= transitionDistance)
        {
            waterColor.a = maxAlpha - (distance * transitionStrength);
            gameObject.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", waterColor);
        }
        else
        {
            //gameObject.GetComponent<MeshRenderer>().enabled = false;
            waterColor.a = 0.0f;
            gameObject.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", waterColor);
        }
    }
}
