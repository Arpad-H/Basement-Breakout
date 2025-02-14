using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawner : MonoBehaviour
{
    private Vector3 spawnPoint;
    void Start()
    {
        spawnPoint = transform.position;
    }

    void Update()
    {
        if (transform.position.y < -1)
        {
            transform.position = spawnPoint;
        }
    }
}
