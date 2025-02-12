using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VentilatorRotate : MonoBehaviour
{
    public Vector3 rotationSpeed;
    public AudioSource ventilatorSound;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}
