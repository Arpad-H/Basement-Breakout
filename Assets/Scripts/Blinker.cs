using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blinker : MonoBehaviour
{
    [SerializeField] private Material red;
    [SerializeField] private Material black;
    [SerializeField] private float speed;
    private float time = 0;
    
    void Update()
    {
        time += Time.deltaTime;
        float oscillator = Mathf.Sin(speed * time);
        if (oscillator > 0) {
            gameObject.GetComponent<MeshRenderer>().material = black;
        }else if (oscillator < 0) {
            gameObject.GetComponent<MeshRenderer>().material = red;
        }
    }

    private void OnDisable()
    {
        gameObject.GetComponent<MeshRenderer>().material = black;
    }
}
