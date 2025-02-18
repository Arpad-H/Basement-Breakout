using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class Marble : MonoBehaviour
{
    [SerializeField] private Transform startTransform;
    [SerializeField] private Transform centerTransform;
    private Rigidbody rb;
    [SerializeField] private AudioSource audioSourceRoll;
    [SerializeField] private AudioSource audioSourceKick;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    
    public void Update()
    {
        if (centerTransform == null)
        {
            gameObject.SetActive(false);
        }
        float length = (new Vector2(centerTransform.position.x, centerTransform.position.z) - new Vector2(transform.position.x, transform.position.z)).magnitude;
        if (length > 0.2f)
        {
            transform.position = startTransform.position;
        }
        if (rb.velocity.magnitude > 0.005f && rb.velocity.magnitude < 1f)
        {
            audioSourceRoll.mute = false;
        }
        else
        {
            //audioSource.pitch = Math.Clamp(2000f*rb.velocity.magnitude-1f, -0.5f, 2f);
            audioSourceRoll.mute = true;
            //audioSourceKick.Play();
        }
    }
}
