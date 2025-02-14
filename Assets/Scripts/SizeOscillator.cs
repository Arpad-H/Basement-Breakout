using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SizeOscillator : MonoBehaviour
{
    [SerializeField] private float oscillationSpeed = 1f;
    private float maxZHeight;
    private float ogZPos;
    private float time = 0;

    void Start()
    {
        maxZHeight = transform.localScale.z;
        ogZPos = transform.localPosition.z;
    }
    
    void Update()
    {
        time += Time.deltaTime;
        float oscillator = Mathf.Abs(Mathf.Sin(oscillationSpeed + time));
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, maxZHeight * oscillator);
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, ogZPos * oscillator);
    }
}
