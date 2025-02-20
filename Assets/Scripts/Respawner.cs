using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawner : MonoBehaviour
{
    [SerializeField] float min = -1f;
    [SerializeField] float max = 8f;
    
    private Vector3 spawnPoint;
    void Start()
    {
        spawnPoint = transform.position;
    }

    void Update()
    {
        if (transform.position.y < min || transform.position.y > max)
        {
            transform.position = spawnPoint;
        }
    }
}
