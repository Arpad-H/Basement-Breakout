using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateRoom : MonoBehaviour
{
    [SerializeField]GameObject room;
    // Start is called before the first frame update
    private void Awake()
    {
        room.SetActive(false);
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            room.SetActive(true);
        }
      
    }
}
